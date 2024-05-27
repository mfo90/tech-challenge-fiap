using Dapper;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RegionalContactsApp.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDbConnection _dbConnection;

        public ContactRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<Contact>("SELECT * FROM Contacts");
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Contact>("SELECT * FROM Contacts WHERE Id = @Id", new { Id = id });
        }

        public async Task AddAsync(Contact contact)
        {
            var sql = "INSERT INTO Contacts (Name, Phone, Email, DDD) VALUES (@Name, @Phone, @Email, @DDD)";
            await _dbConnection.ExecuteAsync(sql, contact);
        }

        public async Task UpdateAsync(Contact contact)
        {
            var sql = "UPDATE Contacts SET Name = @Name, Phone = @Phone, Email = @Email, DDD = @DDD WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, contact);
        }

        public async Task DeleteAsync(int id)
        {
            var sql = "DELETE FROM Contacts WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Contact>> GetContactsByDDDAsync(string ddd)
        {
            var sql = "SELECT * FROM Contacts WHERE DDD = @DDD";
            return await _dbConnection.QueryAsync<Contact>(sql, new { DDD = ddd });
        }

        public async Task<Contact> GetContactByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Contacts WHERE Email = @Email";
            return await _dbConnection.QueryFirstOrDefaultAsync<Contact>(sql, new { Email = email });
        }
    }
}
