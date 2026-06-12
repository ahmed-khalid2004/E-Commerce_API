namespace DomainLayer.Exceptions
{
    public class ProductTypeNotFoundException : NotFoundException
    {
        public ProductTypeNotFoundException(int id)
            : base($"Product type with Id {id} was not found.") { }
    }
}