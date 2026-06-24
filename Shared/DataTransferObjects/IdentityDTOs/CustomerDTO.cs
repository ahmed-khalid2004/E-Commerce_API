namespace Shared.DataTransferObjects.IdentityDTOs
{
    public class CustomerDTO
    {
        public string Id { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}