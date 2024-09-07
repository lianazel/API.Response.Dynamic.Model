using System.Globalization;

using System.Text;

namespace API.Responses.Dynamic.Model.Infrastructures.Services
{

    /// <summary>
    /// Clase de service pour construction dynamique instruction SQL "Where..."
    /// </summary>
    public class ApiBuildSqlWhere
    {
        #region properties 
        //  > Pour la construction du nom de schéma <
        private StringBuilder Schema;

        // - - - - - - - - - - - - - 
        // > TRAITEMENT DATE : conversion "string" ==> "DataTime" < 
        // - - - - - - - - - - - - -
        private CultureInfo provider = CultureInfo.InvariantCulture;
        // > Conversion "DateTime" en "string" < 
        private string[] format = { "yyyyMMdd" };
        private DateTime date;

        // - - - - - - - - -
        // > Insertion de l'instruction "Where" <
        // - - - - - - - - -
        // ( si le booléan est à "false", ==> where pas encore intégré )
        private bool BWhere;


        // - - - - - - - - -
        // > Types de Paramètres et autres Item <
        // - - - - - - - - -
        private ApiParamType ApiPType = new ApiParamType();

        #endregion

        // > Constructeur <
        public ApiBuildSqlWhere()
        {

        }

        #region Methods


        /// <summary>
        ///  Extraction des paramètres & construction requète SQL 
        /// </summary>
        /// <param name="_Paramter"></param>     => Paramètre à analyser 
        /// <param name="_AndOrOpeLog"></param>  => Opérateur "OR" (||), "AND" (&&) 
        /// <param name="DictColumns"></param>   => Columne utilisée pour la requête SQL 
        /// <returns></returns>
        public Dictionary<int, object> FindFilters(string _Paramter, string? _AndOrOpeLog, Dictionary<string, string> _DictColumns)
        {
            // > Dictionnaire  <
            // Dictionary<string, object> Params = new();
            string ItemCondition;

            // > On extrait les blocs de condition séparés par des && ( soit des "AND" )
            //  Exemple  :   <NOMPEY:[=:TI/> && <PRENPY:[]:AL/>
            //  Remarque "_AndOrOpeLog" peut contenir "&&" (AND) ou "||" (OR).
            string[] blocks = _Paramter.Split(_AndOrOpeLog);


            // > Va recevoir la liste des arguments séparés par un "AND" <
            StringBuilder paramtersBuilded = new StringBuilder();


            foreach (string block in blocks)
            {
                // ---------------
                // > On appelle l'analyseur de paramètre pour le bloc <
                // ---------------
                ItemCondition = FindFilterItem(block + ";", _DictColumns);

                paramtersBuilded.Append(ItemCondition);

                // > On ajoute " AND " ou " OR "  ( selon ce que renvoie le dictionnaire "DicOpeLogAndOr" ) 
                paramtersBuilded.Append(ApiPType.DicOpeLogAndOr[_AndOrOpeLog]);


            }

            // > Si l'instruction SQL se termine par " AND " ou " OR "...
            //  ... ( selon ce que renvoie le dictionnaire ), alors on supprime...
            //  ...   l'opérateur logique en trop ).
            // Exemple : si " WHERE NOMPEY LIKE'TI%' AND" ==> il faut surrpimer le " AND ". 
            if (paramtersBuilded.ToString().EndsWith(ApiPType.DicOpeLogAndOr[_AndOrOpeLog]))
            {
                // Syntaxe : ==> public string Remove(int StartIndex, int count)  
                // Lien https://www.geeksforgeeks.org/c-sharp-remove-method/
                // Rappel :
                //   1/ -> "_AndOrOpeLog" : contient "&&" ou "||" ( défini dans "ApiParmType" ).
                //   2/ -> Le dictionnaire "DicOpeLogAndOr" de la classe "ApiParamType" renvoie soit un "AND", soit un "OR".

                //   Méthode ".Remove(StartIndex, Lenght)"
                paramtersBuilded.Remove((paramtersBuilded.Length - (ApiPType.DicOpeLogAndOr[_AndOrOpeLog].Length)), ApiPType.DicOpeLogAndOr[_AndOrOpeLog].Length);
            }


            // > On renvoie ce dictionnaire <
            Dictionary<int, object> DicResponse = new();
            DicResponse[0] = paramtersBuilded.ToString();

            return DicResponse;

        }


        /// <summary>
        /// Extraction de composant de paramètre 
        /// </summary>
        /// <param name="_Paramter"></param>
        /// <returns></returns>
        private string FindFilterItem(string _Paramter, Dictionary<string, string> _DictColumns)

        {

            // > Extraction Nom colonne  <
            int StartIndex;
            string? ColumnName;

            // > Récéption de la colonne qui servira à la construction SQL du "where..." <
            //    On le récupère via interrogation du dictionnaire "_DictColumns" <
            string? ColumnSql;

            // > Extraction Opérateur de comparaison ("==";"!=";"[=";...)  <
            string OpeComp;

            // > Position Premier séparateur <
            int FirstSep;

            // > Position Second séparateur <
            int SecondSep;
            int EndIndex;

            // > Extraction Opérateur de comparaison  <
            //  Les opérateurs de comparaison peuvent être :
            //     "==" ou "!=" ou "[=" ou "=]" ou "[]" 
            OpeComp = extractOpeComp(_Paramter);


            // > Extraction Valeur Paramètre <
            //   > Longueur de la valeur du paramètre <
            int ValueLength;
            //   > Valeur du paramètre <
            string Value;

            // > Recherce la position de début du nom de colonne <
            StartIndex = _Paramter.IndexOf(ApiPType.CS_DELIMITEURS_DEB) + ApiPType.CS_DELIMITEURS_DEB.Length;

            //   > Recherche position PREMIER separateur (premier ":") <
            //    (exemple: < NOPPHN:==:5 /> ) 
            FirstSep = _Paramter.IndexOf(ApiPType.CS_SEPARATEUR);


            // > Extraction Nom colonne (exemple : <NOPPHN:==:5/> ) 
            ColumnName = _Paramter.Substring(StartIndex, FirstSep - StartIndex);

            // > On cherche le VRAI nom de la colonne <
            try
            {
                ColumnSql = _DictColumns[ColumnName.ToUpper()];
            }
            // > On a rien trouvé dans le dictionnaire ==> on charge avec null <
            catch 
            {
                ColumnSql = null;
            }

            finally
            {

            }
            // > "ColumnSql" est "null" ==> on travaille alors avec "ColumnName" <
            //   ( La recherche dans le dictionnaire a échoué )
            if (ColumnSql == null || ColumnSql.Length == 0 ) 
            {
                ColumnSql = ColumnName;
            };


            //   > Recherche position DEUXIEME separateur (deuxieme ":") <
            //    (exemple: < NOPPHN:==:5 /> ) 
            //     (public int IndexOf (string value, int startIndex, int count);  https://learn.microsoft.com/fr-fr/dotnet/api/system.string.indexof?view=net-7.0
            SecondSep = _Paramter.IndexOf(ApiPType.CS_SEPARATEUR, (FirstSep + 1));

            // > Recherche position du "/>" et retire à la position trouvée la longeur de "/>" soit 1 
            EndIndex = (_Paramter.IndexOf(ApiPType.CS_DELIMITEURS_FIN) - 1);

            // > Calcule longueur valeur paramètre <
            ValueLength = EndIndex - SecondSep;

            // > Extraction de la veleur <
            Value = _Paramter.Substring((SecondSep + ApiPType.CS_SEPARATEUR.Length), ValueLength);
            // > Suupression des eventuelles Espaces <
            Value = Value.Trim();   


            // > On retourne le dictionnaire alimenté par la méthode "checkValue" <
            // > On renvoie ce dictionnaire  qui contient  :
            //     > Type valeur [("INT", "DAT", "STR")] [Object]  <
            Dictionary<string, object> DictValue = new();

            // - - - - - - - - - - - - - - - - -  - - - - - -
            // > Analyse type de valeur ( string; int, date )
            // - - - - - - - - - - - - - - - - - - - - - - -
            // => Paramètres transmis :
            //   > Valeur à analyser pour déterminer le type <
            DictValue = ValueCheck(Value);

            // - - - - - - - - - - - - - - - - - - - -
            // > Construction finale de la chaine de test pour le "WHERE"  <
            // - - - - - - - - - - - - - - - - - - - -
            return BuildSqlWhereInstruction(ColumnSql, DictValue, OpeComp);

        }

        /// <summary>
        /// > Construction instruction "Sql Where..." : reçoit en paramètre : Nom colonne, valeur, opérateur comparaison <
        /// </summary>
        /// <param name="_ColumnNane"></param>
        /// <param name="_DictParams"></param>
        /// <param name="_OpeComp"></param>
        /// <returns></returns>
        private string BuildSqlWhereInstruction(string _ColumnNane, Dictionary<string, object> _DictParams, string _OpeComp)
        {
            // - - - - - - - - - - - - - - - - - - - -
            // > Construction de la chaine de test pour le "WHERE"  <
            // - - - - - - - - - - - - - - - - - - - -
            StringBuilder SqlWhere = new StringBuilder();

            // > Pour les test de type "Compris entre", il faut n° Item <
            int NumItem;

            // > On insére l'ordre SQL "where" qu'une seule fois <
            if (BWhere == false)
            {
                SqlWhere.Append(" where ");
                BWhere = true;
            }

            SqlWhere.Append(_ColumnNane);

            //  Bloc Consruction Chaine SQL "Where" 
            switch (_OpeComp)
            {
                // Egalite? 
                case "==":
                   
                    SqlWhere.Append(" = ");

                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));
                    break;


                // Commence par ?
                case "[=":
                  
                    // > Commence par ==> "like'%valeur'" <
                    SqlWhere.Append(" like'");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));
                    SqlWhere.Append("%'");
                    break;

                // Se termine par ?
                case "=]":
                   
                    // > Termine par ==> "likealeur" <

                    SqlWhere.Append(" like'%");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));
                    SqlWhere.Append("'");

                    break;

                // Contient  ?
                case "[]":
                  
                    SqlWhere.Append(" like'%");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));
                    SqlWhere.Append("%'");

                    break;

                // Inférieur  ?
                case "<":
                    
                    SqlWhere.Append(" < ");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));

                    break;

                // Inférieur ou égal  ?
                case "<=":
                    
                    SqlWhere.Append(" <= ");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));

                    break;

                // Supérieur ?
                case ">":
                   
                    SqlWhere.Append(" > ");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));

                    break;

                // Supérieur ou égal ?
                case "=>":
                    
                    SqlWhere.Append(" => ");
                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, null));

                    break;

                    // Compris entre ?
                    // (EN SQL, on a ==> Valeur_1 > NomColonne < Valeur_2) )
                case "><":

                    SqlWhere.Append(" BETWEEN ");

                    // > Va installer la valeur selon son type <
                    //   ( Appelle de la méthode "AddCaseTypValue" ) 
                    // "NumItem = 1 " ==> première borne 
                    // < DTFATG:><:20230101; 20230601 /> ( donc ici 20230101 ) 
                    //  A finir dans "AddCaseTypValue" 
                    NumItem = 0;
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, NumItem));

                    SqlWhere.Append(" AND ");                              
                  
                    // "NumItem = 1 " ==> deuxième borne 
                    // < DTFATG:><:20230101; 20230601 /> ( donc ici 20230601 )
                    //  A finir dans "AddCaseTypValue" 
                    NumItem = 1;
                    SqlWhere = (AddCaseTypValue(_DictParams, SqlWhere, _OpeComp, NumItem));

                    break;
            }

            // > On renvoie la chaine "Where ...." construite avec les elements <
            return SqlWhere.ToString();

        }


        /// <summary>
        ///  Installation de la valeur dans la chaine selon son TYPE 
        /// </summary>
        /// <param name="_Params"></param>
        /// <param name="_SqlWhere"></param>
        /// <param name="_OpeComp"></param>
        /// <returns></returns>
        private StringBuilder AddCaseTypValue(Dictionary<string, object> _DictParams, StringBuilder _SqlWhere, string _OpeComp, int? _NumItem)
        {

            // > Pour le "TryGetValue" depuis le dictionnaire <
            object GetValue;
            GetValue = null;


            // > Pour recevoir le contenu du doctionnaire <
            string Item;

            // > pour recevoir les deux items séparés par un ";" (voir "CS_SEPARATEUR_LIST_VALUE") <
            string[] TabItems = new string[2];

       

            // > Extraction valeur d'un dictionnaire :
            // https://www.delftstack.com/fr/howto/csharp/csharp-check-if-dictionary-has-key/
            // - - - - - -
            // > S'agit-il d'un ENTIER ? <
            // - - - - - -
            if (_DictParams.TryGetValue(ApiPType.CS_TYP_INT, out GetValue))
            {
                // > On récupère simplement la valeur <
                if (_NumItem == null)
                {
                    _SqlWhere.Append(_DictParams[ApiPType.CS_TYP_INT]);
                }

                // > Sinon, on doit récupérer la bonne valeur depuis le tableau "TabItems" <
                // ( Rappel : on cherche à récupérer les élements de ...
                // ... "Valeur_1;Valeur_2" dans l'expression ...
                // ...  "< NomColonne:><:Valeur_1;Valeur_2 />"  ( EN SQL, on a ==> Valeur_1>NomColonne<Valeur_2) )
                else
                {
                    Item = (string)_DictParams[ApiPType.CS_TYP_INT];

                    // > On envoie dans le tableau les elements séparés par des ";" <
                    //   ( Remarque "Split" => ChaineDecoupe du Wlangage )
                    TabItems = Item.Split(ApiPType.CS_SEPARATEUR_LIST_VALUE);

                    // > On ajoute à la requête SQL le bon élément <
                    _SqlWhere.Append(TabItems[(int)_NumItem]);
                }
            }

            // - - - - - -
            // > S'agit-il d'une DATE  ? <
            // - - - - - -
            else
            {
                if (_DictParams.TryGetValue(ApiPType.CS_TYP_DAT, out GetValue))
                {
                    // > On récupère simplement la valeur <
                    if (_NumItem == null)
                    {
                        _SqlWhere.Append(_DictParams[ApiPType.CS_TYP_DAT]);
                    }

                    // > Sinon, on doit récupérer la bonne valeur depuis le tableau "TabItems" <
                    // ( Rappel : on cherche à récupérer les élements de ...
                    // ... "Valeur_1;Valeur_2" dans l'expression ...
                    // ...  "< NomColonne:><:Valeur_1;Valeur_2 />"  ( EN SQL, on a ==> Valeur_1>NomColonne<Valeur_2) )
                    else
                    {
                        Item = (string)_DictParams[ApiPType.CS_TYP_DAT];

                        // > On envoie dans le tableau les elements séparés par des ";" <
                        //   ( Remarque "Split" => ChaineDecoupe du Wlangage )
                        TabItems = Item.Split(ApiPType.CS_SEPARATEUR_LIST_VALUE);

                        // > On ajoute à la requête SQL le bon élément <
                        _SqlWhere.Append(TabItems[(int)_NumItem]);
                    }
                }

                // > S'agit-il d'une CHAINE ? <
                else
                {
                    if (_DictParams.TryGetValue(ApiPType.CS_TYP_STR, out GetValue))
                    {
                        // > On insére des "'" uniquement pour les Opérateurs de comparaison ci  dessous <
                        if (_OpeComp == "==" || _OpeComp == "!=")
                        {
                            _SqlWhere.Append("'" + _DictParams[ApiPType.CS_TYP_STR] + "'");
                        }

                        // > Si il s'agit d'un opérateur de comparaison de type "[=", "=]" ou "[]" alors...
                        //   ...pas de "'" autour de la valeur <.
                        else
                        {
                            // > On récupère simplement la valeur <
                            if (_NumItem == null)
                            {
                                _SqlWhere.Append(_DictParams[ApiPType.CS_TYP_STR]);
                            }
                            // > Sinon, on doit récupérer la bonne valeur depuis le tableau "TabItems" <
                            // ( Rappel : on cherche à récupérer les élements de ...
                            // ... "Valeur_1;Valeur_2" dans l'expression ...
                            // ...  "< NomColonne:><:Valeur_1;Valeur_2 />"  ( EN SQL, on a ==> Valeur_1>NomColonne<Valeur_2) )
                            else
                            {
                                Item = (string)_DictParams[ApiPType.CS_TYP_STR];

                                // > On envoie dans le tableau les elements séparés par des ";" <
                                //   ( Remarque "Split" => ChaineDecoupe du Wlangage )
                                TabItems = Item.Split(ApiPType.CS_SEPARATEUR_LIST_VALUE);

                                // > On ajoute à la requête SQL le bon élément <
                                _SqlWhere.Append(TabItems[(int)_NumItem]);
                            }

                        }
                    }
                }
            }

            return _SqlWhere;

        }

        /// <summary>
        ///   Extraction Opérateur de Comparaison 
        /// </summary>
        /// <param name="_Param"></param>
        /// <returns></returns>
        private string extractOpeComp(string _Param)
        {
            // - - - - - - - - -
            //  Les opérateurs de comparaison peuvent être :
            //     "==" (égal à)  ou "!=" (différent de ) ou "[=" (commence par) ou...
            //     "=]" (fini par )  ou "[]"  (contient) ou ...
            //     "<" (inferieur à ) ou ">" (superieur à) ou ...
            //     "<=" ( inférieur égale) ou "=>" (supérieur égal)...
            //     ou "><" ( compris entre )
            // - - - - - - - - -

            // > Déclare un caractère pour analyse du ractère extrait <
            char ChOpComp;

            //  > Pour construction du code opération < 
            StringBuilder ItemOpe = new StringBuilder();

            // > Résultat final <
            string OpeComp;

            // > Recherche la position du premier ":" <
            // (exemple: < NOPPHN:==:5 /> ) 
            int PosIndex = _Param.IndexOf(ApiPType.CS_SEPARATEUR);


            // > Parcour sur 2 caractères  la chaine "_Param" <
            //  ( Si position de départ n'est pas négative ET si position...
            //    ...incrémentée PAS superieure à la longueur de la chaine ).
            if (PosIndex != -1 && PosIndex + 1 < _Param.Length)
            {
                // - - - - - - - - - - - - - - - - - - - 
                // > Analyse du premier caractère de l'opérateur de comparaison 
                // - - - - - - - - - - - - - - - - - - - 
                // > Si le premier caractère de l'opérateur de...
                //   ...comprason est un caractère de séparation ==> erreur 
                ChOpComp = _Param[PosIndex + 1];
                if (ChOpComp.ToString() == ApiPType.CS_SEPARATEUR)
                {
                    // > Erreur sur opérateur de comparaison <
                    OpeComp = "#ER-OPECOMP";
                }

                // - - - - - - - - - - - - - - - - - - - 
                // > Analyse du deuxième caractère de l'opérateur de comparaison
                // - - - - - - - - - - - - - - - - - - - 
                else
                {
                    // > Récupére le Premier caractère du Code opération <
                    ItemOpe.Append(_Param[PosIndex + 1]);

                    // > On récupère le deuxième caractère de l'opérateur de comparaison <
                    //   ( Si "ChOpComp" ne contient PAS ":", alors...
                    //     ... on récupère le deuxième caractère )
                    //   ( Dans le cas contraire, cela signifie que l'on a un opérateur...
                    //    de comparaison tel que : "<",">" => inférieur OU supérieur )
                    ChOpComp = _Param[PosIndex + 2];
                    if (ChOpComp.ToString() != ApiPType.CS_SEPARATEUR)
                    {
                        // > Récupére le deuxième caractère du Code opération <
                        ItemOpe.Append(_Param[PosIndex + 2]);
                    }

                    OpeComp = ItemOpe.ToString();

                }

            }
            else
            {
                // > Erreur sur opérateur de comparaison <
                OpeComp = "#ER-OPECOMP";
            }


            // > On retourne l'opérateur logique extrait <
            return OpeComp;
        }

        /// <summary>
        ///  Verification du contenu du paramétre en fonction du type 
        /// </summary>
        /// <param name="_TypeParam"></param>
        /// <param name="_ValueParam"></param
        /// <param name="_SecondSep"></param>
        /// <returns></returns>
        private Dictionary<string, object> ValueCheck(string _ValueParam)
        {
            // > Numéro Personne Physique <
            int NumP;

            // > Date Recherche  <
            DateTime DateR;

            // > Resultat TryParse <
            bool res;

            // > On renvoie ce dictionnaire <
            Dictionary<string, object> Params = new();

            // > Récupération position dans la liste des valeurs du séparateur de valeur <
            int StartIndex;

            // > Recherche la position du séparateur de valeurs  <
            // ( Exemple :
            //     1/ <DTFATG:><:20230101;20230601/> 
            //     2/ Dans ce cas, le séparateur de valeur est ";" ( voir "CS_SEPARATEUR_LIST_VALUE" )
            StartIndex = _ValueParam.IndexOf(ApiPType.CS_SEPARATEUR_LIST_VALUE) + ApiPType.CS_SEPARATEUR_LIST_VALUE.Length;

            // - - - - - - - - - - - - - -
            // > Aucun séparateur de valeur trouvée <
            // - - - - - - - - - - - - - -
            if (StartIndex == 0)
            {
                //********************************************************
                // > Lance l'analyse pour le paramètre  
                //   ( Il ne s'agit pas d'une liste : on envoie Null  ) 
                //********************************************************
                return ValueCheckAnalyse(_ValueParam, null);
            }

            // - - - - - - - - - - - - - -
            // > Un séparateur de valeur trouvée <
            // - - - - - - - - - - - - - -
            else
            {
                string ItemOfList;

                // > Extraction du premier element de la liste de valeur 
                //     1/ <DTFATG:><:20230101;20230601/> 
                //     2/ Dans ce cas, le séparateur de valeur est ";" ( voir "CS_SEPARATEUR_LIST_VALUE" )
                //                       Substring(DosDebut, longeur)
                //    ItemOfList = "20230101"

                //    Remarque :
                //        > On peut démarrer un substring à partir de 0 
                //        > "StartIndex" contient la position de la "virgule"...
                //          ...il faut retirer 1 à "StartIndex" pour ne pas prendre le ";".

                ItemOfList = _ValueParam.Substring(0, (StartIndex - 1));


                //********************************************************
                // > Lance l'analyse pour le premier Item de la liste 
                //   ( On passe la liste en paramètre ) 
                //********************************************************
                return ValueCheckAnalyse(ItemOfList, _ValueParam);


            }

        }

        /// <summary>
        ///  On détermine le type d'une valeur ou d'une liste de valeur 
        /// </summary>
        /// <param name="_ValueToCheck"></param>
        /// <param name="_ListValue"></param>
        /// <returns></returns>
        private Dictionary<string, object> ValueCheckAnalyse(string _ValueToCheck, string? _ListValue)
        {
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            // ##     ****  On veut savoir s il s'agit d'un entier, d'une date, d'un string  ****     ##
            // ## Si _ListValue est différent de null ...                                             ##
            // ## ... alors "_ValueToCheck" contient le premier Item pour analyse                     ## 
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            // > On renvoie ce dictionnaire <
            Dictionary<string, object> Params = new();

            // > Numéro Personne Physique <
            int NumP;

            // > Date Recherche  <
            DateTime DateR;

            // > Resultat TryParse <
            bool res;

            // - - - - - - - - - - -
            // > Démarre analyse Valeurs des paramètres selon type <
            // - - - - - - - - - - -
            //  !!! Ne peut pas utiliser de "switch...case" car...
            //      ... ne fonctionne qu'avec des constantes  au niveau du case !!!

            // - - - - - - - -
            // > Est-ce un numérique ? <
            // - - - - - - - -
            res = int.TryParse(_ValueToCheck, out NumP);
            if (res)
            {
                // > Récupère alors le numérique <
                //   "INT" ==> INTeger

                // > Si on ne reçoit qu'une valeur <
                if (_ListValue == null)
                {
                    Params[ApiPType.CS_TYP_INT] = NumP;
                }

                // > Si on reçoit une liste de valeurs ..
                // ... séparées par une ";" , alors c'est la liste...
                // ... que l'on met dans le dictionnaire <
                else
                {
                    Params[ApiPType.CS_TYP_INT] = _ListValue;
                }
            }
            else
            {


                // - - - - - - - -
                // >  s'agit-il alors  d'une date ?  <
                // - - - - - - - -
                if (DateTime.TryParseExact(_ValueToCheck, format, System.Globalization.CultureInfo.InvariantCulture,
                  System.Globalization.DateTimeStyles.None, out DateR))
                {
                    //   "DAT" ==> DATe 

                    // > Si on ne reçoit qu'une valeur <
                    if (_ListValue == null)
                    {
                        Params[ApiPType.CS_TYP_DAT] = DateR;
                    }

                    // > Si on reçoit une liste de valeurs ..
                    // ... séparées par une ";" , alors c'est la liste...
                    // ... que l'on met dans le dictionnaire <
                    else
                    {
                        Params[ApiPType.CS_TYP_DAT] = _ListValue;
                    }
                }
                else
                {

                    // - - - - - - - -
                    // > Il s'agit d'un string   <
                    // - - - - - - - -
                    //   "STR" ==> STRing

                    // > Si on ne reçoit qu'une valeur <
                    if (_ListValue == null)
                    {
                        Params[ApiPType.CS_TYP_STR] = _ValueToCheck;
                    }

                    // > Si on reçoit une liste de valeurs ..
                    // ... séparées par une ";" , alors c'est la liste...
                    // ... que l'on met dans le dictionnaire <
                    else
                    {
                        Params[ApiPType.CS_TYP_STR] = _ListValue;
                    }
                }
            }



            return Params;

        }
        #endregion

    }
}
