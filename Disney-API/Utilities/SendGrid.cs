using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Disney_API.Utilities
{
    public class SendGrid
    {
        public string EmailFrom { get; set; }
        public string UserEmailFrom { get; set; }
        public string Subject { get; set; }
        public string EmailTo { get; set; }
        public string UserEmailTo { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }

        private readonly SendGridClient client;
        public SendGrid(string apiKey,string EmailFrom, string UserEmailFrom, string UserEmailTo, string Subject, string EmailTo, string PlainTextContent, string HtmlContent)
        {
            this.EmailTo = EmailTo;
            this.EmailFrom = EmailFrom;
            this.UserEmailFrom = UserEmailFrom;
            this.UserEmailTo = UserEmailTo;
            this.Subject = Subject;
            this.PlainTextContent = PlainTextContent;
            this.HtmlContent = HtmlContent;

            client = new SendGridClient(apiKey);
        }

        public async Task<Response> SendEmail()
        {
            var msg = MailHelper.CreateSingleEmail(new EmailAddress(EmailFrom, UserEmailFrom), new EmailAddress(EmailTo, UserEmailTo), Subject, PlainTextContent, HtmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            return response;
        }

    }
}
