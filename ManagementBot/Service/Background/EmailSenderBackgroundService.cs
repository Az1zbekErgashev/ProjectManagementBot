using ManagementBot.Configuration;
using ManagementBot.Enitis;
using System.Net;
using System.Net.Mail;
using TrustyTalents.Service.Services.Emails;

namespace TrustyTalents.Service.Services.Background
{
    public class EmailSenderBackgroundService : BackgroundService
    {
        private readonly IEmailInboxService _inboxService;
        private readonly int _batchSize;
        private readonly SmtpOptions _smtpOptions;

        public EmailSenderBackgroundService(IEmailInboxService inboxService, IConfiguration configuration)
        {
            _inboxService = inboxService;
            _batchSize = configuration.GetValue<int>("BatchSize");

            _smtpOptions = new SmtpOptions();
            configuration.GetSection("EmailSettings").Bind(_smtpOptions);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int queueCount = _inboxService.GetQueueCount();

                if (queueCount > 0)
                {
                    var messages = _inboxService.DequeueEmails(_batchSize).ToList();
                    await SendEmailsAsync(messages, stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private async Task SendEmailsAsync(List<EmailMessage> messages, CancellationToken stoppingToken)
        {
            using var client = new SmtpClient(_smtpOptions.SmtpServer, _smtpOptions.SmtpPort)
            {
                UseDefaultCredentials = true,
                EnableSsl = _smtpOptions.EnableSsl,
                Credentials = null
            };

            await Parallel.ForEachAsync(messages, stoppingToken, async (message, token) =>
            {
                try
                {
                    var smtpMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpOptions.SenderEmail, _smtpOptions.SenderName),
                        Subject = message.Subject,
                        Body = message.Body,
                        IsBodyHtml = true
                    };

                    smtpMessage.To.Add(message.To);

                    if (message.Attachments != null)
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            smtpMessage.Attachments.Add(attachment);
                        }
                    }

                    await client.SendMailAsync(smtpMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке email: {ex.Message}");
                }
            });
        }
    }
}
