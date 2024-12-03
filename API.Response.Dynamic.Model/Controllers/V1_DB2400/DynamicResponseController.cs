using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using API.Response.Dynamic.Model.Infrastructures.Services;
using API.Responses.Dynamic.Model.Infrastructures.Services;
using API.Response.Dynamic.Model.Infrastructures.Connectors;
using API.Response.Dynamic.Model.Domain.Models;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;


// - - - - - - - - - - - - - - -
// Versionning API 
// Voir :
//  https://mohsen.es/api-versioning-and-swagger-in-asp-net-core-7-0-fe45f67d8419
// - - - - - - - - - - - - - - -


namespace API.Response.Dynamic.Model.Controllers.V1_DB2400
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class DynamicResponseController : ControllerBase

    // public class DynamicApiController : ControllerBase
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
        public DynamicResponseController(IParamRepository repository, IWebHostEnvironment webHost)
        {
            // > Récupération du repository < 
            _repository = repository;

            // > Récupération environnement exécution <
            _webHostEnvironment = webHost;

        }


        // - - - - - - - - - - - - - - - 
        //              *** V1 ***
        //  *** En commantaire pour ne PAS surcharger la description du swagger ***
        //   > 1/ Construction dynamique du modèle à partir de ...
        //       ...l'analyse des colonnes de la requête SQL.
        //       
        //  > 2/ Remplissage du modèle avec les données renvoyées...
        //       ...par la requête SQL.
        //       
        // > 3/ Renvoie de la collection de modèles...
        //      ...(objets de types "TypeBuilder") au format JSON.
        // <param name="_Profil">         Profil de connexion                              </param>
        // <param name="_Pwrd">           Mot de passe profil                              </param>
        // <param name="_IPadresse">      Adresse IP AS400                                 </param>
        // <param name="_SqlRequest">     Requête SQL à exécuter                           </param>
        // <param name="_JobSummary">     Si Vrai => synthèse du job                       </param>
        // <param name="_DynamicModel">   Utilise un modèle dynamique OU un objet dynamique </param> 
        // - - - - - - - - - - - - - - - 

        /// <summary>
        ///  Construction dynamique du modèle à partir de l'analyse des colonnes de la requête SQL (DB2 400).
        /// </summary>

        /// <remarks>
        /// Entrez votre requête SQL, l'API vous renvérra le résultat sous JSON.
        /// Si l'exécution de la requête échoue, le système vous retourne un message d'erreur.
        /// </remarks>

        [Route("DynamicRequest")]
        [HttpGet]
        public IActionResult DynamicRequest([FromQuery] string _Profil, string _Pwrd, string? _IPadresse,
            string _SqlRequest, bool _ClauseWhere, bool _JobSummary, bool _DynamicModel)
        {

            // > Par défaut, on est pessimiste <
            IActionResult result = BadRequest();

            // --------------------
            // 1 - Analyse Requête SQL 
            // --------------------
            // > 1A = La requête SQL doitêtre de type "SELECT" <
            var sqltools = new ApiSqlTools(_SqlRequest);


            // > Charge un tableau de string avec contrôles à faires <
            string[] TbCtlSql = new string[] { "CHECKSELECT" };
            // > Si Contrôle existence clause "Where" demandée <
            if (_ClauseWhere)
            {
                string newElement = "CHECKWHERE";
                List<string> list = new List<string>(TbCtlSql.ToList());
                list.Add(newElement);
                TbCtlSql = list.ToArray();
            }

            // > Appelle la méthode contrôle en passant le tableau en paramètre <
            var resSql = sqltools.SqlFunctionsCtl(TbCtlSql);

            // > Si aucun message d'erreur ==> OK <
            if (resSql == "")
            {
                // > Types de Paramètres et autres Item <
                var ApiPType = new ApiParamType();

                // > Réception Message d'erreur <
                string statusjob = string.Empty;

                // > Réception des paraètres pour la connexion <
                var PrmCnx = new IseriesParamConnexion();
                // > Profil <
                PrmCnx.profil = _Profil;
                // > Mot de passe  <
                PrmCnx.Pswrd = _Pwrd;
                // > Source de données  <
                PrmCnx.IPadress = _IPadresse;

                // -------------------
                // 2 -  > Mise en place de la connexion OleDb <
                // -------------------
                //         ( Lance la construction de la chaine de connexion ) 
                //       ( Remarque à Déplacer dans classe "ApiConnexionManagerIseries" )
                var SQLR = new IseriesConnectManager(PrmCnx);

                // -------------------------
                // 2B - > Création d'un modèle de données qui va être une synthèse du traitement <
                // -------------------------
                var modelOnDemand = new ApiDynamicModelOnDemand();

                // > Ajout dans une liste des propriétés à créer <
                List<string> ListSyntColumns = new List<string>();
                ListSyntColumns.Add("Version_API");
                ListSyntColumns.Add("Count_Items");
                ListSyntColumns.Add("Items");

                // > Ajout des Types de chaque propriété  <
                Dictionary<string, Type> DictTypeColuumns = new Dictionary<string, Type>();
                //    -> Version de l'API <
                DictTypeColuumns["Version_API"] = typeof(string);
                //    -> Nombre D'Items dans la collection  <
                DictTypeColuumns["Count_Items"] = typeof(int);
                //    -> Collection de mdodèles Dynamiques <
                DictTypeColuumns["Items"] = typeof(List<dynamic>);

                // > Renvoie un objet de type TypeBuilder <
                //   ( "SummaryJob" ==> Résumé du traitement ) 
                var tbuilder = modelOnDemand.NewTypeBuilder("SummaryJob");

                // > Création du modèle de données avec les propriétés <
                var tbuilderFinal = modelOnDemand.AddDynamicProperty(tbuilder, ListSyntColumns, DictTypeColuumns, out List<PropertyInfo> _OutpropertiesInfos);

                // > Création de l'instance correspondant au modèle de données dynamique <
                //   ( Pour la méthode "InvokeMember", on a besoin d'un "object" et d'un "Type" )
                object SJinstanceModel = Activator.CreateInstance(tbuilderFinal.CreateType());
                Type SJtype = tbuilderFinal.CreateType();

                // -------------------
                // 3 -  > Lance construction du modèle dynamiqye à...
                //        ...partir du schema de colonnes renvoyées...
                //        ...par la requête SQL <
                //      > Exécution de la requête SQL ( et fermeture connection ) <
                //      > Les données sont chargées en mémoire pour mapping à l'étape 3 <
                // -------------------
                var modelFromSql = new ApiDynamicModelFromSql();

                statusjob = modelFromSql.BuilderModelFromSql(_SqlRequest, SQLR.BuildCnxString, "MyClass", out TypeBuilder typeBuilder,
                out List<PropertyInfo> propertiesInfos, out List<Dictionary<string, object>> RowsDictColumnsValue);

                // -------------------
                // 4 -  > Lance mappage des données renvoyées par la requête SQL..
                //        ...avec les colonnes du modèle dynamuque construit (typeBuilder).<
                //     > Remarque : tout le travail s'effectue maintenant en mémoire. <
                // -------------------
                // ( Si aucune erreur détectée avant ) 
                if (statusjob == string.Empty)
                {
                    var modelsDataFromSql = new ApiDynamicResultsFromSql();
                    statusjob = string.Empty;

                    statusjob = modelsDataFromSql.ReturnListObjects(typeBuilder, RowsDictColumnsValue, out List<object> OutInstanceMapped);

                    // > Le job s'est bien terminée <
                    if (statusjob == string.Empty)
                    {
                        // > Un résumé du traitement est-il demandé ? <                   
                        if (_JobSummary)
                        {
                            //   ( Faut-il utiliser un modèle dynamique ? ) 
                            if (_DynamicModel)
                            {
                                // > Version API <
                                SJtype.InvokeMember("Version_API", BindingFlags.SetProperty,
                                        null, SJinstanceModel, new object[] { ApiPType.CS_VERSION });

                                // >  Nombre d'Item dans la collection <
                                SJtype.InvokeMember("Count_Items", BindingFlags.SetProperty,
                                                  null, SJinstanceModel, new object[] { OutInstanceMapped.Count() });

                                // > Collection de modèles dynamiques <
                                SJtype.InvokeMember("Items", BindingFlags.SetProperty,
                                                    null, SJinstanceModel, new object[] { OutInstanceMapped });

                                result = Ok(SJinstanceModel);
                            }
                            //   ( Faut-il utiliser un OBJET  dynamique ? ) 
                            //     [ Je garde Cet possibilité pour l'exemple de Code ]
                            else
                            {
                                // > Attention ==> !!! AUCUN CONTROL DE TYPE !!! <
                                dynamic dynanmicInstance = new ExpandoObject();

                                // > Version API <
                                dynanmicInstance.VersionAPI = ApiPType.CS_VERSION;
                                // >  Nombre d'Item dans la collection <
                                dynanmicInstance.CountItems = OutInstanceMapped.Count();
                                // > Collection de modèles dynamiques <
                                dynanmicInstance.ListMapped = OutInstanceMapped;

                                result = Ok(dynanmicInstance);
                            }
                        }

                        //   ( Non : On renvoie simplement notre collection de modèles dynamiques ) 
                        else
                        {
                            result = Ok(OutInstanceMapped);
                        }

                    }

                    // > Problème détecté => on renvoie le message d'erreur <
                    else
                    {
                        result = BadRequest(statusjob);
                    }
                }

                // > Quelque chose s'est mal passé à la génération du modèle <
                else
                {
                    result = BadRequest(statusjob);
                }
            }
            else
            {
                result = BadRequest(resSql);
            }

            // > On renvoie le résultat de la demande  <
            return result;

        }

        // - - - - - - - - - - - - - - - - - -
        //              *** V1 ***
        //  *** En commantaire pour ne PAS surcharger la description du swagger ***
        //  > 1/ Construction dynamique du modèle à partir de ...
        //       ...l'analyse des colonnes de la requête SQL.
        //       
        //  > 2/ Sauegarde du modèle en base de données.
        // <param name="_Profil">         Profil de connexion                              </param>
        // <param name="_Pwrd">           Mot de passe profil                              </param>
        // <param name="_IPadresse">      Adresse IP AS400                                 </param>
        // <param name="_SqlRequest">     Requête SQL à exécuter                           </param>
        // <param name="_ModelName">      Nom que l'on donne  au modèle de données         </param>
        // <param name="_JobSummary">     Si Vrai => synthèse du job                       </param>
        // <param name="_DynamicModel">   Utilise un modèle dynamique OU un objet dynamique </param> 
        // - - - - - - - - - - - - - - - - - -


        /// <summary>
        /// Construction dynamique du modèle et Sauegarde du modèle en base de données.
        /// </summary>
        [Route("AddRequestModel")]
        [HttpPost]
        public IActionResult AddRequestModel([FromQuery] string _Profil, string _Pwrd, string? _IPadresse,
            string _SqlRequest, string _ModelName, bool _ClauseWhere, bool _JobSummary, bool _DynamicModel)
        {

            // > Par défaut, on est pessimiste <
            IActionResult result = BadRequest();

            // --  Start Test --
            var toto = new TEST_JC_CATEGORY();
            toto.Age = 18;
            string cat = toto.Category;
            // -- End Test --


            // > Pour interception message Erreur <
            StringBuilder sb = new StringBuilder();

            // --------------------
            // 1 - Analyse Requête SQL 
            // --------------------
            // > 1A = La requête SQL doitêtre de type "SELECT" <
            var sqltools = new ApiSqlTools(_SqlRequest);


            // > Charge un tableau de string avec contrôles à faires <
            string[] TbCtlSql = new string[] { "CHECKSELECT" };
            // > Si Contrôle existence clause "Where" demandée <
            if (_ClauseWhere)
            {
                string newElement = "CHECKWHERE";
                List<string> list = new List<string>(TbCtlSql.ToList());
                list.Add(newElement);
                TbCtlSql = list.ToArray();
            }

            // > Appelle la méthode contrôle en passant le tableau en paramètre <
            var resSql = sqltools.SqlFunctionsCtl(TbCtlSql);

            // > Si aucun message d'erreur ==> OK <
            if (resSql == "")
            {

                // > Réception Message d'erreur <
                string statusjob = string.Empty;

                // > Réception des paraètres pour la connexion <
                var PrmCnx = new IseriesParamConnexion();
                // > Profil <
                PrmCnx.profil = _Profil;
                // > Mot de passe  <
                PrmCnx.Pswrd = _Pwrd;
                // > Source de données  <
                PrmCnx.IPadress = _IPadresse;

                // -------------------
                // 2 -  > Mise en place de la connexion OleDb <
                // -------------------
                //         ( Lance la construction de la chaine de connexion ) 
                //       ( Remarque à Déplacer dans classe "ApiConnexionManagerIseries" )
                var SQLR = new IseriesConnectManager(PrmCnx);

                // -------------------
                // 3 -  > Lance construction du modèle dynamiqye à...
                //        ...partir du schema de colonnes renvoyées...
                //        ...par la requête SQL <
                //      > Exécution de la requête SQL ( et fermeture connection ) <
                //      > Les données sont chargées en mémoire pour mapping à l'étape 3 <
                // -------------------
                var modelFromSql = new ApiDynamicModelFromSql();

                statusjob = modelFromSql.BuilderModelFromSql(_SqlRequest, SQLR.BuildCnxString, _ModelName, out TypeBuilder typeBuilder,
                out List<PropertyInfo> propertiesInfos, out List<Dictionary<string, object>> RowsDictColumnsValue);

                // -------------------
                // 4 -  > Lance la sauvegarde en base du modèle extrait  <
                // -------------------

                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                // -- Ajout de la demande de requête SQL -- 
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                // > On appelle la méthode "AddDemandRequest" < du repository <
                //   ( On passe à la méthode "AddDemandRequest"...
                //     ...un paramétre de type "ApiDemandRequest" ).
                //   ( La méthode "AddDemandRequest" renvoie un objet "ApiDemandRequest" )

                Guid IDGuid = Guid.NewGuid();

                ApiDemandRequest DemandRequest = _repository.AddDemandRequest(new ApiDemandRequest()
                {
                    // > Nom du modèle de données associé à la demande de requête SQL  <
                    DataModelName = _ModelName,

                    // > Base de données : DB2/400 <
                    BDDType = "IBMDA400",

                    // > Nombre de propriétés contenues dans le modèle de données <
                    DataModelNumberProperties = propertiesInfos.Count(),

                    // > Génération de l'ID  par programme <
                    ID_Demand = IDGuid.ToString()
                });



                // > On effectue la validation l'ajout en base de données <
                try
                {
                    _repository.UnitOfWork.SaveChanges();
                }

                catch (Exception ex)
                {
                    // Error Message :
                    //  System.InvalidOperationException : 'The ConnectionString property has not been initialized.'

                    // voir :
                    // https://stackoverflow.com/questions/75677338/system-invalidoperationexception-the-connectionstring-property-has-not-been-ini
                    // https://stackoverflow.com/questions/70679775/system-invalidoperationexception-the-connectionstring-property-has-not-been-in

                    // > Interception Erreur <
                    sb.Append("ERR_A3200-");
                    sb.Append(ex.Message);

                    // > On renvoie le message d'erreur <
                    result = BadRequest(sb.ToString());
                    return result;
                }


                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                // -- Ajout des propriétés associéés à la demande de requête SQL  -- 
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                // > On appelle la méthode "AddDemandProperties" < du repository <
                //   ( On passe à la méthode "AddDemandProperties"...
                //     ...un paramétre de type "ApiDemandProperties" ).
                //   ( La méthode "AddDemandProperties" renvoie un objet "ApiDemandProperties" )

                // > Liste des propriétés ajoutées  <
                var listDemandProperties = new List<ApiDemandProperties>();
                foreach (var Item in propertiesInfos)
                {
                    IDGuid = Guid.NewGuid();

                    ApiDemandProperties DemandProperties = _repository.AddDemandProperties(new ApiDemandProperties()
                    {
                        // > Nom de la propriétés  <
                        PropertyName = Item.Name,

                        // > Type de la propriété  <
                        PropertyType = Item.PropertyType.ToString(),

                        // > Id de l'enregistrement <                      
                        ID_Properties = IDGuid.ToString(),

                        // > Clé éetrangère <
                        Fk_ID_Demand = DemandRequest.ID_Demand

                    });

                    // > On alimente la liste des propriétés <
                    listDemandProperties.Add(DemandProperties);
                }

                // > On effectue la validation l'ajout en base de données <
                try
                {
                    _repository.UnitOfWork.SaveChanges();
                }

                catch (Exception ex)
                {
                    // > Interception Erreur <
                    sb.Append("ERR_A3210-");
                    sb.Append(ex.Message);

                    // > On renvoie le message d'erreur <
                    result = BadRequest(sb.ToString());
                    return result;
                }

                // > On Charge dans la demande de requête SQL...
                //   ... les colonnes du modèle de données associées 
                DemandRequest.properties = listDemandProperties;

                // > On renvoie la demande SQL et ses propriétés qui ont...
                //   ...été ajoutées à la base <
                result = Ok(DemandRequest);

            }

            // > Un message d'erreur est détecté <
            else
            {
                result = BadRequest(resSql);
            }


            // > On renvoie le résultat de la demande  <
            return result;

        }

    }

    #endregion
}
