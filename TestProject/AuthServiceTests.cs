using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranningManagement.Model;
namespace TestProject
{
    [TestFixture]
    public class AuthServiceTests
    {
        private ApplicationDbContext _context;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _authService = new AuthService(_context);

            // Thêm một người dùng giả vào cơ sở dữ liệu
            _context.Users.Add(new User { Name = "testuser", password_hash = "password123" });
            _context.SaveChanges();
        }

        [Test]
        public async Task LoginAsync_UserNotFound_ReturnsUserNotFoundMessage()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "password123";

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            NUnit.Framework.Assert.AreEqual("User not found", result);
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsInvalidPasswordMessage()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            NUnit.Framework.Assert.AreEqual("Invalid password", result);
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginSuccessfulMessage()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            NUnit.Framework.Assert.AreEqual("Login successful", result);
        }
    }
}
