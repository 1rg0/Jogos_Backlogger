using Microsoft.AspNetCore.Mvc;
using Jogos_Backlogger.Data;
using Jogos_Backlogger.Models;
using Microsoft.EntityFrameworkCore;
using Jogos_Backlogger.DTOs;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GeneroController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeneroDTO>>> ListarGeneros()
        {
            var generosDTO = await _context.Generos
                .AsNoTracking()
                .Select(g => new GeneroDTO
                {
                    Id = g.Id,
                    Nome = g.Nome
                })
                .ToListAsync();

            return Ok(generosDTO);
        }

        [HttpPost]
        public async Task<ActionResult<GeneroDTO>> CriarGenero(GeneroCreateDTO generoDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var genero = new Genero
            {
                Nome = generoDTO.Nome
            };

            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();

            var dto = new GeneroDTO
            {
                Id = genero.Id,
                Nome = genero.Nome
            };

            return CreatedAtAction(nameof(CriarGenero), new { id = dto.Id }, dto);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarGenero(int id, GeneroCreateDTO generoDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var generoExistente = await _context.Generos.FindAsync(id);

            if(generoExistente == null)
            {
                return NotFound();
            }

            generoExistente.Nome = generoDTO.Nome;

            try 
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeneroExiste(id))
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

        //[HttpDelete("{id}")]
        //public void DesativarGenero(int id)
        //{

        //}

        private bool GeneroExiste(int id)
        {
            return _context.Generos.Any(e => e.Id == id);
        }
    }
}
