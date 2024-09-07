using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Domain.Models
{
    public class ApiDemandSqlCode
    {

        // > Id Calculé par la base  <
        public long ID { get; set; }

        // > Id Construit (Guid)  <
        public string ID_SqlCode { get; set; }

        // > Clé étrangère "ApiDemandRequest"  <
        public string Fk_ID_Demand { get; set; }

        // > Texte de la requête SQL <
        public string SqlCode { get; set; }

        // > Texte de clause Where  <
        //   ( Construite par le code ) 
        public string SqlWhereClause { get; set; }

    }
}
