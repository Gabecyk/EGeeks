using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using StoreService.Infrastructure.Data;
using StoreService.Domain.Entities;
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
}