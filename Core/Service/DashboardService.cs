using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using DomainLayer.Models.OrderModule;
using DomainLayer.Models.ProductModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Specifications.OrderModuleSpecifications;
using ServiceAbstracion;
using Shared.DataTransferObjects.DashboardDTOs;

namespace Service
{
    public class DashboardService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager) : IDashboardService
    {
        private static readonly OrderStatus[] SuccessfulStatuses =
        [
            OrderStatus.PaymentReceived,
            OrderStatus.Confirmed,
            OrderStatus.Shipped,
            OrderStatus.Delivered
        ];

        public async Task<DashboardSummaryDTO> GetSummaryAsync()
        {
            var productsTask = unitOfWork.GetRepository<Product, int>().GetAllAsync();
            var ordersTask   = unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications());
            var customersTask = userManager.Users.CountAsync();

            await Task.WhenAll(productsTask, ordersTask, customersTask);

            var orders = ordersTask.Result.ToList();

            var totalRevenue = orders
                .Where(o => SuccessfulStatuses.Contains(o.Status))
                .Sum(o => o.GetTotal());

            return new DashboardSummaryDTO
            {
                TotalProducts  = productsTask.Result.Count(),
                TotalOrders    = orders.Count,
                TotalCustomers = customersTask.Result,
                TotalRevenue   = totalRevenue
            };
        }
    }
}
