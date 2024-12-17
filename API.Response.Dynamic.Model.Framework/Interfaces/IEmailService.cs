

namespace API.Response.Dynamic.Model.Framework.Interfaces
{/// <summary>
///  > "AppSettingsSection" : Nom de la section appSettings <
///  > "to"                 : Adresse mail destinaire <
///  > "subject"            : Objet du mail <
///  > "html"               : envoi du mail au format html <
///  > "from"               : Adresse mail emetteur <
/// </summary>
    public interface IEmailService
    {
        Task<string> Send(string AppSettingsSection,string to, string subject, string html, string from = null);

    }
}
