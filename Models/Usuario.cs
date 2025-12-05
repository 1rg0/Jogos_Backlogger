namespace Jogos_Backlogger.Models
{
    public class Usuario
    {
        // Required
        public int id { get; set; }
        public required string nome { get; set; }
        public required DateOnly dataNascimento { get; set; }
        //public required ENUM genero
        public required string email { get; set; }
        public required string senhaHash { get; set; }
        public required bool ativo { get; set; }

        // Not Required
        public string? telefone { get; set; }
        public string? imagemPerfil { get; set; }
        public string? steamId { get; set; }
        public DateTime? steamIntegradoEm {  get; set; }
    }
}
