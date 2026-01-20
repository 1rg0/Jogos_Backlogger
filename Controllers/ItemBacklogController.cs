using Jogos_Backlogger.Data;
using Jogos_Backlogger.DTOs;
using Jogos_Backlogger.Models;
using Jogos_Backlogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemBacklogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SteamService _steamService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ItemBacklogController(
            ApplicationDbContext context,
            SteamService steamService,
            HltbService hltbService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _steamService = steamService;
            _serviceScopeFactory = serviceScopeFactory;
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
                    VezesFinalizado = ib.VezesFinalizado,
                    Jogo = ib.Jogo == null ? null : new JogoDTO
                    {
                        Id = ib.Jogo.Id,
                        Titulo = ib.Jogo.Titulo,
                        Icone = ib.Jogo.Icone,
                        Imagem = ib.Jogo.Imagem,
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
                    VezesFinalizado = ib.VezesFinalizado,
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var jogoExiste = await _context.Jogos.AnyAsync(j => j.Id == itemBacklogDTO.JogoId);

            if (!jogoExiste) return BadRequest($"Jogo não encontrado.");

            var maiorOrdemAtual = await _context.ItemBacklogs
                .Where(i => i.UsuarioId == itemBacklogDTO.UsuarioId)
                .MaxAsync(i => (int?)i.OrdemId) ?? 0;

            var itemBacklog = new ItemBacklog
            {
                JogoId = itemBacklogDTO.JogoId,
                UsuarioId = itemBacklogDTO.UsuarioId,
                OrdemId = maiorOrdemAtual + 1,
                Finalizado = itemBacklogDTO.Finalizado,
                Rejogando = itemBacklogDTO.Rejogando,
                HorasJogadas = itemBacklogDTO.HorasJogadas,
                VezesFinalizado = itemBacklogDTO.VezesFinalizado
            };

            _context.ItemBacklogs.Add(itemBacklog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CriarItemBacklog), new { id = itemBacklog.Id }, itemBacklogDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarItemBacklog(int id, ItemBacklogCreateDTO itemBacklogDTO)
        {
            var itemExistente = await _context.ItemBacklogs.FindAsync(id);
            if (itemExistente == null) return NotFound();

            itemExistente.OrdemId = itemBacklogDTO.OrdemId;
            itemExistente.Finalizado = itemBacklogDTO.Finalizado;
            itemExistente.Rejogando = itemBacklogDTO.Rejogando;
            itemExistente.HorasJogadas = itemBacklogDTO.HorasJogadas;
            itemExistente.VezesFinalizado = itemBacklogDTO.VezesFinalizado;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemBacklogExiste(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarItemBacklog(int id)
        {
            var itemBacklog = await _context.ItemBacklogs.FindAsync(id);
            if (itemBacklog == null) return NotFound();

            _context.ItemBacklogs.Remove(itemBacklog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("reordenar")]
        public async Task<IActionResult> ReordenarItens([FromBody] ReordenarItemDTO dto)
        {
            var itensNoBanco = await _context.ItemBacklogs
                .Where(i => dto.ListaIds.Contains(i.Id))
                .ToListAsync();

            for (int i = 0; i < dto.ListaIds.Count; i++)
            {
                var idAtual = dto.ListaIds[i];
                var item = itensNoBanco.FirstOrDefault(x => x.Id == idAtual);

                if (item != null) item.OrdemId = i + 1;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("importar-steam")]
        public async Task<IActionResult> ImportarDaSteam([FromBody] ImportarSteamDTO dto)
        {
            var jogoLocal = await _context.Jogos
                .Include(j => j.JogoGeneros)
                .FirstOrDefaultAsync(j => j.SteamId == dto.SteamId);

            bool precisaBuscarHoras = false;

            if (jogoLocal == null)
            {
                var detalhesSteam = await _steamService.GetGameDetails(dto.SteamId);
                if (detalhesSteam == null) return NotFound("Jogo não encontrado na Steam.");

                double horasEstimadas = 0;
                precisaBuscarHoras = true;

                var sinopseLimpa = Regex.Replace(detalhesSteam.short_description ?? "", "<.*?>", string.Empty);

                DateTime dataTemp;
                DateTime.TryParse(detalhesSteam.release_date?.date, out dataTemp);
                DateOnly dataLancamento = DateOnly.FromDateTime(dataTemp);

                jogoLocal = new Jogo
                {
                    SteamId = dto.SteamId,
                    Titulo = detalhesSteam.name,
                    Sinopse = sinopseLimpa,
                    Icone = detalhesSteam.capsule_image,
                    Imagem = detalhesSteam.header_image,
                    Desenvolvedora = detalhesSteam.developers?.FirstOrDefault() ?? "Desconhecida",
                    Distribuidora = detalhesSteam.publishers?.FirstOrDefault() ?? "",
                    DataLancamento = dataLancamento,
                    HorasParaZerar = horasEstimadas,
                    JogoGeneros = new List<JogoGenero>()
                };

                if (detalhesSteam.genres != null)
                {
                    var todosGenerosLocais = await _context.Generos.ToListAsync();
                    foreach (var gSteam in detalhesSteam.genres)
                    {
                        var generoCorrespondente = todosGenerosLocais
                            .FirstOrDefault(gl => gl.Nome.ToLower().Contains(gSteam.description.ToLower())
                                               || gSteam.description.ToLower().Contains(gl.Nome.ToLower()));

                        if (generoCorrespondente != null)
                        {
                            jogoLocal.JogoGeneros.Add(new JogoGenero
                            {
                                Genero = generoCorrespondente
                            });
                        }
                    }
                }

                _context.Jogos.Add(jogoLocal);
                await _context.SaveChangesAsync();
            }

            var jaNoBacklog = await _context.ItemBacklogs
                .AnyAsync(i => i.UsuarioId == dto.UsuarioId && i.JogoId == jogoLocal.Id);

            if (jaNoBacklog) return BadRequest("Este jogo já está no seu backlog.");

            var maiorOrdemAtual = await _context.ItemBacklogs
                .Where(i => i.UsuarioId == dto.UsuarioId)
                .MaxAsync(i => (int?)i.OrdemId) ?? 0;

            var novoItem = new ItemBacklog
            {
                UsuarioId = dto.UsuarioId,
                JogoId = jogoLocal.Id,
                OrdemId = maiorOrdemAtual + 1,
                Finalizado = false,
                Rejogando = false,
                HorasJogadas = dto.HorasJogadas,
                VezesFinalizado = 0
            };

            _context.ItemBacklogs.Add(novoItem);
            await _context.SaveChangesAsync();

            if (precisaBuscarHoras)
            {
                int jogoIdParaAtualizar = jogoLocal.Id;
                string nomeParaBuscar = jogoLocal.Titulo;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            var hltbService = scope.ServiceProvider.GetRequiredService<HltbService>();

                            var horas = await hltbService.GetEstimativaHoras(nomeParaBuscar);

                            if (horas > 0)
                            {
                                var jogoDb = await dbContext.Jogos.FindAsync(jogoIdParaAtualizar);
                                if (jogoDb != null)
                                {
                                    jogoDb.HorasParaZerar = horas;
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                Console.WriteLine($"HLTB retornou 0h para {nomeParaBuscar}. Mantido original.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Falha ao atualizar horas: {ex.Message}");
                    }
                });
            }

            var listaNomesGeneros = jogoLocal.JogoGeneros != null
                ? jogoLocal.JogoGeneros.Select(jg => jg.Genero!.Nome).ToList()
                : new List<string>();

            var dtoRetorno = new ItemBacklogDTO
            {
                Id = novoItem.Id,
                UsuarioId = novoItem.UsuarioId,
                JogoId = novoItem.JogoId,
                OrdemId = novoItem.OrdemId,
                Finalizado = novoItem.Finalizado,
                Rejogando = novoItem.Rejogando,
                HorasJogadas = novoItem.HorasJogadas,
                VezesFinalizado = novoItem.VezesFinalizado,
                Jogo = new JogoDTO
                {
                    Id = jogoLocal.Id,
                    Titulo = jogoLocal.Titulo,
                    Icone = jogoLocal.Icone,
                    Imagem = jogoLocal.Imagem,
                    DataLancamento = jogoLocal.DataLancamento,
                    Desenvolvedora = jogoLocal.Desenvolvedora,
                    Distribuidora = jogoLocal.Distribuidora,
                    HorasParaZerar = 0,
                    Generos = listaNomesGeneros
                }
            };

            return CreatedAtAction(nameof(DetalhesItemBacklog), new { id = novoItem.Id }, dtoRetorno);
        }

        [HttpPost("importar-lote")]
        public async Task<IActionResult> ImportarLote([FromBody] ImportarLoteDTO dto)
        {
            int sucessos = 0;

            foreach (var itemInfo in dto.Jogos)
            {
                var dtoIndividual = new ImportarSteamDTO
                {
                    UsuarioId = dto.UsuarioId,
                    SteamId = itemInfo.SteamId,
                    HorasJogadas = itemInfo.HorasJogadas
                };

                var result = await ImportarDaSteam(dtoIndividual);

                if (result is CreatedAtActionResult || result is CreatedResult)
                {
                    sucessos++;
                }
            }

            return Ok(new { message = $"{sucessos} jogos importados. As horas serão atualizadas em breve." });
        }

        [HttpPost("sincronizar-horas-steam")]
        public async Task<IActionResult> SincronizarHorasSteam([FromQuery] int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null || string.IsNullOrEmpty(usuario.SteamId))
            {
                return BadRequest("Usuário não encontrado ou sem Steam ID vinculado.");
            }

            var jogosSteam = await _steamService.GetUserLibrary(usuario.SteamId);

            if (jogosSteam == null || !jogosSteam.Any())
            {
                return Ok(new { message = "Nenhum jogo encontrado na Steam para sincronizar." });
            }

            var itensBacklog = await _context.ItemBacklogs
                .Include(i => i.Jogo)
                .Where(i => i.UsuarioId == usuarioId && i.Jogo.SteamId != null)
                .ToListAsync();

            int atualizados = 0;

            foreach (var item in itensBacklog)
            {
                var jogoNaSteam = jogosSteam.FirstOrDefault(js => js.appid == item.Jogo.SteamId);

                if (jogoNaSteam != null)
                {
                    double horasReais = Math.Round(jogoNaSteam.playtime_forever / 60.0, 1);

                    if (Math.Abs(item.HorasJogadas - horasReais) > 0.1)
                    {
                        item.HorasJogadas = horasReais;
                        atualizados++;
                    }
                }
            }

            if (atualizados > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Sincronização concluída. {atualizados} jogos atualizados.", atualizados });
        }

        private bool ItemBacklogExiste(int id)
        {
            return _context.ItemBacklogs.Any(e => e.Id == id);
        }
    }
}