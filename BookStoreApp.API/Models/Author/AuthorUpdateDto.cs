using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Author
{
    public class AuthorUpdateDto : BaseDto  
    {
        [Required]
        [MaxLength(50)]
        public String FirstName { get; set; }   

        [Required]
        [MaxLength(50)]
        public String LastName { get; set; } 

        [MaxLength(250)]
        public String Bio { get; set; }
    }
}
