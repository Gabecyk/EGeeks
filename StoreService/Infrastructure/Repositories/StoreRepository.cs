namespace StoreService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;
using StoreService.Infrastructure.Data;

public class StoreRepository : IStoreRepository
{
    private readonly StoreDbContext _context;

    public StoreRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Store store)
    {
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
    }

    public async Task<Store?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
