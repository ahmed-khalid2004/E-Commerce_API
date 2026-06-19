namespace DomainLayer.Exceptions
{
    public class ReviewNotFoundException : NotFoundException
    {
        public ReviewNotFoundException(int id) : base($"Review with id: {id} is not found") { }
    }
}