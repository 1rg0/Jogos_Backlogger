using System.Text.Json.Serialization;

namespace Jogos_Backlogger.DTOs.Steam
{
    public class SteamAppWrapper
    {
        public bool success { get; set; }
        public SteamAppData data { get; set; }
    }

    public class SteamAppData
    {
        public string name { get; set; }
        public string detailed_description { get; set; }
        public string short_description { get; set; }
        public string header_image { get; set; }
        public string capsule_image { get; set; }
        public string website { get; set; }
        public List<string> developers { get; set; }
        public List<string> publishers { get; set; }
        public SteamReleaseDate release_date { get; set; }
        public List<SteamGenre> genres { get; set; }
    }

    public class SteamReleaseDate
    {
        public bool coming_soon { get; set; }
        public string date { get; set; }
    }

    public class SteamGenre
    {
        public string id { get; set; }
        public string description { get; set; }
    }
}