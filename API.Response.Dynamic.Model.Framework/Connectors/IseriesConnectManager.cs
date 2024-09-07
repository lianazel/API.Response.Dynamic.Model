using API.Response.Dynamic.Model.Domain.Models;
using System.Data.OleDb;
using System.Text;

namespace API.Response.Dynamic.Model.Infrastructures.Connectors
{
    public class IseriesConnectManager
    {
        #region properties
        // > Booléan résultat connexion <
        bool ResCnx;

        // > Objet OLEDB connexion <
        public OleDbConnection ConnectLst { get; set; }

        // > Chaine qui va recevoir l'instruction pour la connexion <
        public string BuildCnxString { get; set; }

        //string ConnectString;

        // > Constant USER  <
        //  ( Forcé à *NONE pour éviter toute connection PIRATE ) 
        const string CS_USER = "*NONE";

        // > Constant Mot de passe <
        //  ( Forcé à *NONE pour éviter toute connection PIRATE ) 
        const string CS_MDP = "*NONE";

        // > Constant Data Source  <
        const string CS_DTS = "172.29.28.51";

        // > Pour Construction via "StringBuilder" <
        const string CS_TXT_PROVIDER_PASSWORD = "Provider=IBMDA400;Password=";
        const string CS_TXT_USER_ID = "User ID=";
        const string CS_TXT_ADRESSE_IP = "Data Source=";

        // > Perperties connextion <
        //   => USER 
        string USER = null;
        //   => Mot de passe  
        string MDP = null;
        //   => Source de données <
        string DTS = null;



        #endregion

        public IseriesConnectManager(IseriesParamConnexion _PrmCnx)
        {
            USER = _PrmCnx.profil;
            MDP = _PrmCnx.Pswrd;
            DTS = _PrmCnx.IPadress;

            // > Lance la construction de la chaine de connexion <
            BuildCnxString = BuildConnectString();
        }

        /// <summary>
        ///   Ouverture de la connexion 
        /// </summary>
        /// <param name="_PrmCnx"></param>
        /// <returns></returns>
        public bool OpenCnx()
        {


            try
            {
                // > Remarque La méthode "BuildConnectString()" renvoie un string <
                ConnectLst = new OleDbConnection(BuildConnectString());

                // > Ouverture de la connextion <
                ConnectLst.Open();

                // > Tout est OK <
                ResCnx = true;

            }
            catch (Exception ex)
            {
                // > On a rencontré un problème <
                ResCnx = false;
            }

            finally

            { }

            return ResCnx;

        }

        /// <summary>
        /// Ferme Connexion 
        /// </summary>
        /// <returns></returns>
        public void CloseCnx()
        {
            ConnectLst.Close();
        }

        /// <summary>
        /// Construction chaine de connexion
        /// </summary>
        /// <returns></returns>
        private string BuildConnectString()
        {
            // > Optimisation Mémoire <
            // ( On utilise un "StringBuilder ) 
            StringBuilder ConnectString = new StringBuilder();
            ConnectString.Append(CS_TXT_PROVIDER_PASSWORD);

            // --- Mot de Passe ---
            // > Si mot de passe Non null <
            if (MDP != null)
            {
                ConnectString.Append(MDP);
                ConnectString.Append(";");
            }

            // > On injecte la constante ( pour le mot de passe )
            else
            {
                ConnectString.Append(CS_MDP);
                ConnectString.Append(";");
            }

            // --- Utilisateur ---
            // > Si utilisateur Non null <
            ConnectString.Append(CS_TXT_USER_ID);
            if (USER != null)
            {
                ConnectString.Append(USER);
                ConnectString.Append(";");
            }
            // > On injecte la constante ( pour l'utilisateur )
            else
            {
                ConnectString.Append(CS_USER);
                ConnectString.Append(";");
            }

            // --- Adresse IP  ---
            // > Si utilisateur Non null <
            ConnectString.Append(CS_TXT_ADRESSE_IP);
            if (DTS != null)
            {
                ConnectString.Append(DTS);
                ConnectString.Append(";");
            }
            // > On injecte la constante ( pour l'utilisateur )
            else
            {
                ConnectString.Append(CS_DTS);
                ConnectString.Append(";");
            }

            return ConnectString.ToString();

        }


    }
}
