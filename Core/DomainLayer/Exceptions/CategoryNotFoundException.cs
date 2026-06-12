namespace DomainLayer.Exceptions
{
    public class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id)
            : base($"Category with Id {id} was not found.")
        {
        }
    }
}