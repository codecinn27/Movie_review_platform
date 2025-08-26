using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Contracts
{
    public class CreateReviewDto
    {
        public required int Rating { get; set; }
        public required string Comment { get; set; }
    }

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? MovieTitle { get; set; }
    }

    // DTO for updating a review
    public class UpdateReviewDto
    {
        [Range(0, 5, ErrorMessage = "The rating must be between 0 and 5.")]
        public int? Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; } = string.Empty;
    }
    

    public class DeleteResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }


}
