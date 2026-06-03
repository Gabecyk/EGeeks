namespace StoreService.Application.DTOs;

public record UpdateStoreRequest(
    string Name,
    string Description,
    string ProfileImageUrl
);