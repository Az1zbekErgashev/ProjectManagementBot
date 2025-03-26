namespace ManagementBot.Commons
{
    public class TelegramTexts
    {
        public const string WelcomeMessage =
            "🎉 *Welcome to Wisestone!* 🎉\n\n" +
            "🏢 *Wisestone* is the leading company in software testing.\n\n" +
            "🌐 Learn more about us on our website:\n" +
            "[🌍 Wisestone Website](https://www.wisestone-t.com)\n\n" +
            "📌 To proceed and submit a request, please use the command:\n";

        public const string СontinueMessage =
            "✅ *You are already registered!* ✅\n\n" +
            "📩 To proceed with your request, please use the command:\n" +
            "If you need assistance, feel free to reach out! 🤝";

        public const string RequestFillingMessage =
            "📋 *You are about to fill out a request.*\n\n" +
            "📌 Please make sure you have all the necessary information.\n" +
            "Once completed, our specialists will review your request as soon as possible. 🕒\n\n" +
            "🔹 *Are you ready to begin?*\n\n";
        public const string CancelRequest =
            "🔴 Request submission canceled.\r\n\r\nIf you want to try again, simply send \"Send Request\".";
        public const string SendPhoneNumber = "📞 Please send your phone number.\r\nTap the button below to share your number or enter it manually";
        public const string SendEmail = "📧 Please provide your email address.\r\nEnter your business email for contact";
        public const string SendResponsiblePerson = "👤 Please provide the responsible person's name.\r\nEnter the full name of the contact person";
        public const string SendPosition = "👔 Please enter your position or job title.\r\nProvide your current job title or the position you are applying for";
        public const string SendCompanyName = "🏢 Please enter your company name.\r\nSend the name in a reply message";
        public const string SendProjectBudjet = "💰 Please specify the project budget.\r\nEnter the amount in any convenient format (e.g., \"1000 USD\")";
        public const string SendInquiryContent = "✉️ Please enter the inquiry content.\r\nDescribe your request or proposal";
        public const string SendInquirySource = "🌍 Please specify the inquiry source.\r\nHow did you hear about us? (e.g., website, advertisement, recommendation, etc.)";
        public const string SendAdditionInformation = "📝 Please provide additional information.\r\nWrite any important details that should be considered. If there is no additional information, simply send \"Submit\"";
        public const string RequestSentMessage =
        "✅ *Your request has been submitted and is being processed.*\n\n" +
        "🕒 Please wait while our specialists review your request.\n" +
        "📩 You will receive a notification as soon as the review is complete, and we will contact you if necessary.\n\n" +
        "Thank you for reaching out! 💼";

        public const string SendNewRequest = "🔄 To send a request again, press the \"📩 Send Request\" button";
    }
}
