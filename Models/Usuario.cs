
namespace Jogos_Backlogger.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required GeneroUsuario Genero { get; set; }
        public required string Email { get; set; }
        public required string SenhaHash { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public string? Telefone { get; set; }
        public string? ImagemPerfil { get; set; }
        public string? SteamId { get; set; }
        public DateTime? SteamIntegradoEm { get; set; }
    }
    public enum GeneroUsuario
    {
        Masculino,
        Feminino,
        Outro,
        PrefiroNaoInformar
    }
}
