using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service;
using ServiceAbstracion;
using ServicesAbstraction;

namespace Services
{
    public class ServiceManager(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBaseketRepository baseketRepository,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<PaymentService> logger)
    : IServiceManager

    {
        private readonly Lazy<IProductService> _lazyProductService = new(() => new ProductService(unitOfWork, mapper));
        private readonly Lazy<IBasketService> _lazyBasketService = new(() => new BasketService(baseketRepository, mapper));
        private readonly Lazy<IAuthenticationService> _lazyAuthenticationService = new(() => new AuthenticationService(userManager, configuration, mapper));
        private readonly Lazy<IOrderService> _lazyOrderService = new(() => new OrderService(mapper, baseketRepository, unitOfWork));
        private readonly Lazy<IPaymentService> _lazyPaymentService = new(() => new PaymentService(
        configuration,
        baseketRepository,
        unitOfWork,
        mapper,
        logger));
        private readonly Lazy<ICategoryService> _lazyCategoryService = new(() => new CategoryService(unitOfWork, mapper));

        public IProductService ProductService => _lazyProductService.Value;
        public IBasketService BasketService => _lazyBasketService.Value;
        public IAuthenticationService AuthenticationService => _lazyAuthenticationService.Value;
        public IOrderService OrderService => _lazyOrderService.Value;
        public IPaymentService PaymentService => _lazyPaymentService.Value;
        public ICategoryService CategoryService => _lazyCategoryService.Value;
    }
}