using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Contracts
{
    public class CreateMovieDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
        // Nullable so we can check for null
        public DateTime? ReleaseDate { get; set; }
    }

    public class MovieResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public double Rating { get; set; }
    }
}
