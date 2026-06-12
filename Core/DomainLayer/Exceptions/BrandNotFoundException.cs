namespace DomainLayer.Exceptions
{
    public class BrandNotFoundException : NotFoundException
    {
        public BrandNotFoundException(int id)
            : base($"Brand with Id {id} was not found.") { }
    }
}