using Common.Dtos;
using Common.Utils;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EstacionamientoAustral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly JwtUtils _jwtUtils;

        public AuthenticateController(IUserRepository userRepository, IConfiguration configuration, JwtUtils jwtUtils)
        {
            _userRepository = userRepository;
            _config = configuration;
            _jwtUtils = jwtUtils;
        }

        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthRequestDto credentials)
        {
            User? userAuthenticated = _userRepository.Authenticate(credentials.Username, credentials.Password);
            if (userAuthenticated is not null)
            {
                var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));
                SigningCredentials signature = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

                var claimsForToken = new List<Claim>
                {
                    new Claim("sub", userAuthenticated.Id.ToString()),
                    new Claim("given_name", userAuthenticated.Username)
                };

                var jwtSecurityToken = new JwtSecurityToken(
                    _config["Authentication:Issuer"],
                    _config["Authentication:Audience"],
                    claimsForToken,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    signature);

                string tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                return Ok(tokenToReturn);
            }
            return Unauthorized();
        }
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenDto tokenDto)
        {
            var principal = _jwtUtils.GetPrincipalFromExpiredToken(tokenDto.Token);
            if (principal == null)
            {
                return Unauthorized("Token inválido.");
            }

            var newToken = _jwtUtils.GenerateToken(principal.Claims);
            return Ok(new { token = newToken });
        }

    }
}
