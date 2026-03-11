using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, [FromServices] RegisterUseCase useCase)
    {
        await useCase.Execute(request);
        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, [FromServices] LoginUseCase useCase)
    {
        var token = await useCase.Execute(request);
        return Ok(token);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            email,
            role
        });
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("customer-area")]
    public IActionResult CustomerArea()
    {
        return Ok("Welcome to the customer area!");
    }

    [Authorize(Roles = "Seller")]
    [HttpGet("seller-area")]
    public IActionResult SellerArea()
    {
        return Ok("Welcome to the seller area!");
    }
}