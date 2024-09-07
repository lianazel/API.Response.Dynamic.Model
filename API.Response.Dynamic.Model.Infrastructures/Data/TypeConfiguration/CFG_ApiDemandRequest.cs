using API.Response.Dynamic.Model.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Response.Dynamic.Model.Infrastructures.Data.TypeConfiguration
{
    /// <summary>
    ///  Configuration Table "ApiDemandRequest" ( Table des demandes de requêtes ) 
    ///    ( Une demande contient le nom d'un modèle de données )
    /// </summary>
    public class CFG_ApiDemandRequest : IEntityTypeConfiguration<ApiDemandRequest>
    {
        public void Configure(EntityTypeBuilder<ApiDemandRequest> b)
        {
            // > Ici, on indique ici que le nom de la table sera bien
            //  ... "ApiDemandRequest"
            b.ToTable("ApiDemandRequest");

            // > On définit une clé à la table "ApiDemandRequest" : <
            //  => On aura l'ID calculé par la BDD 
            b.HasKey(item => item.ID);

            //  => La clé sera l'ID du modèle calculée par le code <
            b.HasKey(item => item.ID_Demand);
        }
    }
}
