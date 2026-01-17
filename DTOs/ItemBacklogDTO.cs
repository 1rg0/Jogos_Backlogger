namespace Jogos_Backlogger.DTOs
{
    public class ItemBacklogDTO
    {
        public int Id { get; set; }
        public int JogoId { get; set; }
        public int UsuarioId { get; set; }
        public int OrdemId { get; set; }
        public bool Finalizado { get; set; } = false;
        public bool Rejogando { get; set; } = false;
        public double HorasJogadas { get; set; } = 0;
        public int VezesFinalizado { get; set; }

        public JogoDTO? Jogo { get; set; }
    }

    public class ItemBacklogDetailDTO
    {
        public int Id { get; set; }
        public int JogoId { get; set; }
        public int UsuarioId { get; set; }
        public int OrdemId { get; set; }
        public bool Finalizado { get; set; }
        public bool Rejogando { get; set; }
        public double HorasJogadas { get; set; }
        public int VezesFinalizado { get; set; }

        public JogoDetailDTO? Jogo { get; set; }
    }

    public class ItemBacklogCreateDTO
    {
        public int JogoId { get; set; }
        public int UsuarioId { get; set; }
        public int OrdemId { get; set; }
        public bool Finalizado { get; set; }
        public bool Rejogando { get; set; }
        public double HorasJogadas { get; set; }
        public int VezesFinalizado { get; set; } = 0;
    }

    public class ImportarSteamDTO
    {
        public int SteamId { get; set; }
        public int UsuarioId { get; set; }
        public double HorasJogadas { get; set; } = 0;
    }

    public class ImportarItemInfo
    {
        public int SteamId { get; set; }
        public double HorasJogadas { get; set; }
    }

    public class ImportarLoteDTO
    {
        public int UsuarioId { get; set; }
        public List<ImportarItemInfo> Jogos { get; set; }
    }
}
