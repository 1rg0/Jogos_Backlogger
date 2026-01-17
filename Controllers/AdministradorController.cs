using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jogos_Backlogger.Data;
using Jogos_Backlogger.Models;
using Jogos_Backlogger.DTOs;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdministradorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministradorDTO>>> ListarAdministradores()
        {
            var administradoresDTO = await _context.Administradores
                .AsNoTracking()
                .Select(a => new AdministradorDTO
                {
                    Id = a.Id,
                    Email = a.Email
                })
                .ToListAsync();

            return Ok(administradoresDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdministradorDTO>> DetalhesAdministrador(int id)
        {
            var administrador = await _context.Administradores
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AdministradorDTO
                {
                    Id = a.Id,
                    Email = a.Email
                })
                .FirstOrDefaultAsync();

            if (administrador == null)
            {
                return NotFound();
            }

            return Ok(administrador);
        }

        [HttpPost]
        public async Task<ActionResult<AdministradorDTO>> CriarAdministrador(AdministradorCreateDTO administradorDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(administradorDTO.Senha);

            var administrador = new Administrador
            {
                Email = administradorDTO.Email,
                SenhaHash = senhaHash
            };

            _context.Administradores.Add(administrador);
            await _context.SaveChangesAsync();

            var dto = new AdministradorDTO
            {
                Id = administrador.Id,
                Email = administrador.Email
            };

            return CreatedAtAction(nameof(DetalhesAdministrador), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAdministrador(int id, AdministradorCreateDTO administrador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var administradorExistente = await _context.Administradores.FindAsync(id);

            if (administradorExistente == null)
            {
                return NotFound();
            }

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(administrador.Senha);

            administradorExistente.Email = administrador.Email;
            administradorExistente.SenhaHash = senhaHash;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministradorExiste(id))
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

        private bool AdministradorExiste(int id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
