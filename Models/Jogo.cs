namespace Jogos_Backlogger.Models
{
    public class Jogo
    {
        // Required
        public int id { get; set; }
        public required string titulo { get; set; }
        public required DateOnly dataLancamento { get; set; }
        public required string desenvolvedora { get; set; }
        public required string distribuidora { get; set; }
        public required double horasParaZerar { get; set; }

        // Not Required
        public string? imagem { get; set; }
        public string? icone { get; set; }
        public string? sinopse { get; set; }

    }
}
