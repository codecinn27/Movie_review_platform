using Microsoft.EntityFrameworkCore;
using MovieAPI.AppDataContext;
using MovieAPI.Contracts;
using MovieAPI.Models;

namespace MovieAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly MovieDbContext _context;

        public MovieService(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<MovieDto>> GetAllMoviesAsync(PaginationParams paginationParams)
        {

            // Get total count first
            var totalCount = await _context.Movies.CountAsync();

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / paginationParams.PageSize);
            var skip = (paginationParams.Page - 1) * paginationParams.PageSize;


            // query Movies from DbContext and project to MovieDto
            var movies = await _context.Movies
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    ReleaseDate = m.ReleaseDate,
                    AverageRating = m.Reviews.Any() ? m.Reviews.Average(r => r.Rating) : 0,
                    Reviews = m.Reviews
                        .Select(r => new ReviewDto
                        {
                            Id = r.Id,
                            UserName = r.User.Name,
                            Rating = r.Rating,
                            Comment = r.Comment,
                            CreatedAt = r.CreatedAt,
                            MovieTitle = null
                        })
                        .ToList()
                })
                .ToListAsync();

             return new PaginatedResponse<MovieDto>
            {
                Data = movies,
                Page = paginationParams.Page,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNextPage = paginationParams.Page < totalPages,
                HasPreviousPage = paginationParams.Page > 1
            };
        }

        // Overload to maintain backward compatibility
        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            var paginationParams = new PaginationParams { Page = 1, PageSize = 50 };
            var result = await GetAllMoviesAsync(paginationParams);
            return result.Data;
        }

        public async Task DeleteMovieAsync(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                throw new Exception("Movie not found.");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }



        public async Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto)
        {

            // Normalize title for consistent comparison
            var normalizedTitle = dto.Title.Trim();

            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = normalizedTitle, // Use normalized title
                Description = dto.Description,
                // If dto.ReleaseDate is null, default to UTC+8
                ReleaseDate = dto.ReleaseDate ?? DateTime.UtcNow.AddHours(8)
            };

            try
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a duplicate key constraint violation
                if (ex.InnerException?.Message.Contains("Duplicate entry") == true ||
                    ex.InnerException?.Message.Contains("duplicate key") == true)
                {
                    throw new InvalidOperationException($"Movie title '{normalizedTitle}' already exists. Please choose a different title.");
                }
                
                // Re-throw if it's a different kind of database error
                throw;
            }

            return new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Rating = 0 // initially no reviews
            };
        }

        public async Task<MovieDto?> GetMovieByIdAsync(Guid id)
        {
            var movie = await _context.Movies
                .Include(m => m.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return null;

            double averageRating = movie.Reviews.Any()
                ? movie.Reviews.Average(r => r.Rating)
                : 0;

            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                AverageRating = Math.Round(averageRating, 1), // Round to 1 decimal place
                Reviews = movie.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserName = r.User.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }

        public async Task<MovieResponseDto> UpdateMovieAsync(Guid id, UpdateMovieDto dto)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                throw new Exception("Movie not found.");

            // Check if the new title (if provided) is unique
            if (!string.IsNullOrEmpty(dto.Title) &&
                dto.Title != movie.Title &&
                await _context.Movies.AnyAsync(m => m.Title == dto.Title))
            {
                throw new Exception("Movie title already exists. Please choose a different title.");
            }

            // Update fields if provided
            movie.Title = dto.Title ?? movie.Title;
            movie.Description = dto.Description ?? movie.Description;
            movie.ReleaseDate = dto.ReleaseDate ?? movie.ReleaseDate;

            await _context.SaveChangesAsync();

            return new MovieResponseDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Rating = movie.Reviews.Any() ? movie.Reviews.Average(r => r.Rating) : 0
            };
        }


        public async Task<IEnumerable<ReviewDto>> GetReviewsByMovieIdAsync(Guid movieId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .Where(r => r.MovieId == movieId)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserName = r.User.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                LastUpdated = r.LastUpdated,
                MovieTitle = r.Movie.Title
            });
        }
        
        public async Task<MovieRatingDto> GetMovieRatingAsync(Guid movieId)
        {
            var movie = await _context.Movies
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
                return null;

            var totalReviews = movie.Reviews.Count;
            var averageRating = totalReviews > 0
                ? movie.Reviews.Average(r => r.Rating)
                : 0;

            return new MovieRatingDto
            {
                MovieTitle = movie.Title,
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = totalReviews
            };
        }


    }
}
