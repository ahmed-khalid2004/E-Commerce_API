namespace DomainLayer.Exceptions
{
    public class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException(Guid id)
            : base($"Order with Id = {id} is not found.")
        {
        }
    }
}