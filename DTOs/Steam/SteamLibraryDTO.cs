namespace Jogos_Backlogger.DTOs.Steam
{
    public class SteamLibraryResponse
    {
        public SteamLibraryResponseData response { get; set; }
    }

    public class SteamLibraryResponseData
    {
        public int game_count { get; set; }
        public List<SteamOwnedGame> games { get; set; }
    }

    public class SteamOwnedGame
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
    }
}