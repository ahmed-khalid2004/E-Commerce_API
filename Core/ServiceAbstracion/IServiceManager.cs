using DomainLayer.Contracts;
using ServiceAbstracion;

namespace ServicesAbstraction
{
    public interface IServiceManager
    {
        IProductService ProductService { get; }
        IBasketService BasketService { get; }
        IAuthenticationService AuthenticationService { get; }
        IOrderService OrderService { get; }
        IPaymentService PaymentService { get; }
        ICategoryService CategoryService { get; }
        IReviewService ReviewService { get; }
        ICustomerService CustomerService { get; }
        IDashboardService DashboardService { get; }
    }
}