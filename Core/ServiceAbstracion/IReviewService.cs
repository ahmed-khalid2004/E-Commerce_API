using Shared.DataTransferObjects.ProductModuleDTOs;
namespace ServiceAbstracion
{
    public interface IReviewService
    {
        Task<IEnumerable<ProductReviewDTO>> GetReviewsForProductAsync(int productId);
        Task<ProductReviewDTO> CreateOrUpdateReviewAsync(string userId, string userDisplayName, CreateReviewDTO dto);
        Task<ProductReviewDTO> CreateReplyAsync(string userId, string userDisplayName, CreateReplyDTO dto);
        Task<ProductReviewDTO> UpdateReviewAsync(int reviewId, string userId, UpdateReviewDTO dto);
        Task DeleteReviewAsync(int reviewId, string userId);
    }
}