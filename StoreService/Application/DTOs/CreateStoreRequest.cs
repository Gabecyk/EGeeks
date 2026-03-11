namespace StoreService.Application.DTOs;

public record CreateStoreRequest(
    string Name,
    string Description,
    string ProfileImageUrl
);