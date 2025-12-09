namespace Jogos_Backlogger.Models
{
    public class Jogo
    {
        // Required
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public required DateOnly DataLancamento { get; set; }
        public required string Desenvolvedora { get; set; }
        public required string Distribuidora { get; set; }
        public double HorasParaZerar { get; set; } = 0;

        // Not Required
        public string? Imagem { get; set; }
        public string? Icone { get; set; }
        public string? Sinopse { get; set; }

        public virtual ICollection<JogoGenero> JogoGeneros { get; set; }

    }
}
