namespace Jogos_Backlogger.DTOs.Steam
{
    public class SteamStoreItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tiny_image { get; set; }
        public string header_image { get; set; }
    }

    public class SteamSearchResponse
    {
        public List<SteamStoreItem> items { get; set; }
        public int total { get; set; }
    }
}