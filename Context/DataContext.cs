using Microsoft.EntityFrameworkCore;
using UMA.Models;

namespace UMA.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
