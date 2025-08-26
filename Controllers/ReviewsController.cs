using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services;
using MovieAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Test endpoint - try this first
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "ReviewsController is working!" });
        }

        // Simple GET all reviews (for testing)
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to fetch reviews" });
            }
        }



        // GET: api/review/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(Guid id)
        {
            Console.WriteLine($"Controller received ID: {id}");

            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"KeyNotFoundException: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PUT: api/review/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get logged-in user's ID from JWT
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(id, dto, userId);
                return Ok(updatedReview);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE: api/review/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReviews(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var result = await _reviewService.DeleteReviewAsync(id, userId);

                if (result.Success)
                {
                    return Ok(result);
                }

                // This case normally won’t happen as service throws exception on failure
                return BadRequest(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new DeleteResponseDto
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new DeleteResponseDto
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new DeleteResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        } 

    }
}
