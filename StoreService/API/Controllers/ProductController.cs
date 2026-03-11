using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.UseCases;
using StoreService.Application.DTOs;
using System.Security.Claims;

namespace StoreService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [Authorize(Roles = "Seller")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request, 
        [FromServices] CreateProductUseCase useCase)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(request, userId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] GetAllProductsUseCase usecase)
    {
        var products = await usecase.Execute();
        return Ok(products);
    }

    [Authorize(Roles = "Seller")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] DeleteProductUseCase useCase)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(id, userId);
        return Ok();
    }
}