using AuthenticationPlugin;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _dbContext.Users.Where(u => u.Email == user.Email).FirstOrDefault();

            if(userWithSameEmail != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "User with same email already exists");
            }

            User userObj = new User 
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "Users"
            };
            _dbContext.Users.Add(userObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            var userObj = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

            if(userObj == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if(!SecurePasswordHasherHelper.Verify(user.Password, userObj.Password))
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userObj.Role),
            };
            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id = userObj.Id
            });
        }

    }
}
