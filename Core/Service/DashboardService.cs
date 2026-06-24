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
            var ordersTask = unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications());
            var customersTask = userManager.Users.CountAsync();

            await Task.WhenAll(productsTask, ordersTask, customersTask);

            var orders = ordersTask.Result.ToList();
            var totalRevenue = orders
                .Where(o => SuccessfulStatuses.Contains(o.Status))
                .Sum(o => o.GetTotal());

            return new DashboardSummaryDTO
            {
                TotalProducts = productsTask.Result.Count(),
                TotalOrders = orders.Count,
                TotalCustomers = customersTask.Result,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<IReadOnlyList<RecentOrderDTO>> GetRecentOrdersAsync(int limit)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications());

            return orders
                .Take(limit)
                .Select(o => new RecentOrderDTO
                {
                    Id = o.Id,
                    BuyerEmail = o.BuyerEmail,
                    OrderDate = o.OrderDate,
                    Total = o.GetTotal(),
                    Status = o.Status.ToString()
                })
                .ToList();
        }

        public async Task<IReadOnlyList<TopProductDTO>> GetTopProductsAsync(int limit)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications());

            return orders
                .Where(o => SuccessfulStatuses.Contains(o.Status))
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Product.ProductId)
                .Select(g => new TopProductDTO
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.ProductName,
                    PictureUrl = g.First().Product.PictureUrl,
                    TotalQuantitySold = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(limit)
                .ToList();
        }
    }
}