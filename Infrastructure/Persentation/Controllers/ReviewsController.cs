using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.DataTransferObjects.ProductModuleDTOs;

namespace Presentation.Controllers
{
    public class ReviewsController(IServiceManager _serviceManager) : ApiBaseController
    {
        [AllowAnonymous]
        [HttpGet("product/{productId:int}")]
        public async Task<ActionResult<IEnumerable<ProductReviewDTO>>> GetProductReviews(int productId)
            => Ok(await _serviceManager.ReviewService.GetReviewsForProductAsync(productId));

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductReviewDTO>> CreateOrUpdateReview(CreateReviewDTO dto)
        {
            var result = await _serviceManager.ReviewService
                .CreateOrUpdateReviewAsync(GetUserIdFromToken(), GetDisplayNameFromToken(), dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("reply")]
        public async Task<ActionResult<ProductReviewDTO>> CreateReply(CreateReplyDTO dto)
        {
            var result = await _serviceManager.ReviewService
                .CreateReplyAsync(GetUserIdFromToken(), GetDisplayNameFromToken(), dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductReviewDTO>> UpdateReview(int id, UpdateReviewDTO dto)
        {
            var result = await _serviceManager.ReviewService.UpdateReviewAsync(id, GetUserIdFromToken(), dto);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _serviceManager.ReviewService.DeleteReviewAsync(id, GetUserIdFromToken());
            return NoContent();
        }
    }
}