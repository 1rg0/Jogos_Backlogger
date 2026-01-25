using Jogos_Backlogger.Data;
using Jogos_Backlogger.Hubs;
using Jogos_Backlogger.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jogos_Backlogger.Services
{
    public class PythonApiResponse
    {
        [JsonPropertyName("horas")]
        public double Horas { get; set; }

        [JsonPropertyName("game_name")]
        public string GameName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class HltbService
    {
        private readonly HttpClient _httpClient;
        private readonly IHubContext<JogoHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public HltbService(
            HttpClient httpClient,
            IHubContext<JogoHub> hubContext,
            IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        public async Task<double> GetEstimativaHoras(string nomeJogo)
        {
            var (horas, _) = await ConsultarApiPython(nomeJogo);
            return horas;
        }

        public async Task AtualizarHorasBackground(int jogoId, string nomeJogo, int usuarioId)
        {
            try
            {
                var (horas, sucesso) = await ConsultarApiPython(nomeJogo);

                if (sucesso && horas > 0)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var jogo = await context.Jogos.FindAsync(jogoId);
                        if (jogo != null)
                        {
                            jogo.HorasParaZerar = (int)horas;

                            context.Entry(jogo).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                        }
                    }

                    await _hubContext.Clients.Group($"User_{usuarioId}").SendAsync("ReceberAtualizacaoHoras");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HLTB Background Error] {ex.Message}");
            }
        }

        private async Task<(double horas, bool sucesso)> ConsultarApiPython(string nomeJogo)
        {
            try
            {
                var urlPython = $"http://localhost:8000/estimativa?nome_jogo={Uri.EscapeDataString(nomeJogo)}";
                var response = await _httpClient.GetAsync(urlPython);

                if (!response.IsSuccessStatusCode) return (0, false);

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<PythonApiResponse>(json);

                if (dados != null && dados.Status == "sucesso")
                {
                    return (dados.Horas, true);
                }

                return (0, false);
            }
            catch
            {
                return (0, false);
            }
        }
    }
}