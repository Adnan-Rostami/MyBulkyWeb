using BulkyWeb.Services.Payments;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
[ApiController]
//[Authorize]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromQuery] int orderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string redirectUrl = await _paymentService.CreatePayment(orderId, userId);

        return Ok(new { payUrl = redirectUrl });
    }
    // [AllowAnonymous]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify([FromQuery] string authority, string status)
    {

        string userId = "edcc7537-7a27-4bd0-8b6c-4efde11d0d96";
        if (status != "OK")
            return BadRequest("Payment cancelled by user");

        // var refId = await _paymentService.Verify(authority);
        //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var refId = await _paymentService.Verify(authority, userId);

        if (refId != null)
        {
            return Ok(new
            {
                message = "پرداخت موفق",
                refId = refId
            });
        }

        return BadRequest("Payment failed");
    }
}
