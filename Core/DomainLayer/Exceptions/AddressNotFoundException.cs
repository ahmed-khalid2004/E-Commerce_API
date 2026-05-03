
namespace DomainLayer.Exceptions
{
    public sealed class AddressNotFoundException(string UserName) : NotFoundException($"Address for user {UserName} not found.")
    {
       
    }
}