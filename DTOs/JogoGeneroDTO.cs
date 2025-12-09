using Jogos_Backlogger.Models;

namespace Jogos_Backlogger.DTOs
{
    public class JogoGeneroDTO
    {
        public int JogoId { get; set; }
        public int GeneroId { get; set; }
    }

    public class JogoGeneroDetailDTO
    {
        public int JogoId { get; set; }
        public int GeneroId { get; set; }
        public JogoDTO? Jogo { get; set; }
        public GeneroDTO? Genero { get; set; }
    }
}
