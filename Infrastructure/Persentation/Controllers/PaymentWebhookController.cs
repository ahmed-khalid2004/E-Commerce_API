using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstracion;

[ApiController]
[Route("api/payments/webhook")]
public class PaymentWebhookController : ApiBaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentWebhookController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // Stripe sends raw POST with Stripe-Signature header — no JWT, no auth
    [AllowAnonymous]
    [DisableRequestSizeLimit]
    [HttpPost]
    public async Task<IActionResult> StripeWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrEmpty(signature))
            return BadRequest("Missing Stripe-Signature header");

        await _paymentService.HandleWebhookEventAsync(json, signature);
        return Ok();
    }
}