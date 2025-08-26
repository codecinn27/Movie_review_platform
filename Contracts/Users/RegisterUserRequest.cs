using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Contracts
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public required string Password { get; set; }
    }
}