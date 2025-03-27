using ManagementBot.Helpers;
using ProjectManagement.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagementBot.Enitis
{
    public class CrmRequest : Auditable
    {
        public string? InquiryType { get; set; } 
        public string? CompanyName { get; set; } 
        public string? Department { get; set; } 
        public string? ResponsiblePerson { get; set; } 
        public string? InquiryField { get; set; } 
        public string? ClientCompany { get; set; } 
        public string? ProjectDetails { get; set; } 
        public string? Client { get; set; } 
        public string? ContactNumber { get; set; } 
        public string? Email { get; set; } 
        public string? ProcessingStatus { get; set; } 
        public string? FinalResult { get; set; }
        public string? Notes { get; set; }
        public int RequestStatusId { get; set; }

        [Column(TypeName = "text")]
        public string? Date { get; set; }
        public DateTime? Deadline { get; set; }
        public string? ProjectBudget { get; set; }
        public string? InquirySource { get; set; }
        public string? AdditionalInformation { get; set; }
        public ProjectStatus Status { get; set; } = 0;
        public Priority Priority { get; set; } = Priority.Normal;
    }
}
