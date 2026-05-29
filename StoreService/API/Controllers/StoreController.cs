using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StoreService.Application.DTOs;

namespace StoreService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
   [Authorize(Roles = "Seller")]
   [HttpPost("Store")]
   public async Task<IActionResult> Create(CreateStoreRequest request, [FromServices] CreateStoreUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(request, userId);
        return Ok();
   }

    // ainda fazer o endpoint funcionar, falta o use case e o handler
   [Authorize(Roles = "Seller")]
   [HttpPut("Store")]
   public async Task<IActionResult> Update(UpdateStoreRequest request, [FromServices] UpdateStoreUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(request, userId);
        return Ok();
   }
}