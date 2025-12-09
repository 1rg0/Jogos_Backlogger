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
    public class ItemBacklogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemBacklogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemBacklogDTO>>> ListarItensBacklog(int usuarioId)
        {
            var itensBacklog = await _context.ItemBacklogs
                .AsNoTracking()
                .Where(ib => ib.UsuarioId == usuarioId)
                .Include(ib => ib.Jogo)
                    .ThenInclude(jg => jg.JogoGeneros)
                        .ThenInclude(jg => jg.Genero)
                .OrderBy(ib => ib.OrdemId)
                .Select(ib => new ItemBacklogDTO
                {
                    Id = ib.Id,
                    JogoId = ib.JogoId,
                    UsuarioId = ib.UsuarioId,
                    OrdemId = ib.OrdemId,
                    Finalizado = ib.Finalizado,
                    Rejogando = ib.Rejogando,
                    HorasJogadas = ib.HorasJogadas,
                    Jogo = ib.Jogo == null ? null : new JogoDTO
                    {
                        Id = ib.Jogo.Id,
                        Titulo = ib.Jogo.Titulo,
                        Icone = ib.Jogo.Icone,
                        DataLancamento = ib.Jogo.DataLancamento,
                        Desenvolvedora = ib.Jogo.Desenvolvedora,
                        Distribuidora = ib.Jogo.Distribuidora,
                        HorasParaZerar = ib.Jogo.HorasParaZerar, 
                        Generos = ib.Jogo.JogoGeneros.Select(jg => jg.Genero.Nome).ToList()
                    }
                })
                .ToListAsync();

            return Ok(itensBacklog);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemBacklogDetailDTO>> DetalhesItemBacklog(int id)
        {
            var itemBacklog = await _context.ItemBacklogs
                .AsNoTracking()
                .Where(ib => ib.Id == id)
                .Include(ib => ib.Jogo)
                    .ThenInclude(jg => jg.JogoGeneros)
                        .ThenInclude(jg => jg.Genero)
                .Select(ib => new ItemBacklogDetailDTO
                {
                    Id = ib.Id,
                    JogoId = ib.JogoId,
                    UsuarioId = ib.UsuarioId,
                    OrdemId = ib.OrdemId,
                    Finalizado = ib.Finalizado,
                    Rejogando = ib.Rejogando,
                    HorasJogadas = ib.HorasJogadas,
                    Jogo = ib.Jogo == null ? null : new JogoDetailDTO
                    {
                        Id = ib.Jogo.Id,
                        Titulo = ib.Jogo.Titulo,
                        Icone = ib.Jogo.Icone,
                        DataLancamento = ib.Jogo.DataLancamento,
                        Desenvolvedora = ib.Jogo.Desenvolvedora,
                        Distribuidora = ib.Jogo.Distribuidora,
                        HorasParaZerar = ib.Jogo.HorasParaZerar,
                        Imagem = ib.Jogo.Imagem,
                        Sinopse = ib.Jogo.Sinopse, 
                        Generos = ib.Jogo.JogoGeneros.Select(jg => jg.Genero.Nome).ToList()
                    }
                })
                .FirstOrDefaultAsync();

            if (itemBacklog == null)
            {
                return NotFound();
            }

            return itemBacklog;
        }

        [HttpPost]
        public async Task<ActionResult<ItemBacklogDTO>> CriarItemBacklog(ItemBacklogCreateDTO itemBacklogDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jogoExiste = await _context.Jogos.AnyAsync(j => j.Id == itemBacklogDTO.JogoId);

            if (!jogoExiste)
            {
                return BadRequest($"Jogo não encontrado.");
            }

            var itemBacklog = new ItemBacklog
            {
                JogoId = itemBacklogDTO.JogoId,
                UsuarioId = 1,
                OrdemId = itemBacklogDTO.OrdemId,
                Finalizado = itemBacklogDTO.Finalizado,
                Rejogando = itemBacklogDTO.Rejogando,
                HorasJogadas = itemBacklogDTO.HorasJogadas
            };

            _context.ItemBacklogs.Add(itemBacklog);
            await _context.SaveChangesAsync();

            var dto = new ItemBacklogDTO
            {
                Id = itemBacklog.Id,
                JogoId = itemBacklog.JogoId,
                UsuarioId = itemBacklog.UsuarioId,
                OrdemId = itemBacklog.OrdemId,
                Finalizado = itemBacklog.Finalizado,
                Rejogando = itemBacklog.Rejogando,
                HorasJogadas = itemBacklog.HorasJogadas
            };

            return CreatedAtAction(nameof(CriarItemBacklog), new { id = itemBacklog.Id }, itemBacklogDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarItemBacklog(int id, ItemBacklogCreateDTO itemBacklogDTO)
        {
            var itemExistente = await _context.ItemBacklogs.FindAsync(id);

            if(itemExistente == null)
            {
                return NotFound();
            }

            itemExistente.OrdemId = itemBacklogDTO.OrdemId;
            itemExistente.Finalizado = itemBacklogDTO.Finalizado;
            itemExistente.Rejogando = itemBacklogDTO.Rejogando;
            itemExistente.HorasJogadas = itemBacklogDTO.HorasJogadas;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemBacklogExiste(id))
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
        public async Task<IActionResult> DeletarItemBacklog(int id)
        {
            var itemBacklog = await _context.ItemBacklogs.FindAsync(id);

            if (itemBacklog == null)
            {
                return NotFound();
            }

            _context.ItemBacklogs.Remove(itemBacklog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemBacklogExiste(int id)
        {
            return _context.ItemBacklogs.Any(e => e.Id == id);
        }
    }
}
