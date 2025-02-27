using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.User
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress] //States that it should be evaluated as an email-adress.
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }


}