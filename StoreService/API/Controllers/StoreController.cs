using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Application.UseCases;

namespace StoreService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
   [Authorize(Roles = "Seller")]
   [HttpPost]
   public async Task<IActionResult> Create(CreateStoreRequest request, [FromServices] CreateStoreUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(request, userId);
        return Ok();
   }

   [Authorize(Roles = "Seller")]
   [HttpPut]
   public async Task<IActionResult> Update(UpdateStoreRequest request, [FromServices] UpdateStoreUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(request, userId);
        return Ok();
   }

   [Authorize(Roles = "Seller")]
   [HttpGet]
   public async Task<IActionResult> GetStoreById([FromServices] GetStoreByIdUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var store = await useCase.Execute(userId);
        if (store == null)
            return NotFound();

        return Ok(store);
   }

   [Authorize(Roles = "Customer")]
   [HttpGet("{storeId}/products")]
   public async Task<IActionResult> GetProductsByStoreId(Guid storeId, [FromServices] IGetProductsByStoreIdUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var products = await useCase.Execute(storeId);
        return Ok(products);
   }

   [Authorize(Roles = "Customer")]
   [HttpGet("{storeId}/StoreForCustomer")]
   public async Task<IActionResult> GetStoreByIdForCustomer(Guid storeId, [FromServices] GetStoreByStoreIdForCustomerUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var store = await useCase.Execute(storeId);
        if (store == null)
            return NotFound();

        return Ok(store);
   }

   [HttpGet("stores")]
   public async Task<IActionResult> GetAllStores([FromServices] IGetAllStoresUseCase useCase)
   {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var stores = await useCase.Execute();
        return Ok(stores);
   }
}