

namespace API.Response.Dynamic.Model.Framework.Interfaces
{
    public interface IEmailService
    {
        Task<string> Send(string to, string subject, string html, string from = null);

    }
}
