
    namespace DomainLayer.Exceptions
    {
    public sealed class DeliveryMethodNotFoundException(int id) : Exception($"Delivery method with id {id} was not found.")
    {
      
    }
}