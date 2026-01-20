namespace Jogos_Backlogger.DTOs
{
    public class JogoDTO
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public string? Icone { get; set; }
        public string? Imagem { get; set; }
        public required DateOnly DataLancamento { get; set; }
        public required string Desenvolvedora { get; set; }
        public required string Distribuidora { get; set; }
        public double HorasParaZerar { get; set; } = 0;
        public List<string> Generos { get; set; } = new List<string>();
    }

    public class JogoDetailDTO
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public string? Icone { get; set; }
        public required DateOnly DataLancamento { get; set; }
        public required string Desenvolvedora { get; set; }
        public required string Distribuidora { get; set; }
        public double HorasParaZerar { get; set; } = 0;
        public string? Imagem { get; set; }
        public string? Sinopse { get; set; }
        public List<string> Generos { get; set; } = new List<string>();
    }

    public class JogoCreateDTO
    {
        public required string Titulo { get; set; }
        public required DateOnly DataLancamento { get; set; }
        public required string Desenvolvedora { get; set; }
        public required string Distribuidora { get; set; }
        public double HorasParaZerar { get; set; } = 0;
        public string? Icone { get; set; }
        public string? Imagem { get; set; }
        public string? Sinopse { get; set; }
        public List<int> GeneroIds { get; set; } = new List<int>();
    }
}
