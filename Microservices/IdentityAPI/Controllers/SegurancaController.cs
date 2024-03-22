using IdentityAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SegurancaController : ControllerBase
    {
        private IConfiguration _config;
        public SegurancaController(IConfiguration Configuration)
        {
            _config = Configuration;
        }
        [HttpPost]
        public IActionResult Login([FromBody] Usuario loginDetalhes)
        {
            bool resultado = ValidarUsuario(loginDetalhes);
            if (resultado)
            {

                var tokenString = GerarTokenJWT(VerificarRole(loginDetalhes));
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        private string GerarTokenJWT(List<Claim> claims)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, audience: audience,
                                             expires: DateTime.Now.AddMinutes(120), 
                                             signingCredentials: credentials,claims:claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }


        private bool ValidarUsuario(Usuario loginDetalhes)
        {
            if (loginDetalhes.NomeUsuario == "maacnet@teste.com" && loginDetalhes.Senha == "*senhaForte2024*")
            {

                return true;
            }
            
            if (loginDetalhes.NomeUsuario == "usuario@teste.com" && loginDetalhes.Senha == "*senhaForte2024*")
            {

                return true;
            }

            return false;

        }


        private List<Claim> VerificarRole(Usuario loginDetalhes)
        {

            if (loginDetalhes.NomeUsuario == "maacnet@teste.com")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "maacnet@teste.com"),
                    new Claim(ClaimTypes.Role, "InserirBancos"),
                    // Adicione outras claims conforme necessário
                };

                return claims;

            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "usuario@teste.com"),
                    new Claim(ClaimTypes.Role, "ConsultarBancos"),
                };

                return claims;
            }
           

        }

    }

}

