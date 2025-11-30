using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.OrderModule;
using Microsoft.Extensions.Configuration;
using ServiceAbstracion;
using Shared.DataTransferObjects.BasketModuleDTOs;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product =  DomainLayer.Models.ProductModule.Product;

namespace Service
{
    public class PaymentService(IConfiguration _configuration,
        IBaseketRepository _basketRepository,
        IUnitOfWork _unitOfWork,
        IMapper _mapper) : IPaymentService
    {
        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
           StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

              var Basket = await  _basketRepository.GetBasketASync(basketId)
                 ?? throw new BasketNotFoundException(basketId);

            var ProductRepo = _unitOfWork.GetRepository<Product,int>();

            foreach (var item in Basket.Items)
            {
                var Product = await ProductRepo.GetByIdAsync(item.Id) 
                    ?? throw new ProductNotFoundException(item.Id);
                    item.Price = Product.Price;
            }

            ArgumentNullException.ThrowIfNull(Basket.deliveryMethodId);
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod,int>()
                .GetByIdAsync(Basket.deliveryMethodId.Value) ?? throw new DeliveryMethodNotFoundException(Basket.deliveryMethodId.Value);
            Basket.shippingPrice = deliveryMethod.Cost;

            var BasketAmount = (long) (Basket.Items.Sum(i => i.Quantity * i.Price * 100) + deliveryMethod.Cost) * 100;
            var PaymentService = new PaymentIntentService();
            if (Basket.paymentIntentId is null)
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = BasketAmount,
                    Currency = "USD",
                    PaymentMethodTypes = ["card"]
                };
                var PaymentIntent = await PaymentService.CreateAsync(options);
                Basket.paymentIntentId = PaymentIntent.Id;
                Basket.clientSecret = PaymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = BasketAmount
                };
                var service = new PaymentIntentService();
                await PaymentService.UpdateAsync(Basket.paymentIntentId, options);
            }
            await _basketRepository.CreateOrUpdateBasketAsync(Basket);
            return _mapper.Map<BasketDTO>(Basket);
        }
    }
}
