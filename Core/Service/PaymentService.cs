using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.OrderModule;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceAbstracion;
using Shared.DataTransferObjects.BasketModuleDTOs;
using Stripe;
using Product = DomainLayer.Models.ProductModule.Product;

namespace Service
{
    public class PaymentService(
        IConfiguration _configuration,
        IBaseketRepository _basketRepository,
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        ILogger<PaymentService> _logger) : IPaymentService
    {
        // Stripe event type string constants (avoids relying on the Events class,
        // whose namespace/availability differs across Stripe.net versions)
        private const string PaymentIntentSucceededType = "payment_intent.succeeded";
        private const string PaymentIntentPaymentFailedType = "payment_intent.payment_failed";

        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string userId, int deliveryMethodId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketASync(userId)
                ?? throw new BasketNotFoundException(userId);

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            foreach (var item in basket.Items)
            {
                var product = await productRepo.GetByIdAsync(item.Id)
                    ?? throw new ProductNotFoundException(item.Id);
                item.Price = product.Price;
            }

            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(deliveryMethodId)
                ?? throw new DeliveryMethodNotFoundException(deliveryMethodId);

            basket.deliveryMethodId = deliveryMethodId;
            basket.shippingPrice = deliveryMethod.Cost;

            var amount = (long)(basket.Items.Sum(i => i.Quantity * i.Price) + deliveryMethod.Cost) * 100;
            var intentService = new PaymentIntentService();

            if (basket.paymentIntentId is null)
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                var intent = await intentService.CreateAsync(createOptions);
                basket.paymentIntentId = intent.Id;
                basket.clientSecret = intent.ClientSecret;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions { Amount = amount };
                await intentService.UpdateAsync(basket.paymentIntentId, updateOptions);
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return _mapper.Map<BasketDTO>(basket);
        }

        public async Task HandleWebhookEventAsync(string json, string stripeSignatureHeader)
        {
            var webhookSecret = _configuration["StripeSettings:WebhookSecret"]!;

            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, webhookSecret);
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

            switch (stripeEvent.Type)
            {
                case PaymentIntentSucceededType:
                    {
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogInformation("PaymentIntent succeeded: {Id}", intent?.Id);
                        if (intent is not null)
                            await UpdateOrderStatusAsync(orderRepo, intent.Id, OrderStatus.PaymentReceived);
                        break;
                    }
                case PaymentIntentPaymentFailedType:
                    {
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogWarning("PaymentIntent failed: {Id}", intent?.Id);
                        if (intent is not null)
                            await UpdateOrderStatusAsync(orderRepo, intent.Id, OrderStatus.PaymentFailed);
                        break;
                    }
                default:
                    _logger.LogInformation("Unhandled Stripe event type: {Type}", stripeEvent.Type);
                    break;
            }
        }

        private async Task UpdateOrderStatusAsync(
            IGenericRepository<Order, Guid> orderRepo, string paymentIntentId, OrderStatus status)
        {
            var spec = new Service.Specifications.OrderWithPaymentIntentIdSpecifications(paymentIntentId);
            var order = await orderRepo.GetByIdAsync(spec);

            if (order is null)
            {
                _logger.LogWarning("No order found for PaymentIntentId: {Id}", paymentIntentId);
                return;
            }

            order.Status = status;
            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}