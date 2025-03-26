using ManagementBot.Helpers;

namespace ManagementBot.Enitis
{
    public class Users : Auditable
    {
        public long ChatId { get; set; }
        public ICollection<Requests>? Requests { get; set; }
        public required string State { get; set; }
    }
}
