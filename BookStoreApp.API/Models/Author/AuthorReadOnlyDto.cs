using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Author
{
    public class AuthorReadOnlyDto //Fundera nu på vad vi vill ha från authors.cs och lägga i denna.
    {

        public String FirstName { get; set; }   // Kan skriva "prop" så kommer visual studio att föreslå detta.

        public String LastName { get; set; } // ctrl + D för autocomplete

        public String Bio { get; set; }

    }
}
