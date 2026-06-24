using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.IdentityModule;
using DomainLayer.Models.OrderModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Specifications.OrderModuleSpecifications;
using ServiceAbstracion;
using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;

namespace Service
{
    public class CustomerService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper) : ICustomerService
    {
        private static readonly OrderStatus[] SuccessfulStatuses =
        [
            OrderStatus.PaymentReceived,
            OrderStatus.Confirmed,
            OrderStatus.Shipped,
            OrderStatus.Delivered
        ];

        public async Task<IReadOnlyList<CustomerDTO>> GetAllCustomersAsync()
        {
            var users = await userManager.Users.ToListAsync();
            return mapper.Map<IReadOnlyList<CustomerDTO>>(users);
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);
            return mapper.Map<CustomerDTO>(user);
        }

        public async Task<IReadOnlyList<OrderToReturnDTO>> GetCustomerOrdersAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            var spec = new OrderSpecifications(user.Email!);
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);
            return mapper.Map<IReadOnlyList<OrderToReturnDTO>>(orders);
        }

        public async Task<CustomerStatsDTO> GetCustomerStatsAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            var spec = new OrderSpecifications(user.Email!);
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);

            var successfulTotal = orders
                .Where(o => SuccessfulStatuses.Contains(o.Status))
                .Sum(o => o.GetTotal());

            return new CustomerStatsDTO
            {
                TotalOrders = orders.Count(),
                TotalSpend = successfulTotal
            };
        }

        public async Task DeleteCustomerAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());
        }
    }
}