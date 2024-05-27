using Dapper;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace RegionalContactsApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            string sql = "SELECT * FROM Users WHERE Username = @Username";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task AddUserAsync(User user)
        {
            string sql = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)";
            await _dbConnection.ExecuteAsync(sql, user);
        }
    }
}
