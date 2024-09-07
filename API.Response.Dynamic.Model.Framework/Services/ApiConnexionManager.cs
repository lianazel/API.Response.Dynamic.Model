
using System.Data.OleDb;
using System.Text;


namespace API.Responses.Dynamic.Model.Infrastructures.Services
{
    public class ApiConnexionManager
    {
        #region properties

        // > Objet OLEDB connexion <
        public OleDbConnection ConnectLst { get; set; }

        // > Booléan résultat connexion <
        private bool ResCnx;

        #endregion

        // > Constructeur <
        public ApiConnexionManager() { }

        #region methods

       
        /// <summary>
        ///  > Gestion de la connexion <
        /// </summary>
        /// <param name="_Mode">             "True" => Ouverture / "False" => Fermture  </param>
        /// <param name="_ConnectionString"> Chaine de connexion </param>
        /// <param name="msgstatus">         Message renvoyé en cas d'erreur  </param>
        /// <returns></returns>
        public bool CnxManage(bool _Mode, string? _ConnectionString, out string? msgstatus)
        {
            // > inoitialise string <
            msgstatus = string.Empty;

            StringBuilder sb = new StringBuilder();

            // > "True" ==> Ouverture demandée <
            if (_Mode)
            {
                try
                {
                    // > Remarque La méthode "BuildConnectString()" renvoie un string <
                    ConnectLst = new OleDbConnection(_ConnectionString);

                    // > Ouverture de la connextion <
                    ConnectLst.Open();
            

                    // > Tout est OK <
                    ResCnx = true;

                }
                catch (Exception ex)
                {
                    sb.Append("ERR-MBF-001A");
                    sb.Append(ex.Message);
                    msgstatus = sb.ToString();

                    // > Tout est OK <
                    ResCnx = false;
                }
            }

            // > Fermeture demandée <
            else
            {
                ConnectLst.Close();
            }

            return ResCnx;

        }

        #endregion 

    }
}
