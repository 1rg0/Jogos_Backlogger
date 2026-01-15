using Jogos_Backlogger.Models;

namespace Jogos_Backlogger.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required GeneroUsuario Genero { get; set; }
        public required string Email { get; set; }
        public bool Ativo { get; set; } = true;
    }

    public class UsuarioDetailDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required GeneroUsuario Genero { get; set; }
        public required string Email { get; set; }
        public bool Ativo { get; set; } = true;
        public string? Telefone { get; set; }
        public string? ImagemPerfil { get; set; }
        public string? SteamId { get; set; }
        public DateTime? SteamIntegradoEm { get; set; }
    }

    public class UsuarioCreateDTO
    {
        public required string Nome { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required GeneroUsuario Genero { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public string? Telefone { get; set; }
        public string? ImagemPerfil { get; set; }
        public string? SteamId { get; set; }
    }

    public class UsuarioUpdateDTO
    {
        public required string Nome { get; set; }
        public string? Telefone { get; set; }
        public string? ImagemPerfil { get; set; }
        public string? SteamId { get; set; }
    }

    public class AlterarSenhaDTO
    {
        public required string SenhaAtual { get; set; }
        public required string NovaSenha { get; set; }
    }
}
