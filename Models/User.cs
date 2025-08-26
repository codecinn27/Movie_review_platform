using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{

    public enum UserRole
    {
        Admin, //0
        User  //1
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public required string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.User; // default to User
        
        // Initialize the collection
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}