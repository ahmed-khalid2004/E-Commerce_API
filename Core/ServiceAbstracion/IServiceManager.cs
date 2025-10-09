using DomainLayer.Contracts;
using ServiceAbstracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstraction
{
    public interface IServiceManager
    {
        public IProductService ProductService { get; }

        public IBasketService BasketService { get; }

        public IAuthenticationService AuthenticationService { get; }

    }
}