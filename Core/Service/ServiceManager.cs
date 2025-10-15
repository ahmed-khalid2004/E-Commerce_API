using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service;
using ServiceAbstracion;
using ServicesAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, IBaseketRepository baseketRepository, UserManager<ApplicationUser> userManager, IConfiguration _configuration) : IServiceManager
    {
        private readonly Lazy<IProductService> _lazyProductService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));
        private readonly Lazy<IBasketService> _lazyBasketService = new Lazy<IBasketService>(() => new BasketService(baseketRepository, mapper));
        private readonly Lazy<IAuthenticationService> _lazyAuthenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager,_configuration,mapper));
        private readonly Lazy<IOrderService> _lazyOrderService = new Lazy<IOrderService>(() => new OrderService(mapper, baseketRepository, unitOfWork));
        public IProductService ProductService => _lazyProductService.Value;
        public IBasketService BasketService => _lazyBasketService.Value;
        public IAuthenticationService AuthenticationService => _lazyAuthenticationService.Value;

        public IOrderService OrderService => _lazyOrderService.Value;
    }
}