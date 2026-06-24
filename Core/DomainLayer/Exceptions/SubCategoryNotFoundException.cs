namespace DomainLayer.Exceptions
{
    public class SubCategoryNotFoundException : NotFoundException
    {
        public SubCategoryNotFoundException(int id)
            : base($"SubCategory with Id {id} was not found.") { }
    }
}