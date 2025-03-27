﻿namespace ManagementBot.Configuration
{
    public class SmtpOptions
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public string MailUserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
