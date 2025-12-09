using System.ComponentModel.DataAnnotations.Schema;

namespace Jogos_Backlogger.Models
{
    public class JogoGenero
    {
        public int JogoId { get; set; }
        public int GeneroId { get; set; }

        [ForeignKey("JogoId")]
        public Jogo? Jogo { get; set; }

        [ForeignKey("GeneroId")]
        public Genero? Genero { get; set; }
    }
}
