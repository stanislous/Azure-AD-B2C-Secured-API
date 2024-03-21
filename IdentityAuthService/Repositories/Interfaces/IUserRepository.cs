using IdentityAuthService.Model;

namespace IdentityAuthService.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserDetails(string emailAddress);
    Task<bool> UpdateUserByIsMigrated(string emailAddress);
}