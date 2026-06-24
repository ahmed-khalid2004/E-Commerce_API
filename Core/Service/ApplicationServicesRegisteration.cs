using DomainLayer.Contracts;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstracion;
using Services;
using ServicesAbstraction;

namespace Service
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddAutoMapper(typeof(Service.AssemblyReference).Assembly);
            Services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();

            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<Func<IProductService>>(Provider =>
            () => Provider.GetRequiredService<IProductService>());

            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<Func<IOrderService>>(Provider =>
            () => Provider.GetRequiredService<IOrderService>());

            Services.AddScoped<IBasketService, BasketService>();
            Services.AddScoped<Func<IBasketService>>(Provider =>
            () => Provider.GetRequiredService<IBasketService>());

            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<Func<IAuthenticationService>>(Provider =>
            () => Provider.GetRequiredService<IAuthenticationService>());

            Services.AddScoped<ICacheService, CacheService>();
            Services.AddScoped<Func<ICacheService>>(Provider =>
            () => Provider.GetRequiredService<ICacheService>());

            Services.AddScoped<IPaymentService, PaymentService>();
            Services.AddScoped<Func<IPaymentService>>(Provider =>
            () => Provider.GetRequiredService<IPaymentService>());

            // Category
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<Func<ICategoryService>>(Provider =>
            () => Provider.GetRequiredService<ICategoryService>());

            Services.AddScoped<IEmailService, EmailService>();
            Services.AddScoped<Func<IEmailService>>(Provider =>
            () => Provider.GetRequiredService<IEmailService>());

            Services.AddScoped<IReviewService, ReviewService>();
            Services.AddScoped<Func<IReviewService>>(Provider =>
                () => Provider.GetRequiredService<IReviewService>());

            Services.AddScoped<ICustomerService, CustomerService>();
            Services.AddScoped<Func<ICustomerService>>(Provider =>
                () => Provider.GetRequiredService<ICustomerService>());

            Services.AddScoped<IDashboardService, DashboardService>();
            Services.AddScoped<Func<IDashboardService>>(Provider =>
                () => Provider.GetRequiredService<IDashboardService>());

            return Services;
        }
    }
}