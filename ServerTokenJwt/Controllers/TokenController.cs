using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerTokenJwt.Configuration;
using ServerTokenJwt.ModelsAPI;
using ServerTokenJwt.ModelsDB.Contexts;
using ServerTokenJwt.Repositorio;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServerTokenJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly JwtConfiguracion _jwtConfig;
        private readonly ITokenJWTESRepositorio _repositorySeguridad;

        public TokenController(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _repositorySeguridad = new TokenJWTESRepositorio(optionsMonitor, context);
        }

        [HttpGet]
        [Route("[action]")]
        public string Test()
        {
            return "Server Autenticación TokenJWT en linea!";
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] UsuarioModel user)
        {
            string mensaje = "Credenciales Inválidas!";
            try
            {
                var isCorrect = await _repositorySeguridad.Login(user);

                if (isCorrect == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        errors = new List<string>() {
                                "Acceso Denegado!"
                            },
                        success = false
                    });
                }

                var jwtToken = GenerateJwtToken(isCorrect);

                if (!string.IsNullOrWhiteSpace(jwtToken))
                {
                    return Ok(new RegistrationResponse()
                    {
                        success = true,
                        errors = new List<string>() { "0" },
                        token = jwtToken
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new RegistrationResponse()
                {
                    errors = new List<string>() { e.Message },
                    success = false
                });
            }
            return BadRequest(new RegistrationResponse()
            {
                errors = new List<string>() { mensaje },
                success = false
            });
        }

        private string GenerateJwtToken(UsuarioModel user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.secret);

            var audience = _jwtConfig.audience;
            var stringAudience = audience.Replace(" ", "");
            string[] multiAudience = stringAudience.Split(',');

            var stringRoles = user.roles.Replace(" ", "");
            string[] roles = stringRoles.Split(',');

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.id.ToString()),
                    new Claim("FullName", user.fullName),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.usuario),
                    new Claim(JwtRegisteredClaimNames.Email, "bello90033@gmail.com"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = multiAudience[0],
                Audience = multiAudience[0],
            };

            //foreach (var item in multiAudience)
            //{
            //    tokenDescriptor.Subject.AddClaim(new Claim(JwtRegisteredClaimNames.Aud, item));
            //}

            foreach (var item in roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim("Role", item));
            }

            var token = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
