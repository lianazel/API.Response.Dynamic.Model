using API.Response.Dynamic.Model.Framework;

namespace API.Response.Dynamic.Model.Domain.Models
{
    public interface IParamRepository : IRepository
    {
        /// <summary>
        /// Ajout d'une demande de requête SQL 
        /// </summary>
        /// <param name="demandReques"></param>
        /// <returns></returns>
        public ApiDemandRequest AddDemandRequest(ApiDemandRequest demandReques);

        /// <summary>
        /// Mettre à jour une demande de requête SQL 
        /// </summary>
        /// <param name="demandReques"></param>
        /// <returns></returns>
        public ApiDemandRequest UpdDemandRequest(ApiDemandRequest demandReques);


        /// <summary>
        /// Renvoie d'une demande de requête ( on a "ICollection", mais on renvoie qu'un seul élement ) 
        /// </summary>
        /// <param name="ID_Demand"></param>
        /// <returns></returns>
        public ICollection<ApiDemandRequest> GetAllDemandRequest (string ID_Demand);

        /// <summary>
        /// Ajout d'une propriété 
        /// </summary>
        /// <param name="demandProperties"></param>
        /// <returns></returns>
        public ApiDemandProperties AddDemandProperties(ApiDemandProperties demandProperties);

        /// <summary>
        /// Renvoie d'une liste de propriétes ( on a "ICollection" => on renvoie une....
        /// ...liste de propriété liées à une demande de requête ) 
        /// </summary>
        /// <param name="FK_ID_Demand"></param>
        /// <returns></returns>
        public ICollection<ApiDemandProperties> GetAllDemandProperties(string FK_ID_Demand);

    }
}
