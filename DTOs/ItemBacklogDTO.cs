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
    }
}
