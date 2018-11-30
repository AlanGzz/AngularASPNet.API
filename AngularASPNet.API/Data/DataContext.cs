using AngularASPNet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularASPNet.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options){ }
            
        public DbSet<Value> Values { get; set; }
    }
}