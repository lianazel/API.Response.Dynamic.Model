using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Domain.Models
{
    public class ApiDemandProperties
    {
        // > Id calculé par la base t <
        public long ID { get; set; }

        // > Id Enregistrement Construit (par Guid)  <
        public string ID_Properties { get; set; }

        // > Clé étrangère "ApiDemandRequest"  <
        public string Fk_ID_Demand { get; set; }

        // > Nom de la propriété  <
        public string PropertyName { get; set; }

        // > Type de la propriété  <
        public string PropertyType { get; set; } 

    }
}
