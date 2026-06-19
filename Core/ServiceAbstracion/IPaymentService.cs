using Shared.DataTransferObjects.BasketModuleDTOs;

namespace ServiceAbstracion
{
    public interface IPaymentService
    {
        Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string userId, int deliveryMethodId);
        Task HandleWebhookEventAsync(string json, string stripeSignatureHeader);
    }
}