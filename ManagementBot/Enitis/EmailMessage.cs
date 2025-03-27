using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ManagementBot.Enitis
{
    public class EmailMessage
    {
        [Required] public string To { get; set; } = string.Empty;

        [Required] public string Subject { get; set; } = string.Empty;

        [Required] public string Body { get; set; }

        public List<Attachment> Attachments { get; set; }


        public static EmailMessage SuccessSendRequest(string email, string fullname)
        {
            return new EmailMessage()
            {
                To = email,
                Subject = "Your request has been successfully submitted",
                Body = @$"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                background: #ffffff;
                                margin: 20px auto;
                                padding: 20px;
                                border-radius: 8px;
                                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                            }}
                            h2 {{
                                color: #333;
                            }}
                            p {{
                                font-size: 16px;
                                color: #555;
                                line-height: 1.5;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 14px;
                                color: #888;
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2>Your request has been successfully submitted!</h2>
                            <p>Hello, {fullname}</p>
                            <p>We have received your request and started processing it. Our specialists will get in touch with you shortly.</p>
                            <p>Thank you for reaching out!</p>
                            <div class='footer'>
                                <p>&copy; {DateTime.UtcNow.Year} CRM System | All rights reserved</p>
                            </div>
                        </div>
                    </body>
                    </html>
                "
            };
        }
    }
}
