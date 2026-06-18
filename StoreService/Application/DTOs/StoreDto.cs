namespace StoreService.Application.DTOs;

public record StoreDto
(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    string ProfileImageUrl,
    DateTime CreatedAt
);
