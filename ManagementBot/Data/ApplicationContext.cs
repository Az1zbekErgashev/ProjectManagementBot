using ManagementBot.Enitis;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace ManagementBot.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Requests> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
           .HasMany(u => u.Requests)
           .WithOne(h => h.User)
           .HasForeignKey(x => x.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
