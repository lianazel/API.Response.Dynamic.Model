using API.Response.Dynamic.Model.Domain.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Response.Dynamic.Model.Controllers.V3_SQLSERVER
{

    [ApiController]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DynamicResponse3Controller : ControllerBase
    {
        #region fields
        // > Pour récupérer l'injection de dépandance <
        //   ( Voir la classe "Program.cs" )
        private readonly IParamRepository _repository = null;

        // > On déclare un membre pour récupérer l'envireonnement d'exécution <
        //   ( ici on en aura pas besoin, mais c'est pour l'exemple )
        private readonly IWebHostEnvironment _webHostEnvironment = null;

        #endregion

        #region Methods 

        // > Constructeur <
        public DynamicResponse3Controller(IParamRepository repository, IWebHostEnvironment webHost)
        {
            // > Récupération du repository < 
            _repository = repository;

            // > Récupération environnement exécution <
            _webHostEnvironment = webHost;

        }




        /// <summary>
        ///  Construction dynamique du modèle à partir de l'analyse des colonnes de la requête SQL (SQL SERVER).
        /// </summary>
        /// 
        /// <remarks>
        /// Entrez votre requête SQL, l'API vous renvérra le résultat sous JSON.
        /// Si l'exécution de la requête échoue, le système vous retourne un message d'erreur.
        /// 
        /// Paramètres :
        /// -------------
        ///  _StringCnx  : Chaine de connexion;
        ///  _SqlRequest : Requête SQL;
        ///  _Args       : Arguments d'extraction;
        /// </remarks>
        [Route("DynamicRequest")]
        [HttpGet]
        public IActionResult DynamicRequest([FromQuery] string? _StringCnx, string? _SqlRequest, string? _Args)
        {

            // > Par défaut, on est pessimiste <
            IActionResult result = this.BadRequest();

            return result;
        }
    }
}
#endregion