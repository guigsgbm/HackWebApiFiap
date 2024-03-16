using HackWebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Repository;

public class ReqRepository : IRepository<requestToDB>
{
    private readonly AppDbContext _context;
    public ReqRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<requestToDB?> Add(requestToDB entity)
    {
        var result = await _context.Requests.AddAsync(entity);
        return result.Entity;
    }

    public async Task<requestToDB?> DeleteById(int id)
    {
        var item = await _context.Requests.FirstOrDefaultAsync(x => x.Id == id);

        if (item != null)
        {
            var result = _context.Requests.Remove(item);
            return result.Entity;
        }

        return null;
    }

    public async Task<IEnumerable<requestToDB?>> GetAll(int skip, int take)
    {
        return await _context.Requests
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<requestToDB?> GetById(int id)
    {
        return await _context.Requests.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

}