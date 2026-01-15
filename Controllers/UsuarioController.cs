using Microsoft.AspNetCore.Mvc;
using Jogos_Backlogger.Data;
using Jogos_Backlogger.Models;
using Microsoft.EntityFrameworkCore;
using Jogos_Backlogger.DTOs;

namespace Jogos_Backlogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ListarUsuarios()
        {
            var usuariosDTO = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    DataNascimento = u.DataNascimento,
                    Genero = u.Genero,
                    Email = u.Email,
                    Ativo = u.Ativo
                })
                .ToListAsync();

            return Ok(usuariosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDetailDTO>> DetalhesUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDetailDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    DataNascimento = u.DataNascimento,
                    Genero = u.Genero,
                    Email = u.Email,
                    Ativo = u.Ativo,
                    Telefone = u.Telefone,
                    ImagemPerfil = u.ImagemPerfil,
                    SteamId = u.SteamId,
                    SteamIntegradoEm = u.SteamIntegradoEm
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> CriarUsuario(UsuarioCreateDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email))
            {
                return Conflict(new { message = "Email já está em uso." });
            }

            if(await _context.Administradores.AnyAsync(a => a.Email == usuarioDTO.Email))
            {
                return BadRequest("Este e-mail já está em uso por um administrador.");
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);

            var usuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                DataNascimento = usuarioDTO.DataNascimento,
                Genero = usuarioDTO.Genero,
                Email = usuarioDTO.Email,
                SenhaHash = senhaHash,
                Telefone = usuarioDTO.Telefone,
                ImagemPerfil = usuarioDTO.ImagemPerfil,
                SteamId = usuarioDTO.SteamId,
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var dto = new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                DataNascimento = usuario.DataNascimento,
                Genero = usuario.Genero,
                Email = usuario.Email,
                Ativo = usuario.Ativo
            };

            return CreatedAtAction(nameof(DetalhesUsuario), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarUsuario(int id, UsuarioCreateDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Senha);

            usuarioExistente.Nome = usuarioDTO.Nome;
            usuarioExistente.DataNascimento = usuarioDTO.DataNascimento;
            usuarioExistente.Genero = usuarioDTO.Genero;
            usuarioExistente.Email = usuarioDTO.Email;
            usuarioExistente.SenhaHash = senhaHash;
            usuarioExistente.Telefone = usuarioDTO.Telefone;
            usuarioExistente.ImagemPerfil = usuarioDTO.ImagemPerfil;
            usuarioExistente.SteamId = usuarioDTO.SteamId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Ativo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/alterar-senha")]
        public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.SenhaAtual, usuario.SenhaHash);
            if (!senhaValida)
            {
                return BadRequest("A senha atual está incorreta.");
            }

            string novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            usuario.SenhaHash = novaSenhaHash;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/perfil")]
        public async Task<IActionResult> AtualizarPerfil(int id, [FromBody] UsuarioUpdateDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Nome = dto.Nome;
            usuario.Telefone = dto.Telefone;
            usuario.ImagemPerfil = dto.ImagemPerfil;
            usuario.SteamId = dto.SteamId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [HttpPost("{id}/foto")]
        public async Task<IActionResult> UploadFotoPerfil(int id, IFormFile arquivo)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens");

            if (!Directory.Exists(pastaDestino))
                Directory.CreateDirectory(pastaDestino);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";
            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(usuario.ImagemPerfil))
            {
                try
                {
                    var nomeArquivoAntigo = Path.GetFileName(usuario.ImagemPerfil);

                    var caminhoArquivoAntigo = Path.Combine(pastaDestino, nomeArquivoAntigo);

                    if (System.IO.File.Exists(caminhoArquivoAntigo))
                    {
                        System.IO.File.Delete(caminhoArquivoAntigo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao apagar imagem antiga: {ex.Message}");
                }
            }

            var urlRelativa = $"/imagens/{nomeArquivo}";

            usuario.ImagemPerfil = urlRelativa;
            await _context.SaveChangesAsync();

            return Ok(new { url = urlRelativa });
        }

        private bool UsuarioExiste(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
