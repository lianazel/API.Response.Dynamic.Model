using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Framework.Repositories
{
    public interface IEmailService
    {
        void send(string to, string subject, string html, string from = null);

    }
}
