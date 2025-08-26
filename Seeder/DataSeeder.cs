using MovieAPI.Models;
using MovieAPI.AppDataContext;

namespace MovieAPI.Data
{
    public class DataSeeder
    {
        private readonly MovieDbContext _context;

        public DataSeeder(MovieDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Prevent running multiple times
            if (_context.Movies.Any() || _context.Reviews.Any())
                return;

            // --- Get existing users from DB ---
            var user1 = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse("08dde091-b558-4fd1-856d-2e40b18ed85f"));
            var user2 = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse("08dde383-606b-4818-8b9e-975904d3c5a2"));
            var user3 = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse("08dde451-cf46-4584-8b60-522dbc9995d7"));

            if (user1 == null || user2 == null || user3 == null)
                throw new Exception("One or more users not found in the database. Please make sure the users exist.");

            var users = new List<User> { user1, user2, user3 };

            // --- Create 20 Movies ---
            var movies = new List<Movie>();
            for (int i = 1; i <= 20; i++)
            {
                movies.Add(new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = $"Movie {i}",
                    Description = $"Description for Movie {i}",
                    ReleaseDate = DateTime.UtcNow.AddDays(-i * 10)
                });
            }
            _context.Movies.AddRange(movies);

            // --- Create 40 Reviews (2 per movie) ---
            var reviews = new List<Review>();
            var rnd = new Random();

            foreach (var movie in movies)
            {
                // Pick 2 random users from existing users
                var selectedUsers = users.OrderBy(x => rnd.Next()).Take(2).ToList();

                foreach (var user in selectedUsers)
                {
                    reviews.Add(new Review
                    {
                        Id = Guid.NewGuid(),
                        Rating = rnd.Next(1, 6),
                        Comment = "Sample review comment.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = user.Id,
                        MovieId = movie.Id,
                        User = user,
                        Movie = movie
                    });
                }
            }

            _context.Reviews.AddRange(reviews);

            // --- Save to DB ---
            _context.SaveChanges();
        }
    }
}
