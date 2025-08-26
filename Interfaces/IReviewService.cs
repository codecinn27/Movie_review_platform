using MovieAPI.Contracts;

namespace MovieAPI.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(Guid movieId, CreateReviewDto dto, Guid userId);
        Task<ReviewDto> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, Guid userId);
        Task<ReviewDto> GetReviewByIdAsync(Guid reviewId);

        Task<DeleteResponseDto> DeleteReviewAsync(Guid reviewId, Guid userId);
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();

        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(Guid userId) 
            => throw new NotImplementedException();

    }
}
