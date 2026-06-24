using DomainLayer.Contracts;
using ServiceAbstracion;
using ServicesAbstraction;

namespace Service
{
    public class ServiceManagerWithFactoryDelegate(
        Func<IProductService> ProductFactory,
        Func<IBasketService> BasketFactory,
        Func<IAuthenticationService> AuthenticationFactory,
        Func<IOrderService> OrderFactory,
        Func<IPaymentService> PaymentFactory,
        Func<ICategoryService> CategoryFactory,
        Func<IReviewService> ReviewFactory,
        Func<ICustomerService> CustomerFactory,
        Func<IDashboardService> DashboardFactory) : IServiceManager
    {
        public IProductService ProductService => ProductFactory.Invoke();
        public IBasketService BasketService => BasketFactory.Invoke();
        public IAuthenticationService AuthenticationService => AuthenticationFactory.Invoke();
        public IOrderService OrderService => OrderFactory.Invoke();
        public IPaymentService PaymentService => PaymentFactory.Invoke();
        public ICategoryService CategoryService => CategoryFactory.Invoke();
        public IReviewService ReviewService => ReviewFactory.Invoke();
        public ICustomerService CustomerService => CustomerFactory.Invoke();
        public IDashboardService DashboardService => DashboardFactory.Invoke();
    }
}