using Jogos_Backlogger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jogos_Backlogger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SteamController : ControllerBase
    {
        private readonly SteamService _steamService;

        public SteamController(SteamService steamService)
        {
            _steamService = steamService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Pesquisar(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
                return BadRequest("Digite pelo menos 3 caracteres.");

            var resultados = await _steamService.BuscarJogos(q);

            var resposta = resultados.Select(s => new
            {
                SteamId = s.id,
                Titulo = s.name,
                Capa = s.header_image ?? s.tiny_image
            });

            return Ok(resposta);
        }

        [HttpGet("library/{steamId}")]
        public async Task<IActionResult> GetLibrary(string steamId)
        {
            if (string.IsNullOrEmpty(steamId))
                return BadRequest("Steam ID inválido.");

            var jogos = await _steamService.GetUserLibrary(steamId);

            var resultado = jogos
                .Where(g => !string.IsNullOrEmpty(g.name))
                .OrderByDescending(g => g.playtime_forever)
                .Select(g => new
                {
                    SteamId = g.appid,
                    Titulo = g.name,
                    HorasJogadas = Math.Round(g.playtime_forever / 60.0, 1),
                    IconeUrl = $"http://media.steampowered.com/steamcommunity/public/images/apps/{g.appid}/{g.img_icon_url}.jpg"
                });

            return Ok(resultado);
        }
    }
}