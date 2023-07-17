using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JupiterSecurity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpPost]
        [Route("staticLogin")]
        public ActionResult<string> Login(LoginDTO credintials)
        {
            if (credintials.username == "admin" && credintials.password == "password")
            {
                var userClaims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, credintials.username),
                    new Claim(ClaimTypes.Email, $"{credintials.username}@gmail.com"),
                    new Claim("Nationality", "EGY"),
                };

                var secretKey = _configuration.GetValue<string>("SecretKey");
                var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);
                var key = new SymmetricSecurityKey(secretKeyBytes);
                var methodGeneratingToken = new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);
                var jwt = new JwtSecurityToken(
                  claims: userClaims,
                  notBefore: DateTime.Now,
                  expires: DateTime.Now.AddMinutes(15),
                  signingCredentials: methodGeneratingToken);

                var tokenHandler = new JwtSecurityTokenHandler();
                string tokenString = tokenHandler.WriteToken(jwt);
                return Ok(tokenString);
            }
            return Unauthorized("Wrong");
        }
    }
}
