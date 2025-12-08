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
        public DbSet<JogoGenero> JogoGeneros { get; set; }
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
            modelBuilder.Entity<JogoGenero>().HasKey(jg => new { jg.JogoId, jg.GeneroId });

            // JogoGenero relacionamentos
            modelBuilder.Entity<JogoGenero>()
                .HasOne(jg => jg.Jogo)
                .WithMany()
                .HasForeignKey(jg => jg.JogoId);

            modelBuilder.Entity<JogoGenero>()
                .HasOne(jg => jg.Genero)
                .WithMany()
                .HasForeignKey(jg => jg.GeneroId);

            // ItemBacklog relacionamentos
            modelBuilder.Entity<ItemBacklog>()
                .HasOne<Jogo>()
                .WithMany()
                .HasForeignKey(ib => ib.JogoId);

            modelBuilder.Entity<ItemBacklog>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(ib => ib.UsuarioId);
        }
    }
}
