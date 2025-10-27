using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.BasketModule;
using DomainLayer.Models.OrderModule;
using DomainLayer.Models.ProductModule;
using Service.Specifications.OrderModuleSpecifications;
using ServiceAbstracion;
using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OrderService(IMapper _mapper, IBaseketRepository _baseketRepository, IUnitOfWork _unitOfWork) : IOrderService
    {
        public async Task<OrderToReturnDTO> CreateOrder(OrderDTO orderDto, string Email)
        {
            var OrderAddress = _mapper.Map<AddressDTO, OrderAddress>(orderDto.shipToAddress);

            var Basket = await _baseketRepository.GetBasketASync(orderDto.BasketId)
                ?? throw new BasketNotFoundException(orderDto.BasketId);

            List<OrderItem> OrderItems = [];
            var ProductRepo = _unitOfWork.GetRepository<Product, int>();
            foreach (var item in Basket.Items)
            {
                var Product = await ProductRepo.GetByIdAsync(item.Id)
                    ?? throw new ProductNotFoundException(item.Id);
                OrderItems.Add(CreateOrderItem(item, Product));

            }

            var DeliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(orderDto.DeliveryMethodId)
                ?? throw new DeliveryMethodNotFoundException(orderDto.DeliveryMethodId);

            var SubTotal = OrderItems.Sum(I => I.Price * I.Quantity);

            var Order = new Order(Email,OrderAddress,DeliveryMethod,SubTotal,OrderItems);
            await _unitOfWork.GetRepository<Order, Guid>().AddAsync(Order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<Order, OrderToReturnDTO>(Order);
        }

        private static OrderItem CreateOrderItem(BasketItem item, Product Product)
        {
            return new OrderItem()
            {
                Product = new ProductItemOrdered() { ProductId = Product.Id, PictureUrl = Product.PictureUrl, ProductName = Product.Name },
                Price = Product.Price,
                Quantity = item.Quantity
            };
        }

        public async Task<IEnumerable<DeliveryMethodDTO>> GetDeliveryMethodsAsync()
        {
            var DelivaryMethods = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryMethod>,IEnumerable<DeliveryMethodDTO>>(DelivaryMethods);
        }

        public async Task<IEnumerable<OrderToReturnDTO>> GetAllOrdersAsync(string Email)
        {
            var Spec = new OrderSpecifications(Email);
            var Orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(Spec);
            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderToReturnDTO>>(Orders);
        }

        public async Task<OrderToReturnDTO> GetOrderByIdAsync(Guid Id)
        {
            var Spec = new OrderSpecifications(Id);
            var Order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(Spec);
            return _mapper.Map<Order,OrderToReturnDTO>(Order);
        }
    }
}
