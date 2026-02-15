using IdentityService.Data;
using IdentityService.Dtos;
using IdentityService.Models;
using IdentityService.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services
{
    public class UserService(IdentityDbContext dbContext, IPasswordHasher<User> passwordHasher) : IUserService
    {
        private readonly IdentityDbContext _dbContext = dbContext;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        public async Task<User> RegisterAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = HashPassword(user, request.Password);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }

        private string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
