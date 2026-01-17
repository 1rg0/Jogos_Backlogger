using System.Text.Json;
using Jogos_Backlogger.DTOs.Steam;
using System.Text.RegularExpressions;

namespace Jogos_Backlogger.Services
{
    public class SteamService
    {
        private readonly HttpClient _httpClient;

        public SteamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SteamStoreItem>> BuscarJogos(string termo)
        {
            var url = $"https://store.steampowered.com/api/storesearch/?term={termo}&l=portuguese&cc=BR";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return new List<SteamStoreItem>();

            var jsonString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultado = JsonSerializer.Deserialize<SteamSearchResponse>(jsonString, options);

            return resultado?.items ?? new List<SteamStoreItem>();
        }

        public async Task<SteamAppData?> GetGameDetails(int steamAppId)
        {
            var url = $"https://store.steampowered.com/api/appdetails?appids={steamAppId}&l=portuguese&cc=BR";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var jsonString = await response.Content.ReadAsStringAsync();

            var dict = JsonSerializer.Deserialize<Dictionary<string, SteamAppWrapper>>(jsonString);

            if (dict != null && dict.ContainsKey(steamAppId.ToString()))
            {
                var wrapper = dict[steamAppId.ToString()];
                if (wrapper.success)
                {
                    return wrapper.data;
                }
            }

            return null;
        }

        public async Task<List<SteamOwnedGame>> GetUserLibrary(string steamId64)
        {
            string apiKey = "1757F6A4BCC8A9B8A89455447D108D26";

            var url = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={apiKey}&steamid={steamId64}&format=json&include_appinfo=1&include_played_free_games=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<SteamOwnedGame>();

                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<SteamLibraryResponse>(jsonString);

                return data?.response?.games ?? new List<SteamOwnedGame>();
            }
            catch
            {
                return new List<SteamOwnedGame>();
            }
        }
    }
}