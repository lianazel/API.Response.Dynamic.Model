using API.Response.Dynamic.Model.Domain.Models;
using API.Response.Dynamic.Model.Framework.UnitOfWork;
using API.Response.Dynamic.Model.Infrastructures.Data.TypeConfiguration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace API.Response.Dynamic.Model.Infrastructures.Data
{
    public class DataContext : DbContext, IUnitOfWork
    {
        // > Constructeur (1) <
        public DataContext() : base()
        {

        }

        // > Constructeur (2) <
        public DataContext([NotNullAttribute] DbContextOptions options) : base(options)         
        {         
        }

        // #####=-=-=-=-=-=-=-=-=-=-=-=-=-=
        // ##### -- Installe les configurations pour les tables 
        // #####=-=-=-=-=-=-=-=-=-=-=-=-=-=
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // > On déclare la classe de configuration "CFG_ApiDemandRequest" pour la table "ApiDemandRequest"...
            //    ...(Demandes d'exécution de requêtes) <
            modelBuilder.ApplyConfiguration(new CFG_ApiDemandRequest());

            // > On déclare la classe de configuration "CFG_ApiDemandProperties" pour la table "ApiDemandProperties"...
            //    ...( Description du modèle qui a été généré lors de la demande d'exécution ) <
             modelBuilder.ApplyConfiguration(new CFG_ApiDemandProperties());

            // > On déclare la classe de configuration "CFG_ApiDemandSqlCode" pour la table "ApiDemandSqlCode"...
            //    ...( Code de la requête SQL à exécuter  ) <
            modelBuilder.ApplyConfiguration(new CFG_ApiDemandSqlCode());
        }

        // > Table "ApiDemandRequest" (Demande d'exécution de requête SQL)  <
        //   ( Lors de l'exécution de la requête SQL, on extrait un modèle de données )
        public DbSet<ApiDemandRequest> DemandRequest { get; set; }

        // > Table "ApiDemandProperties" (Modèles de données)  <
        public DbSet<ApiDemandProperties> DemandProperties { get; set; }

        // > Table "ApiDemandSqlCode" (Code de la requête SQL)  <
        public DbSet<ApiDemandSqlCode> DemandSqlCode { get; set; }
    }
}
