using Shared.DataTransferObjects.IdentityDTOs;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class OrderDTO
    {
        // BasketId removed — resolved server-side from the authenticated UserId
        public int DeliveryMethodId { get; set; }
        public AddressDTO shipToAddress { get; set; } = default!;
    }
}