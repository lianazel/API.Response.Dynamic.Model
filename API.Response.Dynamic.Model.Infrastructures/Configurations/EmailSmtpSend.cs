using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Infrastructures.Configurations
{
    /// <summary>
    ///  MailKit : Setting SMPTP Servers for sending mails
    /// </summary>
    public class EmailSmtpSend
    {
        // > Exemple : "smtp.gmail.com" <
        public string smptHost {  get; set; }   
        public bool smtpDefaultCredentials { get; set; }

        // > Exemple : 587 <
        public int smtpPort { get; set; }   

        // Exemple : "JCC" 
        public string smtpName { get; set; }

        string smtpAdressMail { get; set; } 
        string smtppassword { get; set; }

        public bool smtpDisconnect { get; set; }



    }
}
