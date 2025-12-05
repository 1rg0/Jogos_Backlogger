namespace Jogos_Backlogger.Models
{
    public class ItemBacklog
    {
        public int id { get; set; }
        public int jogoId { get; set; }
        public int usuarioId { get; set; }
        public int ordemId { get; set; }
        public required bool finalizado { get; set; }
        public required bool rejogando { get; set; }
        public required double horasJogadas { get; set; } = 0;
    }
}
