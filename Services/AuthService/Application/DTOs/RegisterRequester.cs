using System.ComponentModel.DataAnnotations;

public record RegisterRequest(
    [Required] [StringLength(100, MinimumLength = 3)] string Name,
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password,
    [Required] string Role
);