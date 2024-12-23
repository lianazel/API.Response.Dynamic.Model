﻿using API.Response.Dynamic.Model.Framework.Interfaces;
using API.Response.Dynamic.Model.Infrastructures.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace API.Response.Dynamic.Model.MailingMethods

{
    // > Utilisation de MailKit , voir : <
    // https://jasonwatmore.com/post/2022/03/11/net-6-send-an-email-via-smtp-with-mailkit
    // https://mailtrap.io/blog/asp-net-core-send-email/
    // https://ironpdf.com/fr/blog/net-help/mailkit-csharp-guide/

    // > Je me suis crée un compte "Ethéré" pour les tests <
    // https://ethereal.email/create


    /// <summary>
    /// Rappel : Travail avec le NuGet "MailKit" 
    /// </summary>
    public  class EmailService : IEmailService
    {
        #region properties
        // > Message a envoyer <
        private readonly string _message;

        // > Expediteur <
        private readonly string _emailFrom;

        // > Destinataire w
        private readonly string _emailTo;

        // > Cré une variable "args" de type tableau de string nullable (?) <
        // ( pour " var builder = WebApplication.CreateBuilder(args) )
        // ( Remarque : on peut l'appeler autrement que "args" si on veut )
        private static string[]? args { get; set; }

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
        /// <summary>
        /// Method "Send" ==> send mail 
        /// </summary>
        /// <param name="AppSettingsSection">Destinataire</param>
        /// <param name="to">Destinataire</param>
        /// <param name="subject">Objet du mail</param>
        /// <param name="html"></param>
        /// <param name="from">Emetteur</param>
        public Task<string> Send(string AppSettingsSection,string to, string subject, string html, string from = null)
        {
            // > Par défaut, optimiste  <
            //   ( Pas de message )
            string ResMsge = string.Empty ;

            // > 1/ Création d'un "configurationBuilder" <
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // > 2/ Création d'un objet "Configuration" à partir du...
            //      ..."configurationBuilder" <
            IConfiguration configuration = configurationBuilder.Build();


            // > 3/ On déclare une variable "builder" de type...
            //      ... "WebAplicationBuilder" <
            var builder = WebApplication.CreateBuilder(args);


            // - - - - - - - - - - -
            // > Récupération des paramètres depuis l'appSettings <
            // - - - - - - - - - - -
            //   [ Nouvelle méthode pour récupérer les infos par Binding ] 
            EmailSmtpSend SmtpSend = new EmailSmtpSend();

            // > On récupère les paramètres d'une boite Mail Fictive pour les tests <
            //  ( Problème : les systèmes de messagerie (Gmail, m.com,etc...) bloque les connections...
            //    ...faites à partir de code ) ==> du coup, difficile à tester 
            builder.Configuration.GetSection(AppSettingsSection).Bind(SmtpSend);

            // - - - - - - - - - - - -
            // > Create Message <
            // - - - - - - - - - - - -
            var email = new MimeMessage();

            if (from != null)
            {
                email.From.Add(MailboxAddress.Parse(from));
            }
            else
            {
                email.From.Add(MailboxAddress.Parse(SmtpSend.smtpAdressMail));
            }

            email.To.Add(MailboxAddress.Parse(to));

            // > Sujet du mail <
            email.Subject = subject;

            // > Corps du mail <
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // - - - - - - - - - - - -
            // > Send Message <
            // - - - - - - - - - - - -
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(SmtpSend.smptHost, SmtpSend.smtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(SmtpSend.smtpAdressMail, SmtpSend.smtppassword);
                smtp.Send(email);
                smtp.Disconnect(SmtpSend.smtpDisconnect);
            }

            // > Anomalie détectée => Interception message <
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("JWT_ERR_SENDMAIL_A1 - ");
                sb.Append(ex.Message);
                ResMsge = sb.ToString();
            }

            // > On renvoie le message resultat <
            //   ( si vide => tout est OK )
            return Task.FromResult(ResMsge); 
        }
    }
}
