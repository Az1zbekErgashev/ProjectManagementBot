using ManagementBot.Enitis;
using Microsoft.EntityFrameworkCore;

namespace ManagementBot.Data
{
    public class CrmDBContext : DbContext
    {
        public CrmDBContext(DbContextOptions<CrmDBContext> options) : base(options) { }

        public DbSet<CrmRequest> Requests { get; set; }
    }
}
