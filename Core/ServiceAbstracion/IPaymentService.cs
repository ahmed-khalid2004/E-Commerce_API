using Shared.DataTransferObjects.BasketModuleDTOs;

namespace ServiceAbstracion
{
    public interface IPaymentService
    {
        Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);
        Task HandleWebhookEventAsync(string json, string stripeSignatureHeader);
    }
}