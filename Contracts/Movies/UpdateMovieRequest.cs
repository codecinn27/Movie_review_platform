namespace MovieAPI.Contracts
{
    public class UpdateMovieDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
