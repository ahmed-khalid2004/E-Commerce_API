using DomainLayer.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Services;
using ServicesAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddAutoMapper(typeof(Service.AssemblyReference).Assembly);
            Services.AddScoped<IServiceManager, ServiceManager>();
            Services.AddScoped<IProductService, ProductService>();
            return Services;
        }

    }
}
