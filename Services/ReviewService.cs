using MovieAPI.Contracts;
using MovieAPI.Models;
using Microsoft.EntityFrameworkCore;
using MovieAPI.AppDataContext;

namespace MovieAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly MovieDbContext _context;

        public ReviewService(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewDto> CreateReviewAsync(Guid movieId, CreateReviewDto dto, Guid userId)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
            if (movie == null) throw new KeyNotFoundException("Movie not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            // Check for existing review
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);

            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this movie.");

            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                MovieId = movie.Id,
                User = user,
                Movie = movie
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                UserName = user.Name,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                MovieTitle = movie.Title
            };
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid reviewId)
        {

            // Debug: Log the GUID being searched for
            Console.WriteLine($"Searching for review with ID: {reviewId}");

            // First, let's check if ANY reviews exist
            var totalReviews = await _context.Reviews.CountAsync();
            Console.WriteLine($"Total reviews in database: {totalReviews}");

            // Get all review IDs to see what's actually in the database
            var allReviewIds = await _context.Reviews.Select(r => r.Id).ToListAsync();
            Console.WriteLine($"All review IDs in database: {string.Join(", ", allReviewIds)}");

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            Console.WriteLine($"Review found: {review != null}");

            if (review == null)
            {
                // Try to find the review without includes to see if that's the issue
                var reviewWithoutIncludes = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Id == reviewId);

                Console.WriteLine($"Review found without includes: {reviewWithoutIncludes != null}");

                throw new KeyNotFoundException($"Review not found with ID: {reviewId}");
            }

            return new ReviewDto
            {
                Id = review.Id,
                UserName = review.User?.Name ?? "Unknown User",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                LastUpdated = review.LastUpdated,
                MovieTitle = review.Movie?.Title ?? "Unknown Movie"
            };
        }

        public async Task<ReviewDto> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, Guid userId)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            // Get the user performing the update
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null)
                throw new KeyNotFoundException("User not found");

            // Check if the user owns this review
            if (review.UserId != userId && currentUser.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("You can only update your own reviews");

            // Ensure at least one field is provided
            if (dto.Rating == null && string.IsNullOrWhiteSpace(dto.Comment))
                throw new InvalidOperationException("You must provide at least a rating or a comment to update");

            // Update the review
            if (dto.Rating != null)
                review.Rating = dto.Rating.Value;

            if (!string.IsNullOrWhiteSpace(dto.Comment))
                review.Comment = dto.Comment;

            review.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                UserName = review.User.Name,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                LastUpdated = review.LastUpdated,
                MovieTitle = review.Movie.Title
            };
        }

        public async Task<DeleteResponseDto> DeleteReviewAsync(Guid reviewId, Guid userId)
        {
            // Get the review with its user
            var review = await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return new DeleteResponseDto
                {
                    Success = false,
                    Message = "Review not found"
                };

            // Get the current user performing the deletion
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null)
                return new DeleteResponseDto
                {
                    Success = false,
                    Message = "User not found"
                };

            // Check if the user is the owner or an Admin
            if (review.UserId != userId && currentUser.Role != UserRole.Admin)
                return new DeleteResponseDto
                {
                    Success = false,
                    Message = "You can only delete your own reviews"
                };

            // Remove the review
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return new DeleteResponseDto
            {
                Success = true,
                Message = "Review deleted successfully"
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
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

    }
}
