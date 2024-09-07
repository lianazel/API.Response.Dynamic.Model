

namespace API.Response.Dynamic.Model.Domain.Models
{
    public class TEST_JC_CATEGORY
    {
       public int Age { get; set; }

        // > Fonctionnement : 
        //   1/ Si "Age" est inférieur à 18, la condition est vraie et...
        //      ...la catégorie "Jeune" est renvoyée.

        //   2/ Si la condition précédente est fause et que "Age 
        //      ... est superieur ou égal à 18 et la condition 
        //      ... Age est inférieur à 65 est verifié, alors
        //      ... la catégorie "Adult" est renvoyée.

        //   3/  Si aucune des deux conditions précédentes n'est verifiée...
        //       ...alors la catégorie "Adult" est renvoyée.
        
        public string Category =>
              Age < 18 ? "Jeunes" :
              Age >= 18 && Age < 65 ? "Adult" : "Seniors" ;
       }

}
