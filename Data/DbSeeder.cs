using Jogos_Backlogger.Models;

namespace Jogos_Backlogger.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Generos.Any())
            {
                return;
            }

            var generos = new List<Genero>
            {
                new Genero { Nome = "Ação" },
                new Genero { Nome = "Aventura" },
                new Genero { Nome = "RPG" },
                new Genero { Nome = "Estratégia" },
                new Genero { Nome = "Simulação" },
                new Genero { Nome = "Esportes" },
                new Genero { Nome = "Corrida" },
                new Genero { Nome = "Luta" },
                new Genero { Nome = "Puzzle" },
                new Genero { Nome = "Plataforma" },
                new Genero { Nome = "FPS" },
                new Genero { Nome = "TPS" },
                new Genero { Nome = "Terror" },
                new Genero { Nome = "Survival Horror" },
                new Genero { Nome = "Roguelike" },
                new Genero { Nome = "Metroidvania" },
                new Genero { Nome = "Visual Novel" },
                new Genero { Nome = "Point & Click" },
                new Genero { Nome = "MMORPG" },
                new Genero { Nome = "MOBA" },
                new Genero { Nome = "Battle Royale" },
                new Genero { Nome = "Stealth" },
                new Genero { Nome = "Sandbox" },
                new Genero { Nome = "Ritmo" },
                new Genero { Nome = "Cartas" },
                new Genero { Nome = "Hack and Slash" },
                new Genero { Nome = "Beat 'em up" },
                new Genero { Nome = "JRPG" },
                new Genero { Nome = "Souls-like" },
                new Genero { Nome = "Shoot 'em up" }
            };

            context.Generos.AddRange(generos);
            context.SaveChanges();
        }
        public static void SeedUsuario(ApplicationDbContext context)
        {
            if (context.Usuarios.Any(u => u.Email == "igor@teste.com"))
            {
                return;
            }

            var usuario = new Usuario
            {
                Nome = "Igor",
                Email = "igor@teste.com",
                DataNascimento = new DateOnly(2000, 9, 6),
                Genero = GeneroUsuario.Masculino,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Senha123"),
                SteamId = "76561198104504889",
            };

            context.Usuarios.Add(usuario);
            context.SaveChanges();
        }
    }
}