using API.Response.Dynamic.Model.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Response.Dynamic.Model.Infrastructures.Data.TypeConfiguration
{

    /// <summary>
    ///  Configuration Table "ApiDemandSqlCode" ( Table des codes de requêtes SQL à exécuter ) 
    ///    ( Texte de la requête SQL à exécuter  )
    /// </summary>
    public class CFG_ApiDemandSqlCode : IEntityTypeConfiguration<ApiDemandSqlCode>
    {
        public void Configure(EntityTypeBuilder<ApiDemandSqlCode> b)
        {
            // > Ici, on indique ici que le nom de la table sera bien
            //  ... "AApiDemandSqlCode"
            b.ToTable("ApiDemandSqlCode");

            // > On définit une clé à la table "ApiDemandSqlCode" : <
            //  => On aura l'ID calculé par la BDD 
            b.HasKey(item => item.ID);

            //  => La clé sera l'ID du modèle calculée par le code <
            b.HasKey(item => item.ID_SqlCode);
        }
    }
}
