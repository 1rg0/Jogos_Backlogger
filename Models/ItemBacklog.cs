using System.ComponentModel.DataAnnotations.Schema;

namespace Jogos_Backlogger.Models
{
    public class ItemBacklog
    {
        public int Id { get; set; }
        public int JogoId { get; set; }
        public int UsuarioId { get; set; }
        public int OrdemId { get; set; }
        public bool Finalizado { get; set; } = false;
        public bool Rejogando { get; set; } = false;
        public double HorasJogadas { get; set; } = 0;

        public int VezesFinalizado { get; set; } = 0;

        [ForeignKey("JogoId")]
        public virtual Jogo? Jogo { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
