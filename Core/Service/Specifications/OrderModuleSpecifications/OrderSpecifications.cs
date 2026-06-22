using DomainLayer.Models.OrderModule;
using System.Linq.Expressions;

namespace Service.Specifications.OrderModuleSpecifications
{
    class OrderSpecifications : BaseSpecifications<Order, Guid>
    {
        // All orders — Admin dashboard (no email filter)
        public OrderSpecifications() : base(null)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
            AddOrderByDescending(O => O.OrderDate);
        }

        public OrderSpecifications(string Email) : base(O => O.BuyerEmail == Email)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
            AddOrderByDescending(O => O.OrderDate);
        }

        public OrderSpecifications(Guid id) : base(O => O.Id == id)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
        }
    }
}
