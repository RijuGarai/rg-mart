using IdentityService.Dtos;
using IdentityService.Models;

namespace IdentityService.Services.Contracts
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserRequest request);
        Task<User?> LoginAsync(LoginRequest request);
        Task<User?> GetUserAsync(Guid userId);
    }
}
