namespace Shared.DataTransferObjects.BasketModuleDTOs
{
    public class BasketDTO
    {
        // Nullable on purpose — the frontend never sends this.
        // The service always overwrites it with the authenticated UserId from the JWT.
        public string? Id { get; set; }

        public ICollection<BasketItemDTO> Items { get; set; } = [];
        public string? clientSecret { get; set; }
        public string? paymentIntentId { get; set; }
        public int? deliveryMethodId { get; set; }
        public decimal? shippingPrice { get; set; }
    }
}