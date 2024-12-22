using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UserTest
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Name = "John Doe", Email = "john.doe@example.com" };

            // Act
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("John Doe", savedUser.Name);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserFromDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Name = "Jane Doe", Email = "jane.doe@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "jane.doe@example.com");

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal("Jane Doe", savedUser.Name);
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyUserDetails()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Name = "Old Name", Email = "update@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "update@example.com");
            savedUser.Name = "Updated Name";
            await context.SaveChangesAsync();

            // Assert
            var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "update@example.com");
            Assert.Equal("Updated Name", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Name = "Delete Me", Email = "delete@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "delete@example.com");
            context.Users.Remove(savedUser);
            await context.SaveChangesAsync();

            // Assert
            var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "delete@example.com");
            Assert.Null(deletedUser);
        }
    }
}
