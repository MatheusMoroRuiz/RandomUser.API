using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandomUser.API.Data;
using RandomUser.API.Models;
using RandomUser.API.Services;
using System.Text.Json;


namespace RandomUser.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RandomUserService _randomUserService;

        public UsersController(AppDbContext context, RandomUserService randomUserService)
        {
            _context = context;
            _randomUserService = randomUserService;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportarUsuarios()
        {
            using var httpClient = new HttpClient();
            var resposta = await httpClient.GetAsync("https://randomuser.me/api/?results=30");

            if (!resposta.IsSuccessStatusCode)
                return StatusCode((int)resposta.StatusCode, "Erro ao buscar usuários da API Random User");

            var json = await resposta.Content.ReadAsStringAsync();
            var documento = JsonDocument.Parse(json);
            var resultados = documento.RootElement.GetProperty("results");

            var usuarios = new List<User>();

            foreach (var usuarioJson in resultados.EnumerateArray())
            {
                var nome = usuarioJson.GetProperty("name");
                var localizacao = usuarioJson.GetProperty("location");

                usuarios.Add(new User
                {
                    Name = $"{nome.GetProperty("first").GetString()} {nome.GetProperty("last").GetString()}",
                    Email = usuarioJson.GetProperty("email").GetString(),
                    Gender = usuarioJson.GetProperty("gender").GetString(),
                    Country = localizacao.GetProperty("country").GetString()
                });
            }

            _context.Users.AddRange(usuarios);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = $"{usuarios.Count} usuários importados com sucesso!" });
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
                return NotFound();

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Gender = updatedUser.Gender;
            existingUser.Country = updatedUser.Country;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Parâmetro de busca é obrigatório.");

            var results = await _context.Users
                .Where(u => u.Name.ToLower().Contains(query.ToLower()) ||
                            u.Email.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return Ok(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
