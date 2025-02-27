using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.User
{
    public class UserDto : LoginUserDto //Ska modellera API User
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        //// I loginUserDto
        //[Required]
        //[EmailAddress] //States that it should be evaluated as an email-adress.
        //public string Email { get; set; }

        //[Required]
        //public string Password { get; set; }

    }
}