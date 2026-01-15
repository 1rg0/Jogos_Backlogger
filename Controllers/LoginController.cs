using Jogos_Backlogger.Data;
using Jogos_Backlogger.DTOs;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Email == login.Email);

            if (admin != null)
            {
                if (BCrypt.Net.BCrypt.Verify(login.Senha, admin.SenhaHash))
                {
                    return Ok(new
                    {
                        Id = admin.Id,
                        Nome = "Administrador",
                        Email = admin.Email,
                        Tipo = "Admin"
                    });
                }
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (usuario != null)
            {
                if (BCrypt.Net.BCrypt.Verify(login.Senha, usuario.SenhaHash))
                {
                    return Ok(new
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Tipo = "Usuario"
                    });
                }
            }

            return Unauthorized("E-mail ou senha inválidos.");
        }
    }
}
