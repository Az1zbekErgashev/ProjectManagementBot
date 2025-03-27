using ManagementBot.Enitis;

namespace TrustyTalents.Service.Services.Emails
{
    public interface IEmailInboxService
    {
        void EnqueueEmail(EmailMessage message);
        IEnumerable<EmailMessage> DequeueEmails(int count);
        int GetQueueCount();
    }
}
