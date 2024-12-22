using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Octopus.Client.Repositories.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return "User not found";
            }

            if (user.Password != password)
            {
                return "Invalid password";
            }

            return "Login successful";
        }
    }
}
