namespace Shared.DataTransferObjects.IdentityDTOs
{
    /// <summary>
    /// Returned by Login, Register, and GetCurrentUser.
    /// Token is null when returned from GetCurrentUser/UpdateProfile in cases
    /// where the frontend already holds a valid token and doesn't need a new one
    /// — but per the frontend's request, GetCurrentUser DOES refresh the token
    /// here so a single response can both refresh auth state AND populate the
    /// profile page without a second round trip.
    /// </summary>
    public class UserDTO
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public IList<string> Roles { get; set; } = [];
        public AddressDTO? Address { get; set; }
    }
}