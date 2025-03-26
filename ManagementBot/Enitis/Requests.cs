using ManagementBot.Helpers;

namespace ManagementBot.Enitis
{
    public class Requests : Auditable
    {
        public string? CompanyName { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Department { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }
        public string? ProjectBudget { get; set; }
        public string? InquirySource { get; set; }
        public string? AdditionalInformation { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
    }
}
