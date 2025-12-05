using Jogos_Backlogger.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogos_Backlogger.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            // Construtor
        }

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<JogoGenero> JogosGeneros { get; set; }
        public DbSet<ItemBacklog> ItemBacklogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeando as entidades com o banco de dados
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrador>().ToTable("Administrador");

            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            modelBuilder.Entity<Jogo>().ToTable("Jogo");
            modelBuilder.Entity<Genero>().ToTable("Genero");
            modelBuilder.Entity<JogoGenero>().ToTable("JogoGenero");
            modelBuilder.Entity<ItemBacklog>().ToTable("ItemBacklog");

            // Chave composta
            modelBuilder.Entity<JogoGenero>().HasKey(jg => new { jg.jogoId, jg.generoId });
        }
    }
}
