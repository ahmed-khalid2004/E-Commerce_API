using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service;
using ServiceAbstracion;
using ServicesAbstraction;
using StackExchange.Redis;

namespace Services
{
    public class ServiceManager(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBaseketRepository baseketRepository,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<PaymentService> logger,
    IEmailService emailService,
    IRedisClient redisClient)
    : IServiceManager

    {
        private readonly Lazy<IProductService> _lazyProductService = new(() => new ProductService(unitOfWork, mapper));
        private readonly Lazy<IBasketService> _lazyBasketService = new(() => new BasketService(baseketRepository, mapper));
        private readonly Lazy<IAuthenticationService> _lazyAuthenticationService = new(() => new AuthenticationService(userManager, configuration, mapper, emailService, redisClient));
        private readonly Lazy<IOrderService> _lazyOrderService = new(() => new OrderService(mapper, baseketRepository, unitOfWork));
        private readonly Lazy<IPaymentService> _lazyPaymentService = new(() => new PaymentService(
        configuration,
        baseketRepository,
        unitOfWork,
        mapper,
        logger));
        private readonly Lazy<ICategoryService> _lazyCategoryService = new(() => new CategoryService(unitOfWork, mapper));
        private readonly Lazy<IEmailService> _LazyEmailService = new(() => new EmailService(configuration));
        private readonly Lazy<IReviewService> _lazyReviewService = new(() => new ReviewService(unitOfWork, mapper));
        private readonly Lazy<ICustomerService> _lazyCustomerService = new(() => new CustomerService(userManager, unitOfWork, mapper));
        private readonly Lazy<IDashboardService> _lazyDashboardService = new(() => new DashboardService(unitOfWork, userManager));

        public IProductService ProductService => _lazyProductService.Value;
        public IBasketService BasketService => _lazyBasketService.Value;
        public IAuthenticationService AuthenticationService => _lazyAuthenticationService.Value;
        public IOrderService OrderService => _lazyOrderService.Value;
        public IPaymentService PaymentService => _lazyPaymentService.Value;
        public ICategoryService CategoryService => _lazyCategoryService.Value;
        public IEmailService EmailService => _LazyEmailService.Value;
        public IReviewService ReviewService => _lazyReviewService.Value;
        public ICustomerService CustomerService => _lazyCustomerService.Value;
        public IDashboardService DashboardService => _lazyDashboardService.Value;

    }
}