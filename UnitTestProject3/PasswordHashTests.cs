using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject3
{
    public class PasswordHashTests
    {
        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "password123";
            string hashedPasswordWithSalt = HashPassword(password);

            // Act
            bool result = VerifyPassword(hashedPasswordWithSalt, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string correctPassword = "password123";
            string incorrectPassword = "wrongpassword";
            string hashedPasswordWithSalt = HashPassword(correctPassword);

            // Act
            bool result = VerifyPassword(hashedPasswordWithSalt, incorrectPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithMalformedHashedPassword_ReturnsFalse()
        {
            // Arrange
            string password = "password123";
            string malformedHashedPasswordWithSalt = "invalidformat";

            // Act
            bool result = VerifyPassword(malformedHashedPasswordWithSalt, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithNullInput_ReturnsFalse()
        {
            // Act
            bool result = VerifyPassword(null, "password123");

            // Assert
            Assert.False(result);
        }

        // Helper function to hash a password with salt
        private static string HashPassword(string password)
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

        // Method under test
        public static bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            // Tách salt và hash từ chuỗi đã lưu trữ
            if (string.IsNullOrEmpty(hashedPasswordWithSalt) || string.IsNullOrEmpty(password))
                return false;

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
    }
}
