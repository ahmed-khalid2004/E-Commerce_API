using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.BasketModuleDTOs
{
    public class CreatePaymentIntentDTO
    {
        [Required]
        public int DeliveryMethodId { get; set; }
    }
}