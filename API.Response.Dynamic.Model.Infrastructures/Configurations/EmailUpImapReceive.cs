using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Infrastructures.Configurations
{
    /// <summary>
    ///  MailKit : Setting Up IMAP Servers for Receving mails
    /// </summary>
    public class EmailUpImapReceive
    { 
        // > Exemple : "imap.gmail.com" <    
        public string Imap {  get; set; }
        // > Exemple : 993
        public int ImapPort { get; set; }

        public string ImapMailAdress { get; set; }
        public string ImapMailPassword { get; set; }

        public bool ImapConnect { get; set; }

        public bool ImapDisConnect { get; set; }

    }
}
