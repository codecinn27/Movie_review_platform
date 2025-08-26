namespace MovieAPI.Contracts
{
    public class MovieDto
    {
        public Guid Id { get; set; }                 // maps to Movie.Id
        public string Title { get; set; } = string.Empty;         // maps to Movie.Title
        public string Description { get; set; } = string.Empty;   // maps to Movie.Description
        public DateTime ReleaseDate { get; set; }    // maps to Movie.ReleaseDate
        public double AverageRating { get; set; }    // maps to Movie.AverageRating
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

    public class MovieRatingDto
    {
        public string MovieTitle { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
