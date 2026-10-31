using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiAutenticacaoJWT.Models;

namespace ApiAutenticacaoJWT.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel dados)
        {
            // Usuário e senha fixos
            if (dados.Usuario != "admin" || dados.Senha != "123456")
            {
                return Unauthorized("Usuário ou senha incorretos.");
            }

            // Criando as informações do usuário
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, dados.Usuario)
            };

            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credenciais = new SigningCredentials(
                chave,
                SecurityAlgorithms.HmacSha256
            );

            // Gerando o token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credenciais
            );

            var tokenGerado = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                mensagem = "Login realizado com sucesso!",
                token = tokenGerado
            });
        }
    }
}