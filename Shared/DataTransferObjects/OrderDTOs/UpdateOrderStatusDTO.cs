using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public string Status { get; set; } = default!;
    }
}
