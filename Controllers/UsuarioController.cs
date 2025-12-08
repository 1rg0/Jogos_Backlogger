using Microsoft.AspNetCore.Mvc;
using Jogos_Backlogger.Data;
using Jogos_Backlogger.Models;
using Microsoft.EntityFrameworkCore;
using Jogos_Backlogger.DTOs;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuariosDTO = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    DataNascimento = u.DataNascimento,
                    Genero = u.Genero,
                    Email = u.Email,
                    SenhaHash = u.SenhaHash,
                    Ativo = u.Ativo
                })
                .ToListAsync();

            return Ok(usuariosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDetailDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDetailDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    DataNascimento = u.DataNascimento,
                    Genero = u.Genero,
                    Email = u.Email,
                    SenhaHash = u.SenhaHash,
                    Ativo = u.Ativo,
                    Telefone = u.Telefone,
                    ImagemPerfil = u.ImagemPerfil,
                    SteamId = u.SteamId,
                    SteamIntegradoEm = u.SteamIntegradoEm
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> CreateUsuario(UsuarioCreateDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                DataNascimento = usuarioDTO.DataNascimento,
                Genero = usuarioDTO.Genero,
                Email = usuarioDTO.Email,
                SenhaHash = usuarioDTO.SenhaHash,
                Telefone = usuarioDTO.Telefone,
                ImagemPerfil = usuarioDTO.ImagemPerfil,
                SteamId = usuarioDTO.SteamId,
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var dto = new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                DataNascimento = usuario.DataNascimento,
                Genero = usuario.Genero,
                Email = usuario.Email,
                SenhaHash = usuario.SenhaHash,
                Ativo = usuario.Ativo
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioCreateDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.Nome = usuarioDTO.Nome;
            usuarioExistente.DataNascimento = usuarioDTO.DataNascimento;
            usuarioExistente.Genero = usuarioDTO.Genero;
            usuarioExistente.Email = usuarioDTO.Email;
            usuarioExistente.SenhaHash = usuarioDTO.SenhaHash;
            usuarioExistente.Telefone = usuarioDTO.Telefone;
            usuarioExistente.ImagemPerfil = usuarioDTO.ImagemPerfil;
            usuarioExistente.SteamId = usuarioDTO.SteamId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExiste(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
