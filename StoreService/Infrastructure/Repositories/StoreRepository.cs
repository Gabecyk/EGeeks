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

    public async Task UpdateAsync(Store store)
    {
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
    }

    public async Task<Store?> GetByIdAsync(Guid Id)
    {
        return await _context.Stores.FindAsync(Id);
    }

    public async Task<IEnumerable<Product>> GetProductsByStoreIdAsync(Guid storeId)
    {
        return await _context.Products.Where(p => p.StoreId == storeId).ToListAsync();
    }

    public async Task<Store?> GetByStoreIdAsync(Guid storeId)
    {
        return await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
    }

    public async Task<List<Store>> GetAllAsync()
    {
        return await _context.Stores.ToListAsync();
    }
}
