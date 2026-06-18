using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.UseCases;
using StoreService.Application.DTOs;
using StoreService.Application.Ports;
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
        [FromServices] IAllProductsUseCase usecase)
    {
        var products = await usecase.Execute();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(
        Guid id,
        [FromServices] IGetProductByIdUseCase useCase)
    {
        var product = await useCase.Execute(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [Authorize(Roles = "Seller")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request,
        [FromServices] UpdateProductUseCase useCase)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await useCase.Execute(id, request, userId);
        return Ok();
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