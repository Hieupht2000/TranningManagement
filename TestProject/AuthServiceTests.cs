using Xunit;
using TranningManagement.Controllers;
using TranningManagement.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.Data;

namespace TestProject
{
    public class AuthServiceTests
    {
        private readonly UserLoginController _userLoginController;

        [Fact]
        public void Login_WithCorrectCredentials_ReturnsTrue()
        {
            // Arrange
            //var authService = new AuthService();

            UserloginDTO userLoginDTO = new UserloginDTO
            {
                Email = "testuser",
                password_hash = "password123"
            };

            var result = _userLoginController.Login(userLoginDTO);
            
            // Assert
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public void Login_WithIncorrectCredentials_ReturnsFalse()
        {
            // Arrange
            //var authService = new AuthService();
            // Arrange
            var userLoginDTO = new UserloginDTO
            {
                Email = "testuser@example.com",
                password_hash = "wrongpassword"
            };

            // Act
            var result = _userLoginController.Login(userLoginDTO);

            // Assert
            Assert.False(result.IsCanceled);
        }
    }
}
