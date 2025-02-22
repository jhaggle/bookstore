using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Author
{
    public class AuthorCreateDto
    {

        // lägger till sånt här så behöver inte komma till databas-sidan för reject eller accept. Kan göra det på klientsidan då.
        // Och de är inte heller required på databaselevel - se author.cs - men doesnt make sense att lägga till en author utan name
        // så vi kan ju göra det här på api-nivån.
        [Required] 
        [MaxLength(50)]
        public String FirstName { get; set; }   // Kan skriva "prop" så kommer visual studio att föreslå detta.

        [Required]
        [MaxLength(50)]
        public String LastName { get; set; } // ctrl + D för autocomplete

        [MaxLength(250)]
        public String Bio { get; set; }

    }
}
