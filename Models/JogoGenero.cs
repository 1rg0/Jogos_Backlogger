namespace Jogos_Backlogger.Models
{
    public class JogoGenero
    {
        public int jogoId { get; set; }
        public int generoId { get; set; }

        public Jogo? Jogo { get; set; }
        public Genero? Genero { get; set; }
    }
}
