using Jogos_Backlogger.Models;

namespace Jogos_Backlogger.DTOs
{
    public class JogoGeneroDTO
    {
        public int JogoId { get; set; }
        public int GeneroId { get; set; }
    }

    public class JogoGeneroDetailDTO : JogoGeneroDTO
    {
        public Jogo? Jogo { get; set; }
        public Genero? Genero { get; set; }
    }
}
