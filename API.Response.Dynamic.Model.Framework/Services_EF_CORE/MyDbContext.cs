using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace API.Response.Dynamic.Model.Framework.Services_EF_CORE
{
    public class MyDbContext : DbContext
    {
        // > Constructeur <
        public MyDbContext(DbContextOptions<MyDbContext>options):base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // > Va chercher les types dynamiques crées à l'exécution <
            var dynamicModelAssembly = Assembly.GetExecutingAssembly();

            var dynamicModelType = dynamicModelAssembly.GetType();

            foreach(var type in dynamicModelAssembly.GetTypes())
            {

                if (type.IsClass && !type.IsAbstract )
                {
                    // > Création de l'entité de Type "TypeBuilder" <
                    var entityType = modelBuilder.Entity(type);

                }

            }

        }

    }
}
