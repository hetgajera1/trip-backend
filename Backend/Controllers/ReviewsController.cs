using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("cabin/{cabinId}")]
        public async Task<ActionResult<List<Review>>> GetByCabin(string cabinId) =>
            await _reviewService.GetByCabinIdAsync(cabinId);

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> Get(string id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound();
            return review;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Review review)
        {
            await _reviewService.CreateAsync(review);
            return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
        }
    }
}
