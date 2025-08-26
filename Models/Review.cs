using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Range(1, 5, ErrorMessage = "The rating must be between 0 and 5.")]
        public int Rating { get; set; }

        public required string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; } 

        // must have Foreign keys
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }

        // Navigation properties
        public required User User { get; set; } = null;
        public required Movie Movie { get; set; } = null;
    }
}