

namespace API.Response.Dynamic.Model.Framework.Interfaces
{
    public interface IEmailService
    {
        Task<Boolean> Send(string to, string subject, string html, string from = null);

    }
}
