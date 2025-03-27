using ManagementBot.Enitis;
using System.Collections.Concurrent;

namespace TrustyTalents.Service.Services.Emails
{
    public class EmailInboxService : IEmailInboxService
    {
        private readonly ConcurrentQueue<EmailMessage> _messages = new();

        public void EnqueueEmail(EmailMessage message)
        {
            _messages.Enqueue(message);
        }

        public IEnumerable<EmailMessage> DequeueEmails(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_messages.TryDequeue(out var message))
                {
                    yield return message;
                }
                else
                {
                    yield break;
                }
            }
        }

        public int GetQueueCount()
        {
            return _messages.Count;
        }
    }
}
