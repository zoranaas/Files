using Files.Models;
using Microsoft.EntityFrameworkCore;

namespace Files.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }  
        public DbSet<FileClass> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.LAPTOP-3RPOIDL2\\SQLEXPRESS;Initial Catalog=Files;Integrated Security=True;MultipleActiveResultSets=True");
        }
    }
}
