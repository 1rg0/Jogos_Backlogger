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
    }

    public class HltbService
    {
        private readonly HttpClient _httpClient;

        public HltbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> GetEstimativaHoras(string nomeJogo)
        {
            try
            {
                // URL da API Python rodando localmente
                // Se eu publicar, preciso mudar para o IP do servidor onde o Python estiver
                var urlPython = $"http://localhost:8000/estimativa?nome_jogo={Uri.EscapeDataString(nomeJogo)}";

                var response = await _httpClient.GetAsync(urlPython);

                if (!response.IsSuccessStatusCode) return 0;

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<PythonApiResponse>(json);

                if (dados != null)
                {
                    if (dados.Horas > 0)
                    {
                        Console.WriteLine($"[PYTHON-API] Match: {dados.GameName} -> {dados.Horas}h");
                    }
                    return dados.Horas;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO DE CONEXÃO PYTHON] Certifique-se que o script main.py está rodando. Erro: {ex.Message}");
                return 0;
            }
        }
    }
}