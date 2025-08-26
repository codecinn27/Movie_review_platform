namespace MovieAPI.Contracts
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Optional: include reviews if needed
        public List<ReviewResponseDto>? Reviews { get; set; }
    }

    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid MovieId { get; set; }
    }
}