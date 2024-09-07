using API.Response.Dynamic.Model.Domain.Models;

// > pour "IEntityTypeConfiguration" <
using Microsoft.EntityFrameworkCore;

// > Pour "EntityTypeBuilder" 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Response.Dynamic.Model.Infrastructures.Data.TypeConfiguration
{
    /// <summary>
    ///   Liste des des propriétés du modèle de données généré par le système
    ///   ( Lorsqu'un modèle est construit, on le mémorise en base => ...
    ///      gain de temps pour la prochaine demande d'exécution de la requête )
    /// </summary>
    public class CFG_ApiDemandProperties : IEntityTypeConfiguration<ApiDemandProperties>
    {
        public void Configure(EntityTypeBuilder<ApiDemandProperties> b )

        {
            // > Ici, on indique ici que le nom de la table sera bien
            //  ... "ApiDemandProperties"
            b.ToTable("ApiDemandProperties");

            // > On définit les clé à la table "ApiDemandProperties" : <
            //  => On aura l'ID calculé par la BDD 
            b.HasKey(item => item.ID);
            //  => La clé sera l'ID de la propriété calculée par le code 
            b.HasKey(item => item.ID_Properties);
            
            //  => La clé sera l'ID correspondant à la clé étrangère de la demande...
            //     ...=> Table "ApiDemandRequest". Il s'agit des propriétés rattachées...
            //     ...   à une demande de requête.
            //b.HasKey(item => item.Fk_ID_Demand);

            // > On définit la relation manuellement  :
            //   => Notre enregistrement d'une propriétés concnerne..
            //   ... qu'une seule demande de requête (".HasOne...")

            //   => Et ensuite, cette  demande de requête  peut...
            //    avoir 0 ou N enregistrements pour la description...
            //     ... de propriétés d'un modèle (".WithMany...)
            b.HasOne<ApiDemandRequest>();
            b.HasMany<ApiDemandProperties>();
        }    
    }
}
