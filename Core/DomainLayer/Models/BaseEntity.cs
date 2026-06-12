namespace DomainLayer.Models
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;  // PK — always set by EF
    }
}