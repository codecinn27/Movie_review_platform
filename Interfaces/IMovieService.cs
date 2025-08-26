using MovieAPI.Contracts;
using MovieAPI.Models;

namespace MovieAPI.Services
{
    public interface IMovieService
    {
        Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto);
        Task<MovieDto?> GetMovieByIdAsync(Guid id);
        Task<MovieResponseDto> UpdateMovieAsync(Guid id, UpdateMovieDto dto);
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
        Task DeleteMovieAsync(Guid id);
        Task<IEnumerable<ReviewDto>> GetReviewsByMovieIdAsync(Guid movieId);
        Task<MovieRatingDto?> GetMovieRatingAsync(Guid movieId);


    }
}
