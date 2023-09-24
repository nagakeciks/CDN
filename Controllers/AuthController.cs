using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HafizDemoAPI;
using HafizDemoAPI.ModelCDN;
using Microsoft.Identity.Client;

namespace HafizDemoAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;
        public AuthController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }


        public class LoginData
        {
            public string Username { get; set; }
            public string UserID { get; set; }
            public string Token { get; set; }
        }

        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                PubFunc pubFunc = new();
                CDNContext cdn = new();
                var user = cdn.Users.Where(u => u.Username == model.Username).FirstOrDefault();

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username" });
                }
                var boolValid = pubFunc.VerifyPassword(model.Password, user.Password, Convert.FromBase64String(user.Salt));
                if (boolValid)
                {
                    // Generate JWT token
                    var token = GenerateJwtToken(user.UserId, user.Username);

                    return Ok(new LoginData { Username = user.Username, UserID = user.UserId.ToString(), Token = token });
                }
                else
                {
                    return Unauthorized(new { message = "Invalid password" });
                }
            }
            catch(Exception ex) {

                return Ok(ex.Message);
            }

        }


        private string GenerateJwtToken(Int32 UserId,string UserName)
        {
            string? _jwtSecret = configuration.GetValue<string>("Jwt:Key"); 
            string? _jwtIssuer = configuration.GetValue<string>("Jwt:Issuer");     
            string? _jwtAudience = configuration.GetValue<string>("Jwt:Audience"); 

        var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", UserId.ToString()),
                    new Claim(ClaimTypes.Name, UserName), 
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
