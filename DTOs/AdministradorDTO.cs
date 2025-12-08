namespace Jogos_Backlogger.DTOs
{
    public class AdministradorDTO
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string SenhaHash { get; set; }
    }
}
