using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.BasketModule;
using DomainLayer.Models.OrderModule;
using DomainLayer.Models.ProductModule;
using Service.Specifications;
using Service.Specifications.OrderModuleSpecifications;
using ServiceAbstracion;
using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;

namespace Service
{
    public class OrderService(
        IMapper _mapper,
        IBaseketRepository _basketRepository,
        IUnitOfWork _unitOfWork) : IOrderService
    {
        public async Task<OrderToReturnDTO> CreateOrder(OrderDTO orderDto, string email, string userId)
        {
            var orderAddress = _mapper.Map<AddressDTO, OrderAddress>(orderDto.shipToAddress);

            // basket is fetched using the authenticated user's id — never from client input
            var basket = await _basketRepository.GetBasketASync(userId)
                ?? throw new BasketNotFoundException(userId);

            ArgumentNullException.ThrowIfNullOrEmpty(basket.paymentIntentId);

            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var orderSpec = new OrderWithPaymentIntentIdSpecifications(basket.paymentIntentId);
            var existingOrder = await orderRepo.GetByIdAsync(orderSpec);
            if (existingOrder is not null)
                orderRepo.Remove(existingOrder);

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var product = await productRepo.GetByIdAsync(item.Id)
                    ?? throw new ProductNotFoundException(item.Id);

                if (product.StockQuantity < item.Quantity)
                    throw new OutOfStockException(product.Name, product.StockQuantity);

                product.StockQuantity -= item.Quantity;
                productRepo.Update(product);

                orderItems.Add(CreateOrderItem(item, product));
            }

            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(orderDto.DeliveryMethodId)
                ?? throw new DeliveryMethodNotFoundException(orderDto.DeliveryMethodId);

            var subtotal = orderItems.Sum(i => i.Price * i.Quantity);

            var order = new Order(email, orderAddress, deliveryMethod, subtotal, orderItems, basket.paymentIntentId);

            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<Order, OrderToReturnDTO>(order);
        }

        private static OrderItem CreateOrderItem(BasketItem item, Product product)
            => new OrderItem
            {
                Product = new ProductItemOrdered
                {
                    ProductId = product.Id,
                    PictureUrl = product.PictureUrl,
                    ProductName = product.Name
                },
                Price = product.Price,
                Quantity = item.Quantity
            };

        public async Task<IEnumerable<DeliveryMethodDTO>> GetDeliveryMethodsAsync()
        {
            var methods = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryMethod>, IEnumerable<DeliveryMethodDTO>>(methods);
        }

        public async Task<IEnumerable<OrderToReturnDTO>> GetAllOrdersAsync(string email)
        {
            var spec = new OrderSpecifications(email);
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);
            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderToReturnDTO>>(orders);
        }

        public async Task<OrderToReturnDTO> GetOrderByIdAsync(Guid id, string email)
        {
            var spec = new OrderSpecifications(id);
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec);

            // Ownership check — the order must belong to the requesting user.
            // Returning NotFound (not Forbidden) avoids leaking which IDs exist.
            if (order is null || order.BuyerEmail != email)
                throw new OrderNotFoundException(id);

            return _mapper.Map<Order, OrderToReturnDTO>(order);
        }

        // ── Admin ─────────────────────────────────────────────────────────────

        public async Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersForAdminAsync()
        {
            var spec = new OrderSpecifications();
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);
            return _mapper.Map<IReadOnlyList<OrderToReturnDTO>>(orders);
        }

        public async Task<OrderToReturnDTO> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var status))
                throw new BadRequestException([$"'{newStatus}' is not a valid order status."]);

            var spec = new OrderSpecifications(orderId);
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(spec)
                ?? throw new OrderNotFoundException(orderId);

            order.Status = status;
            _unitOfWork.GetRepository<Order, Guid>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<Order, OrderToReturnDTO>(order);
        }
    }
}