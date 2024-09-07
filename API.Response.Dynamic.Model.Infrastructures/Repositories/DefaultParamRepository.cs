
// > Pour "Include" <
using API.Response.Dynamic.Model.Domain.Models;
using API.Response.Dynamic.Model.Framework.UnitOfWork;
using API.Response.Dynamic.Model.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Response.Dynamic.Model.Infrastructures.Repositories
{
    public  class DefaultParamRepository : IParamRepository
    {
        #region fields
        // > Déclare le context qui sera alimenté par injection de dépendance <
        public readonly DataContext _Context = null;
        #endregion

        #region property
        // > Installe le "SaveChange".
        public IUnitOfWork UnitOfWork => this._Context;
        #endregion  

        #region methods

        // - - - - - - - - - - - - - - - - - - - - 
        // ## Les demandes de requêtes SQL ( DemandRequest )
        //    ( Une demande génére en mémoire un objet de type "TypeBuilder" qui...
        //      ...sera l'enveloppe du futur modèle de données )
        // - - - - - - - - - - - - - - - - - - - - 
        /// <summary>
        /// Constructeur de la classe 
        /// </summary>
        /// <param name="Context"></param>
        public DefaultParamRepository(DataContext Context)
        {
            // > Récupération du context < 
            this._Context = Context;
        }

        /// <summary>
        /// Ajout d'une demande de requête SQL
        /// Rappel - Voir la méthode "NewTypeBuilder" de la classe "ApiDynamicModelOnDemand" 
        ///   ( Dans la méthode "NewTypeBuilder" , on construit le "TypeBuilder" ...
        ///     ...==> soit l'enveloppe de la classe modèle ) 
        /// </summary>
        /// <param name="demandRequest"></param>
        /// <returns></returns>
        public ApiDemandRequest AddDemandRequest(ApiDemandRequest _demandRequest)
        {                     
            // > On retourne l'entité tracé par l'ORM <
            return this._Context.DemandRequest.Add(_demandRequest).Entity;                
        }


        /// <summary>
        /// Renvoie liste de demande de requête SQL  ou une demande requête SQL (si un ID est transmis).
        /// Rappel - Voir la méthode "NewTypeBuilder" de la classe "ApiDynamicModelOnDemand" 
        ///   ( Dans la méthode "NewTypeBuilder" , on construit le "TypeBuilder" ...
        ///     ...==> soit l'enveloppe de la classe modèle ) 
        /// </summary>
        /// <param name="_ID_Demand"></param>
        /// <returns></returns>
        public ICollection<ApiDemandRequest> GetAllDemandRequest(string _ID_Demand)
        {
            // > Extraction collection des demandes de REQUETES SQL
            //    ( "AsQueryable()" pour autoriser derrière une clause "where" ) 
            var Elements = this._Context.DemandRequest.AsQueryable();

            // > Si un ID est transmis, on ajoute une clause "where" 
            if (_ID_Demand != "")
            {
                Elements = Elements.Where(item => item.ID_Demand == _ID_Demand);
            }
            return Elements.ToList();
        }

        /// <summary>
        /// Mettre à jour une demande de requête SQL 
        /// Rappel - Voir la méthode "NewTypeBuilder" de la classe "ApiDynamicModelOnDemand" 
        ///   ( Dans la méthode "NewTypeBuilder", on construit le "TypeBuilder" ...
        ///     ...==> soit l'enveloppe de la classe modèle ) 
        /// </summary>
        /// <param name="_demandRequest"></param>
        /// <returns></returns>

        public ApiDemandRequest UpdDemandRequest(ApiDemandRequest _demandRequest)
        {
            // > Lecture d'un enregistrement et mise à jour de celui ci <
            var DemandSql = this._Context.DemandRequest.FirstOrDefault(f => f.ID_Demand == _demandRequest.ID_Demand);

            // > Si l'enregistrement LU n'est PAS null <
            if (DemandSql != null)
            {
                // > Nom du modèle de données extrait par le code   <
                //   ( On donnera ce nom à la classe qui sera construite en mémoire )
                DemandSql.DataModelName = _demandRequest.DataModelName;

                // > Type de base de données  <
                DemandSql.BDDType = _demandRequest.BDDType;
            }
            else
            {
                DemandSql = null;
            }
            // > On retourne l'entité tracé par l'ORM <
            return this._Context.DemandRequest.Update(DemandSql).Entity;                      
        }



        // - - - - - - - - - - - - - - - - - - - - 
        // ## Les propriétés du modèle ( contenues dans l'enveloppe ...
        //    ...de type "TypeBuilder" )     
        // - - - - - - - - - - - - - - - - - - - - 

        /// <summary>
        /// Ajout d'une propriété 
        /// Rappel - Voir la méthode "AddDynamicProperty" de la classe "ApiDynamicModelOnDemand" 
        ///   ( Dans la méthode "AddDynamicProperty", on ajoute les colonnes (noms et types de colonnes) extraites...
        ///     ..de la requête SQL à exécuter ) 
        /// </summary>
        /// <param name="_demandProperties"></param>
        /// <returns></returns>
        public ApiDemandProperties AddDemandProperties(ApiDemandProperties _demandProperties)
        {
            // > On retourne l'entité tracé par l'ORM <
            return this._Context.DemandProperties.Add(_demandProperties).Entity;
        }


        /// <summary>
        /// Renvoie d'une liste de propriétes ( on a "ICollection" => on renvoie une....
        /// ...liste de propriété liées à une demande de requête ) 
        /// Rappel - Voir la méthode "AddDynamicProperty" de la classe "ApiDynamicModelOnDemand" 
        ///   ( Dans la méthode "AddDynamicProperty", on ajoute les colonnes (noms et types de colonnes) extraites...
        ///     ..de la requête SQL à exécuter ) 
        /// </summary>
        /// <param name="_FK_ID_Demand"></param>
        /// <returns></returns>
        public ICollection<ApiDemandProperties> GetAllDemandProperties(string _FK_ID_Demand)
        {
            // > Extraction collection des propiétés pour un ID demandRequest
            //    ( "AsQueryable()" pour autoriser derrière une clause "where" ) 
            var Elements = this._Context.DemandProperties.AsQueryable();

            // > Si un ID Demande Requête SQL est transmis, on ajoute une clause "where" <
            //    ( On renvoie toutes les propriétés associés à cette demande de requête SQL )
            if (_FK_ID_Demand != "")
            {
                Elements = Elements.Where(item => item.Fk_ID_Demand == _FK_ID_Demand);
            }
            return Elements.ToList();

        }

        #endregion

    }
}
