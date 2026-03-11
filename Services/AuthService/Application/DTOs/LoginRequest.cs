using System.ComponentModel.DataAnnotations;

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password
);