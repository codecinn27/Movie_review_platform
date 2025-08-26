using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Models
{
    public class Movie
    {
        [Key]
        public Guid Id { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }

        public DateTime ReleaseDate { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        
        [NotMapped] // EF won't persist this to DB, it's computed at runtime
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
    }
}