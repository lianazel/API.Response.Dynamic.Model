using Dapper;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;

namespace API.Responses.Dynamic.Model.Infrastructures.Services
{
    /// <summary>
    ///  > Construction dynamic du modèle de données depuis les colonnes renvoyées...
    ///  ...par la requête SQL <
    /// </summary>
    public class ApiDynamicModelFromSql
    {
     
        // > Récupération des paramètres de l'API <
        private ApiParamType paramType = new ApiParamType();

        // > Constructeur <
        public ApiDynamicModelFromSql() 
        { }


        /// <summary>
        /// > Construction dynamique du modèle de données...
        ///   ... à partir du schema des colonnes renvoyées par...
        ///   ... la requête SQL 
        /// </summary>
        /// <param name="_SqlRqte">                Requête SQL               </param>
        /// <param name="_ConnectionString">       Chaine de connextion      </param>
        /// <param name="_ClassName">              Nom de la nouvelle classe </param>
        /// <param name="OuttypeBuildFinal">       OUT - Nouvelle classe créee     </param>
        /// <param name="OutRowsDictColumnsValue"> OUT - Liste de ROW (ligne)=> une ligne est un dictionnaire ...
        /// < ...(Non Colonne, valeur de la colonne)
        /// <returns> string  </returns>
        public string BuilderModelFromSql(string _SqlRqte, string _ConnectionString, string _ClassName, out TypeBuilder OuttypeBuildFinal, 
            out List<PropertyInfo> OutpropertiesInfos, out List<Dictionary<string, object>> OutRowsDictColumnsValue)
        {
            // > Initialisation de la liste des propriétés qui sera retournées <
            OutpropertiesInfos = new List<PropertyInfo>();


            // > Initialisation de la liste des lignes (ROW) <
            //                  Nom Colonne     Valeur Colonne 
            //   ( Un "<Dictionary<string,       object>>()" represente UN  enregistrement  )
            //   ( List<Dictionary<string, object>>()        represente DES enregistrements )
            OutRowsDictColumnsValue = new List<Dictionary<string, object>>();

            // - - - - - - - - - - - - - - -
            // Initialisation diverses 
            // - - - - - - - - - - - - - - -
            // > Init du TypeBuilder qui sera renvoyé <
            OuttypeBuildFinal = null;

            // - - - - - - - - 
            // Gestion message Retour Traitement 
            // - - - - - - - - 
            // > Traitement : si message d'erreur à renvoyer  <
            string MsgJobStatus;
            MsgJobStatus = string.Empty;

            // > String Builder pour construction message erreur <
            StringBuilder sb = new StringBuilder();

            // - - - - - - - - 
            // Récupération des colonnes 
            // - - - - - - - - 
            // > Récupération des noms de colonnes dans une list<> <
            List<string> columnNamesList = new List<string>();
            // Dictionnaires contenant le type de chaque colonne <
            Dictionary<string, Type> DicTypeColumns = new Dictionary<string, Type>();

            // - - - - - - - - 
            // > Déclare Gestionnaire de connexion < 
            // - - - - - - - - 
            var connexionmanager = new ApiConnexionManager();

            // - - - - - - - - 
            // > Déclare le générateur de modèle dynamique <
            // - - - - - - - - 
            var modelOnDemanand = new ApiDynamicModelOnDemand();

            // - - - - - - - -
            // > Gestion de la connexion : ouverture  <
            //  (Ici, connexion à une base DB2 Iseries ) 
            // - - - - - - - -
            try
            {
                // > Echec de l'ouverture de connexion < 
                if (connexionmanager.CnxManage(true, _ConnectionString, out MsgJobStatus) != true)
                {
                    return MsgJobStatus;
                }
            }

            catch
            { }

            // - - - - - - - -
            // > Exécution SQL & construction dynamic du modèl   <
            // - - - - - - - -
            try
            {
                SqlCommand sqlCommand = new SqlCommand(_SqlRqte);

                //    - - - - E x e c u t i o n     S Q L     R e q u ê t e - - - - - - - -
                // >  Initialise une nouvelle instance de la classe OleDbCommand & Exécute la requête SQL   < 
                var command = new OleDbCommand(sqlCommand.CommandText, connexionmanager.ConnectLst);
                              
                // > Renvoie du resultat de la requête SQL dans une liste d'objets que l'on pourra parcourir <
                var resultst = connexionmanager.ConnectLst.Query(sqlCommand.CommandText).ToList();

                // > Numéro de colonne <
                int NumCol = 0;

                // > Extraction des types de colonnes <
                //   ( On ne le fait que pour la première ligne )
                bool bTypeColumn = false;

                // > Dictionnaire Le type de valeur pour chaque colonne <
                //   ( On exécute l'analyse qu'une seule fois ) 
                Dictionary<string, Type> DictColumnNameType = new Dictionary<string, Type>();

                //    - - - - T e s t   P r e s e n c e  E n r e g i s t r e m e n t s - - - - - - - -
                if (resultst.Any())
                {
                    // > Si la requête SQL renvoie trop d'enregistrements, on débranche < 
                    if (resultst.Count() > paramType.CS_MAXRECORDS)
                    {
                        // > Interception Erreur <
                        sb.Append("ERR_A2100-");
                        sb.Append("Cette Requête SQL renvoie trop d'enregistrements. Veuillez affiner votre requête SQL.");
                        MsgJobStatus = sb.ToString();

                        return MsgJobStatus;
                    }

                    // > Pour chaque LIGNE <
                    foreach (var ObjRow in resultst)
                    {
                        // > Si PAS déjà fait ==> construit TABLEAU DE STRING avec les noms de colonnes de l'enregistrement < 
                        if (bTypeColumn == false)
                        {
                            // > Récupére ici la liste des colonnes avec leurs Noms  <
                            columnNamesList = (ObjRow as IDictionary<string, object>)?.Keys.ToList();
                        }

                        // > Récupére ici la liste des colonnes avec leurs VALEURS  <
                        var ColumValues = (ObjRow as IDictionary<string, object>)?.Values.ToList();

                        // > Instanciation nouveau Dictionnaire qui va contenir couples "NomColonne/Valeurs "
                        //   ( A chaque nouvelle ligne, on instancie un nouveau dictionnaire )
                        //       ( Ce nouveau dictionnaire contiendra l'enregistrement ) 
                        Dictionary<string, object> DictColumnNameAndValue = new Dictionary<string, object>();

                        NumCol = 0;
                        foreach (var ColumName in columnNamesList)
                        {
                            DictColumnNameAndValue[ColumName] = ColumValues[NumCol];

                            // > Si PAS déjà fait ==> On extrait le TYPE de chaque COLONNE en fonction de sa VALEUR <
                            //  ( On ne fait ce traitement que pour le ...
                            //    ...premier enregistrement => pas la peine de le...
                            //    ...refaire pour TOUS les enregistrements ) 
                            if (bTypeColumn == false)
                            {
                                object Item = DictColumnNameAndValue[ColumName];
                                Type ItemType = Item.GetType();
                                DictColumnNameType[ColumName] = ItemType;
                            }

                            NumCol++;
                        }

                        // > Ajoute la ligne à la liste contenant TOUTES les lignes <
                        OutRowsDictColumnsValue.Add(DictColumnNameAndValue);

                        // > Optimisation mémoire avec la méthode "TrimExcess" <
                        OutRowsDictColumnsValue.TrimExcess();

                        // > Extraction des types de colonnes EFFECTUEE <
                        bTypeColumn = true;
                    }

                    // - - - - - - -
                    // > Création dynamique de la classe correspondante <
                    // - - - - - - -
                    // > Construction du corps de la classe (rappel : c'est "TypeBuilder" ) <
                    //   ( La méthode renvoie un objet de type "TypeBuilder" )
                   //  TypeBuilder typeBuild = NewTypeBuilder(_ClassName);
                    TypeBuilder typeBuild =   modelOnDemanand.NewTypeBuilder(_ClassName);

                    // > Ajout des propriétés (Colonnes de la requête SQL) à la classe Dynamique <
                    //   ( - la méthode renvoie un TypeBuilder avec les toutes les propriétés "OuttypeBuildFinal" )
                    //   ( - Une liste des propriétés  ( "List<PropertyInfo> propertiesInfos" )                  )
                    List<PropertyInfo> propertiesInfos = new List<PropertyInfo>();
                    OuttypeBuildFinal = modelOnDemanand.AddDynamicProperty(typeBuild, columnNamesList, DictColumnNameType, out propertiesInfos);
                   // OuttypeBuildFinal = AddDynamicProperty(typeBuild, columnNamesList, DictColumnNameType, out propertiesInfos);

                    // > Chargement de la liste des propriétes qui est renvoyée <
                    OutpropertiesInfos = propertiesInfos;

                }

            }

            // > Echec exécution requête SQL <
            // ( Interception message )
            catch (Exception ex)
            {
                // > Fermeture  de la connexion  <
                // > (Avant de renvoyer le message ) <
                connexionmanager.CnxManage(false, null, out MsgJobStatus);

                // > Interception Erreur <
                sb.Append("ERR_A2105-");
                sb.Append(ex.Message);
                MsgJobStatus = sb.ToString();
                              

                return MsgJobStatus;

            }

            // > Fermeture  de la connexion <
            // > ( Le job s'est bien déroulé ) <
            connexionmanager.CnxManage(false, null, out MsgJobStatus);

            MsgJobStatus = string.Empty;    
            return MsgJobStatus;

        }



        /// <summary>
        ///  Construction du corps de la classe dynamique ( ou "TypeBuilder" ) 
        /// </summary>
        /// <param name="_ClassName"> Nom de la classe à créer </param>
        /// <returns></returns>
        private TypeBuilder NewTypeBuilder(string _ClassName)
        {

            AssemblyName AssName = new AssemblyName("DynamicAssembly");

            // > Spécifie que l'assemblie dynamique doit être capable de s'exécuter. <
            // => Cela permet d'accéder aux types et membres crées dynamiquement dans l'assembly.
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(AssName,
           AssemblyBuilderAccess.Run);

            // > Pour résumer, méthode "DefineDynamicModule" permet de créer un module dynamique ...
            // ...dans l'assembly dynamique . On pouura y ajouter des : 
            // ==> des types,
            // ==> d'autres éléments dynamiques.
            ModuleBuilder Modbuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // > On définit ici une nouvelle classe dynamique <
            // ==> On passeen paramètre : 
            // >> Le Nom de la nouvelle classe,
            // >> Non niveau de protection,
            // >> La classe parente ( ici on a null)
            TypeBuilder typeBuild = Modbuilder.DefineType(_ClassName, TypeAttributes.Public, null);

            return typeBuild;

        }

        /// <summary>
        ///  > Ajout des propriétées dans la classe créée <
        /// </summary>
        /// <param name="_typeBuild">        La nouvelle classe créee dynamiquement  </param>
        /// <param name="_columnNamesList">  Liste des colonne à ajouter             </param>
        /// <param name="_DicTypeColumns">   Pour chaque colonne, le type            </param>
        /// <param name="_OutpropertiesInfos">  Renvoi la liste des propriétes dans le model dynamique </param>
        /// <returns> TypeBuilder </returns>
        private TypeBuilder AddDynamicProperty(TypeBuilder _typeBuild, List<string> _columnNamesList,
            Dictionary<string, Type> _DicTypeColumns, out List<PropertyInfo> _OutpropertiesInfos)
        {

            // > Voir : https://learn.microsoft.com/fr-fr/dotnet/api/system.reflection.emit.typebuilder.defineproperty?view=net-8.0 <

            // > Initialisation de la liste <
            _OutpropertiesInfos = new List<PropertyInfo>();

            // > On parcourt la liste des colonne à ajouter dans la nouvelle classe  <
            foreach (string columnName in _columnNamesList)
            {
                // - - - - - - - - - - - 
                // > Définir le champ sous-jacent correspondant à la proprité <
                // - - - - - - - - - - - 
                // Attention : ici, au lieu d'utiliser le "typeof" : 
                // "FieldBuilder fieldBuilder = _typeBuild.DefineField("_" + columnName.ToLower(), typeof(string), FieldAttributes.Public);"
                // ==> on met directement le type renvoyé par le dictionnaire 
                FieldBuilder fieldBuilder = _typeBuild.DefineField(columnName.ToUpper(), 
                    (_DicTypeColumns[columnName]), FieldAttributes.Public);


                // - - - - - - - - - -
                // > Définir la propritété correspondante  <
                // - - - - - - - - - -
                PropertyBuilder propertyBuilder = _typeBuild.DefineProperty(columnName.ToUpper(),
                    PropertyAttributes.HasDefault, _DicTypeColumns[columnName],null);

                // - - - - - - - -
                // > Ajout du "propertyBuilder" crée dans la liste "_OutpropertiesInfos"  <
                // - - - - - - - -
                _OutpropertiesInfos.Add(propertyBuilder);

                // Microsfot - The property set and property get methods require a special
                // set of attributes.
                MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

                // - - - - - - - - - - -
                // > Définir l'accesseur "Getter" de la propriété   <
                // - - - - - - - - - - -
                MethodBuilder getterMethod = _typeBuild.DefineMethod("get_" + columnName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, _DicTypeColumns[columnName], Type.EmptyTypes);

                ILGenerator getterIL = getterMethod.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);


                // - - - - - - - - - - -
                // > Définir l'accesseur "Setter" de la propriété   <
                // - - - - - - - - - - -
                // MethodBuilder setterMethod = _typeBuild.DefineMethod("set_" + columnName,
                //    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, _DicTypeColumns[columnName], Type.EmptyTypes);                                         

                // Remarque : le "getSetAttr" remplace ici le...
                // ..."MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig," de la syntaxe de la ligne en commentaire ci dessus 

                MethodBuilder setterMethod =
               _typeBuild.DefineMethod("set_" + columnName,
                                       getSetAttr,
                                       null,
                                       new Type[] { _DicTypeColumns[columnName] });

                ILGenerator setterIL = setterMethod.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                // - - - - - - - - - - -
                // > Associer l'accesseur "Getter" à la propriété   <
                // - - - - - - - - - - -
                propertyBuilder.SetGetMethod(getterMethod);

                // - - - - - - - - - - -
                // > Associer l'accesseur "Setter" à la propriété   <
                // - - - - - - - - - - -
                propertyBuilder.SetSetMethod(setterMethod);

            }

            return _typeBuild;

        }

    }
}