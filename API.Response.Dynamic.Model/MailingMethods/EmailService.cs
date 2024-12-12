using API.Response.Dynamic.Model.Framework.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

using MimeKit;
using MimeKit.Text;

namespace API.Response.Dynamic.Model.MailingMethods

{
    // > Utilisation de MailKit , voir : <
    // https://jasonwatmore.com/post/2022/03/11/net-6-send-an-email-via-smtp-with-mailkit
    // https://mailtrap.io/blog/asp-net-core-send-email/
    // https://ironpdf.com/fr/blog/net-help/mailkit-csharp-guide/





    public class EmailService : IEmailService
    {
        #region properties
        // > Message a envoyer <
        private readonly string _message;

        // > Expediteur <
        private readonly string _emailFrom;

        // > Destinataire w
        private readonly string _emailTo;
        #endregion

        #region constructor
        public EmailService(string message, string emailFrom, string emailTo)
        {
            // > Réception des paramètres <
            _message = message;
            _emailFrom = emailFrom;
            _emailTo = emailTo;
        }

        public EmailService()
        { }
        #endregion

        public void  send(string to, string subject, string html, string from = null)
        {
            // > Create Message <
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));

            // > Sujet du mail <
            email.Subject = subject;

            // > Corps du mail <
            email.Body = new TextPart(TextFormat.Html) { Text = html };


            using var smtp = new SmtpClient();

        }
    }
}
