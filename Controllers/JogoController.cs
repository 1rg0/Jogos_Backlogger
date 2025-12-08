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
        // Variável para a conexão com o banco de dados
        private readonly ApplicationDbContext _context;

        // Método construtor para injetar o contexto do banco de dados
        public JogoController(ApplicationDbContext context)
        {
            _context = context; // Guarda a conexão com o banco de dados na variável local
        }

        // GET: api/Jogo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoDTO>>> GetJogos()
        {
            // Retorna a lista de jogos do banco de dados
            var jogosDTO = await _context.Jogos
                .AsNoTracking() // Evita o rastreamento para melhorar a performance
                .Select(j => new JogoDTO // Transforma cada jogo em um objeto JogoDTO
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Icone = j.Icone,
                    DataLancamento = j.DataLancamento,
                    Desenvolvedora = j.Desenvolvedora,
                    Distribuidora = j.Distribuidora,
                    HorasParaZerar = j.HorasParaZerar
                })
                .ToListAsync(); // Executa a consulta de forma assíncrona e converte para lista

            return Ok(jogosDTO); // Retorna a lista de jogos (JSON) com status 200 OK
        }

        // GET: api/Jogo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JogoDetailDTO>> GetJogo(int id)
        {
            // Busca o jogo pelo ID no banco de dados
            var jogo = await _context.Jogos
                .AsNoTracking() // Evita o rastreamento para melhorar a performance
                .Where(j => j.Id == id) // Filtra pelo ID do jogo
                .Select(j => new JogoDetailDTO() // Transforma o jogo em um objeto JogoDetailDTO
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Icone = j.Icone,
                    DataLancamento = j.DataLancamento,
                    Desenvolvedora = j.Desenvolvedora,
                    Distribuidora = j.Distribuidora,
                    HorasParaZerar = j.HorasParaZerar,
                    Imagem = j.Imagem,
                    Sinopse = j.Sinopse
                })
                .FirstOrDefaultAsync(); // Executa a consulta de forma assíncrona e pega o primeiro resultado ou retorna nulo

            // Se o jogo não for encontrado, retorna 404 Not Found
            if (jogo == null)
            {
                return NotFound();
            }

            return Ok(jogo); // Retorna o jogo encontrado (JSON) com status 200 OK
        }

        // POST: api/Jogo
        [HttpPost]
        public async Task<ActionResult<JogoDTO>> CreateJogo(JogoCreateDTO jogoDTO)
        {
            // Valida se os dados enviados respeitam as regras (required etc) definidas no modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Cria um novo objeto Jogo com os dados recebidos
            var jogo = new Jogo()
            {
                // O Id será gerado automaticamente pelo banco de dados
                Titulo = jogoDTO.Titulo,
                Icone = jogoDTO.Icone,
                DataLancamento = jogoDTO.DataLancamento,
                Desenvolvedora = jogoDTO.Desenvolvedora,
                Distribuidora = jogoDTO.Distribuidora,
                HorasParaZerar = jogoDTO.HorasParaZerar,
                Imagem = jogoDTO.Imagem,
                Sinopse = jogoDTO.Sinopse
            };

            _context.Jogos.Add(jogo); // Adiciona o novo jogo ao contexto do banco de dados
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados de forma assíncrona, gerando o Id

            // Cria um objeto JogoDTO para retornar na resposta
            var dto = new JogoDTO()
            {
                Id = jogo.Id, // Usa o Id gerado pelo banco de dados
                Titulo = jogoDTO.Titulo,
                Icone = jogoDTO.Icone,
                DataLancamento = jogoDTO.DataLancamento,
                Desenvolvedora = jogoDTO.Desenvolvedora,
                Distribuidora = jogoDTO.Distribuidora,
                HorasParaZerar = jogoDTO.HorasParaZerar
            };

            // Retorna a resposta com status 201 Created, incluindo o local do novo recurso
            // O primeiro parâmetro gera a URL para acessar o novo jogo criado
            // O segundo parâmetro passa o objeto DTO como corpo da resposta
            // O terceiro parâmetro é o objeto JSON retornado
            return CreatedAtAction(nameof(GetJogo), new { id = dto.Id }, dto);
        }

        // PUT: api/Jogo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJogo(int id, JogoCreateDTO jogoDTO)
        {
            // Valida se os dados enviados respeitam as regras (required etc) definidas no modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca o jogo existente no banco de dados pelo ID
            // O EF Core rastreia o objeto retornado para detectar mudanças
            var jogoExistente = await _context.Jogos.FindAsync(id);

            // Retorna 404 Not Found se o jogo não for encontrado
            if (jogoExistente == null)
            {
                return NotFound(); 
            }

            // Atualiza manualmente os campos do jogo existente com os novos valores
            // O campo Id não é alterado
            jogoExistente.Titulo = jogoDTO.Titulo;
            jogoExistente.DataLancamento = jogoDTO.DataLancamento;
            jogoExistente.Desenvolvedora = jogoDTO.Desenvolvedora;
            jogoExistente.Distribuidora = jogoDTO.Distribuidora;
            jogoExistente.HorasParaZerar = jogoDTO.HorasParaZerar;
            jogoExistente.Icone = jogoDTO.Icone;
            jogoExistente.Imagem = jogoDTO.Imagem;
            jogoExistente.Sinopse = jogoDTO.Sinopse;

            try
            {
                // O EF Core rastreia as mudanças automaticamente
                // Ele gera o comando SQL UPDATE apenas para os campos que foram alterados
                // E salva as mudanças no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Tratamento de erro caso dois usuários tentem atualizar o mesmo registro ao mesmo tempo (raro mas acontece)
                if (!JogoExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent(); // Retorna 204 No Content indicando que a atualização foi bem-sucedida
        }

        // DELETE: api/Jogo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJogo(int id)
        {
            // Busca o jogo pelo ID no banco de dados
            var jogo = await _context.Jogos.FindAsync(id);

            // Retorna 404 Not Found se o jogo não for encontrado
            if (jogo == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(jogo); // Marca o jogo para remoção do banco de dados
            await _context.SaveChangesAsync(); // Executa a remoção no banco de dados de forma assíncrona

            return NoContent(); // Retorna 204 No Content indicando que a remoção foi bem-sucedida
        }

        // Método auxiliar privado para verificar se um jogo existe
        private bool JogoExiste(int id)
        {
            return _context.Jogos.Any(e => e.Id == id);
        }
    }
}
