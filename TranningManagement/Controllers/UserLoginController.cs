using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TranningManagement.Model;

namespace TranningManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey = "YourVeryLongSecretKeyThatShouldBeAtLeast32BytesLong"; // Store this securely
        private readonly string _issuer = "http://localhost:7225/";
        private readonly string _audience = "http://localhost:7225/";
        private readonly JwtTokenHelper _jwtTokenHelper;

        public UserLoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserloginDTO userLoginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userLoginDTO.Email);
            if (user == null || !VerifyPassword(user.password_hash, userLoginDTO.password_hash))
            {
                return Unauthorized("Invalid credentials");
            }
            var token = GenerateJwtToken(user.Email);
            return Ok(new { Token = token });
        }

        // Hàm băm mật khẩu
        public static string HashPassword(string password)
        {
            // Tạo salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash mật khẩu với Pbkdf2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Kết hợp salt và hash
            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        public static bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            // Tách salt và hash từ chuỗi đã lưu trữ
            var parts = hashedPasswordWithSalt.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hashedPassword = parts[1];

            // Hash mật khẩu nhập vào với salt đã lưu trữ
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // So sánh hash của mật khẩu nhập vào với hash đã lưu trữ
            return hashed == hashedPassword;
        }

        private string GenerateJwtToken(string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
