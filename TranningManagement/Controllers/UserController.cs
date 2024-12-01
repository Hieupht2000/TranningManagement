using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using TranningManagement.Model;
using static TranningManagement.Controllers.UserController;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDTOs = users.Select(user => new UserDTO
            {
                user_id = user.user_id,
                Name = user.Name,
                Email = user.Email,
                password_hash = user.password_hash,
                Role = user.Role.ToString(),
                Status = user.Status
            }).ToList();

            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDTO = new UserDTO
            {
                user_id = user.user_id,
                Name = user.Name,
                Email = user.Email,
                password_hash = user.password_hash,
                Role = user.Role.ToString(),
                Status = user.Status
            };

            return Ok(userDTO);
        }

        private string HashPassword(string password)
        {
            // Tạo salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Băm mật khẩu với PBKDF2
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Kết hợp salt và hash vào một chuỗi
            return $"{Convert.ToBase64String(salt)}:{hashedPassword}";
        }

        public enum Role
        {
            Student,
            Teacher,
            Staff,
            Admin
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDTO)
        {
            if (!Enum.TryParse(userDTO.Role, true, out Role role))
            {
                return BadRequest($"Invalid role: {userDTO.Role}");
            }

            
            var user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                password_hash = HashPassword(userDTO.password_hash),
                Role = userDTO.Role,
                Status = userDTO.Status
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDTO.user_id = user.user_id; 

            return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, userDTO);
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.user_id == id);
        }
    }
}
