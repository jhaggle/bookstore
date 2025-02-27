using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.User;
using BookStoreApp.API.Static;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApiUser> usermanager;
        private readonly IConfiguration configuration;


        //IConfiguration is a built-in interface in ASP.NET Core that gives access to configuration settings from:
        //    appsettings.json
        //    secrets.json(for storing sensitive data)
        //    Environment variables
        //    Command-line arguments
        //It allows you to read values without manually opening JSON files or dealing with raw config files.
        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.usermanager = userManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("register")] // api/controller/register

        public async Task<IActionResult> Register(UserDto userDto)
        {

            try
            {
                if (userDto == null)
                {
                    return BadRequest("Insufficient data Provided");
                }

                //// Altnerativ 1) Could do this and assign every field manually... 
                //var user = new ApiUser
                //{
                //    FirstName = userDto.FirstName,
                //    LastName = userDto.LastName,
                //    Email = userDto.Email,
                //    UserName = userDto.Email
                //};
                //// Hash password manually (without UserManager)
                //var hasher = new PasswordHasher<ApiUser>();
                //user.PasswordHash = hasher.HashPassword(user, userDto.Password);
                //// Save to database using DbContext (instead of using UserManager)
                //_context.Users.Add(user);
                //await _context.SaveChangesAsync();



                // Alternativ 2, bara TRE rader. Det är om vi använder automapper.
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                // Need to use the userDto because ApiUser doesnt not have any password field
                var result = await usermanager.CreateAsync(user, userDto.Password);



                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await usermanager.AddToRoleAsync(user, userDto.Role);
                return Accepted();
            }
            catch (Exception ex)
            {

                logger.LogError($"Something wrong in {nameof(Register)}");
                return Problem($"The error was of type {ex}", statusCode: 500);
            }

        }

        [HttpPost]
        [Route("login")] // api/controller/login
        //public async Task<IActionResult> Login(LoginUserDto userDto)
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            //We get the credentials from the user, now we need to use the usermanager to determine if this user is valid or not

            try
            {
    
                // 1) Find user by e-mail
                var user = await usermanager.FindByEmailAsync(userDto.Email);

                // 2) Check if password is correct
                var passwordValid = await usermanager.CheckPasswordAsync(user, userDto.Password);

                if (user == null || !passwordValid)
                {
                    //return NotFound("User not found, or credentials invalid");
                    return Unauthorized(userDto);
                }

                string tokenString = await GenerateToken(user);

                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = tokenString,
                    UserId = user.Id
                };

                return Accepted(response); //The request was successful, but no content is returned.
            }
            catch (Exception ex)
            {

                logger.LogError(ex, $"Something wrong in {nameof(Login)}");
                return Problem($"Something wrong in Login", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //Now we need to set up your claims. 
            //Now, claims are bits of information that will let the system know who you are, what you can do, etcetera.
            //So like your name, that's going to be a claim, your role.
            //That's going to be a claim.

            var roles = await usermanager.GetRolesAsync(user);    
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q));

            var userClaims = await usermanager.GetClaimsAsync(user);   // Om vi också vill ha med claims från databasen.

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Here the username is my subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // some replay-key that helps prevent replay-attacks
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            //Nu dags att actually skapa token
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                signingCredentials : credentials
                );

            //This line serializes the JwtSecurityToken into a compact string
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }


}

