using DomainLayer.Contracts;
using ServiceAbstracion;
using ServicesAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManagerWithFactoryDelegate(Func<IProductService> ProductFacrtory,
       Func<IBasketService> BasketFactory , 
       Func<IAuthenticationService> AuthenticationFactory ,
       Func<IOrderService> OrderFactory) : IServiceManager
    {
        public IProductService  ProductService => ProductFacrtory.Invoke();

        public IBasketService  BasketService => BasketFactory.Invoke();

        public IAuthenticationService AuthenticationService => AuthenticationFactory.Invoke();

        public IOrderService OrderService => OrderFactory.Invoke();
    }
}
