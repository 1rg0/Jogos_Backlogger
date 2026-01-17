using Microsoft.AspNetCore.Mvc;
using Jogos_Backlogger.Data;
using Jogos_Backlogger.Models;
using Microsoft.EntityFrameworkCore;
using Jogos_Backlogger.DTOs;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDTO>>> ListarJogos()
        {
            var jogosDTO = await _context.Jogos
                .AsNoTracking()
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .Select(j => new JogoDTO
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Icone = j.Icone,
                    DataLancamento = j.DataLancamento,
                    Desenvolvedora = j.Desenvolvedora,
                    Distribuidora = j.Distribuidora,
                    HorasParaZerar = j.HorasParaZerar,
                    Generos = j.JogoGeneros.Select(jg => jg.Genero.Nome).ToList()
                })
                .ToListAsync();

            return Ok(jogosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JogoDetailDTO>> DetalhesJogo(int id)
        {
            var jogo = await _context.Jogos
                .AsNoTracking()
                .Include(j => j.JogoGeneros)
                    .ThenInclude(jg => jg.Genero)
                .Where(j => j.Id == id)
                .Select(j => new JogoDetailDTO()
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Icone = j.Icone,
                    DataLancamento = j.DataLancamento,
                    Desenvolvedora = j.Desenvolvedora,
                    Distribuidora = j.Distribuidora,
                    HorasParaZerar = j.HorasParaZerar,
                    Imagem = j.Imagem,
                    Sinopse = j.Sinopse,
                    Generos = j.JogoGeneros.Select(jg => jg.Genero.Nome).ToList()
                })
                .FirstOrDefaultAsync();

            if (jogo == null)
            {
                return NotFound();
            }

            return Ok(jogo);
        }

        [HttpPost]
        public async Task<ActionResult<JogoDTO>> CriarJogo(JogoCreateDTO jogoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jogo = new Jogo()
            {
                Titulo = jogoDTO.Titulo,
                Icone = jogoDTO.Icone,
                DataLancamento = jogoDTO.DataLancamento,
                Desenvolvedora = jogoDTO.Desenvolvedora,
                Distribuidora = jogoDTO.Distribuidora,
                HorasParaZerar = jogoDTO.HorasParaZerar,
                Imagem = jogoDTO.Imagem,
                Sinopse = jogoDTO.Sinopse
            };

            _context.Jogos.Add(jogo);

            if (jogoDTO.GeneroIds != null && jogoDTO.GeneroIds.Any())
            {
                var generosExistentes = await _context.Generos
                    .Where(g => jogoDTO.GeneroIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genero in generosExistentes)
                {
                    var vinculo = new JogoGenero
                    {
                        Jogo = jogo,
                        Genero = genero
                    };
                    _context.JogoGeneros.Add(vinculo);
                }
            }

            await _context.SaveChangesAsync();

            var dto = new JogoDTO()
            {
                Id = jogo.Id,
                Titulo = jogoDTO.Titulo,
                Icone = jogoDTO.Icone,
                DataLancamento = jogoDTO.DataLancamento,
                Desenvolvedora = jogoDTO.Desenvolvedora,
                Distribuidora = jogoDTO.Distribuidora,
                HorasParaZerar = jogoDTO.HorasParaZerar,
                Generos = new List<string>()
            };

            return CreatedAtAction(nameof(DetalhesJogo), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarJogo(int id, JogoCreateDTO jogoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jogoExistente = await _context.Jogos
                .Include(j => j.JogoGeneros)
                .FirstOrDefaultAsync(j=> j.Id == id);

            if (jogoExistente == null)
            {
                return NotFound(); 
            }

            jogoExistente.Titulo = jogoDTO.Titulo;
            jogoExistente.DataLancamento = jogoDTO.DataLancamento;
            jogoExistente.Desenvolvedora = jogoDTO.Desenvolvedora;
            jogoExistente.Distribuidora = jogoDTO.Distribuidora;
            jogoExistente.HorasParaZerar = jogoDTO.HorasParaZerar;
            jogoExistente.Icone = jogoDTO.Icone;
            jogoExistente.Imagem = jogoDTO.Imagem;
            jogoExistente.Sinopse = jogoDTO.Sinopse;

            if (jogoExistente.JogoGeneros != null)
            {
                var generosParaRemover = jogoExistente.JogoGeneros
                    .Where(jg => !jogoDTO.GeneroIds.Contains(jg.GeneroId))
                    .ToList();

                foreach (var item in generosParaRemover)
                {
                    _context.JogoGeneros.Remove(item);
                }

                var idsAtuais = jogoExistente.JogoGeneros
                    .Select(jg => jg.GeneroId)
                    .ToList();

                var novosIds = jogoDTO.GeneroIds.Except(idsAtuais).ToList();

                foreach (var novoId in novosIds)
                {
                    if(await _context.Generos.AnyAsync(g => g.Id == novoId))
                    {
                        jogoExistente.JogoGeneros.Add(new JogoGenero
                        {
                            JogoId = id,
                            GeneroId = novoId
                        });
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JogoExiste(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarJogo(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);

            if (jogo == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JogoExiste(int id)
        {
            return _context.Jogos.Any(e => e.Id == id);
        }
    }
}
