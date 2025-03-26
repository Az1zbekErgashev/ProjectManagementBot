namespace ManagementBot.Helpers
{
    public class Auditable
    {
        public int Id { get; set; }
        public int IsDeleted { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
