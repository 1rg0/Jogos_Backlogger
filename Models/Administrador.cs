namespace Jogos_Backlogger.Models
{
    public class Administrador
    {
        public int id { get; set; }
        public required string email { get; set; }
        public required string senhaHash { get; set; }
    }
}
