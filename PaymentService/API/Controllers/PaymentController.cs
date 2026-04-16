using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.UseCases;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost("{paymentId:guid}/confirm")]
    public async Task<IActionResult> Confirm(
        Guid paymentId,
        [FromServices] ConfirmPaymentUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(paymentId, cancellationToken);
        return Ok("Payment confirmed successfully.");
    }

    [HttpPost("{paymentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid paymentId,
        [FromServices] CancelPaymentUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(paymentId, cancellationToken);
        return Ok("Payment canceled successfully.");
    }
}