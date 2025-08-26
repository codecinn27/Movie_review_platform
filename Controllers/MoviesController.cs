using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Contracts;
using MovieAPI.Services;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IReviewService _reviewService;

        public MoviesController(IMovieService movieService, IReviewService reviewService)
        {
            _movieService = movieService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] PaginationParams paginationParams)
        {
            try
            {
                var result = await _movieService.GetAllMoviesAsync(paginationParams);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/movies
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MovieResponseDto>> CreateMovie([FromBody] CreateMovieDto dto)
        {
            try
            {
                var movie = await _movieService.CreateMovieAsync(dto);
                return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only admin can delete
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            try
            {
                await _movieService.DeleteMovieAsync(id);
                return Ok(new { message = $"Movie with ID {id} has been successfully deleted." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET api/movies/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovieById(Guid id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        // PUT api/movies/{id} - Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<MovieResponseDto>> UpdateMovie(Guid id, [FromBody] UpdateMovieDto dto)
        {
            try
            {
                var movie = await _movieService.UpdateMovieAsync(id, dto);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{movieId}/reviews")]
        [AllowAnonymous] // Public endpoint
        public async Task<IActionResult> GetReviewsForMovie(Guid movieId)
        {
            try
            {
                var reviews = await _movieService.GetReviewsByMovieIdAsync(movieId);
                return Ok(reviews);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Movie not found" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to fetch reviews" });
            }
        }




        // ✅ create new review: POST api/movies/{id}/reviews
        [HttpPost("{id}/reviews")]
        [Authorize] // Any authenticated user can post a review
        public async Task<IActionResult> CreateReview(Guid id, [FromBody] CreateReviewDto dto)
        {
            // Get logged-in user's ID from JWT
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var review = await _reviewService.CreateReviewAsync(id, dto, userId);
                return CreatedAtAction(nameof(CreateReview), new { movieId = id, reviewId = review.Id }, review);
            }
            catch (InvalidOperationException ex)
            {
                // Return a simple 400 Bad Request with just the message
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Generic 500 error without full stack trace
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }

        }

        [HttpGet("{movieId}/rating")]
        public async Task<IActionResult> GetMovieRating(Guid movieId)
        {
            var rating = await _movieService.GetMovieRatingAsync(movieId);

            if (rating == null)
            {
                return NotFound(new { message = "No reviews found for this movie." });
            }

            return Ok(rating);
        }

    }
}
