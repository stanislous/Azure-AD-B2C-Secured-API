using Dapper;
using System.Text;
using IdentityAuthService.Model;
using IdentityAuthService.DbContext;
using IdentityAuthService.Repositories.Interfaces;

namespace IdentityAuthService.Repositories.Implementations;

public class UserRepository(IDbProvider provider) : IUserRepository
{
    public IEnumerable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserDetails(string emailAddress)
    {
        try
        {
            using (var connection = await provider.CreateConnectionAsync())
            {
                var query = new StringBuilder();
                query.Append(@"SELECT 
                                    UserFirstName, 
                                    UserLastName, 
                                    UserEmailAddress, 
                                    UserPhoneNumber 
                                FROM Users 
                                WHERE UserEmailAddress = @EmailAddress AND isMigrated = 'false';");
                return connection.QueryFirstOrDefault<User>(query.ToString(), new
                {
                    EmailAddress = emailAddress
                });
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Task<int> Create(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }
}