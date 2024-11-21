
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Response.Dynamic.Model.Domain.Models
{
    public class ApiDemandRequest
    {
        // > Id Calculé par la base  <
        public long ID { get; set; } 

        // > Id Construit (Guid)  <
        public string ID_Demand { get; set; }

        // > Type de base de données <
        public string BDDType { get; set; }

        // > Nom du modèle construit <
        public string DataModelName { get; set; }

        // > Nombre de propriétés <
        public int DataModelNumberProperties { get; set; }

        // --- On renvoie par JSON la liste des propriétes --- 
        // [NotMapped] ==> La propriété n'est pas envoyée en base de données 
       [NotMapped]
       public List<ApiDemandProperties>? properties { get; set; }

    }
}
