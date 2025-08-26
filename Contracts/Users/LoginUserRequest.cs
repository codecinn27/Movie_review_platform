using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Contracts
{
    public class LoginUserRequest
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}