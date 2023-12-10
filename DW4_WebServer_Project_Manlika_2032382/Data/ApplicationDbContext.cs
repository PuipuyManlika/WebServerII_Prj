using DW4_WebServer_Project_Manlika_2032382.Model;
using Microsoft.EntityFrameworkCore;

namespace DW4_WebServer_Project_Manlika_2032382.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Model.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
