using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomerService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public CustomerController(CustomerDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile([FromBody] PostProfile request, [FromServices] CreateCustomerUseCase useCase)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));     
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);

        await useCase.Execute(userId, name, email, request.Address);

        return Ok(new { Message = "Customer profile created successfully." });
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile([FromServices] GetProfileUseCase useCase)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { Message = "Invalid or missing user ID." });

        var customer = await useCase.Execute(userId);

        return Ok(customer);
    }
}