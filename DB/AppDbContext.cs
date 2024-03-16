using HackWebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<requestToDB> Requests { get; set; }
}