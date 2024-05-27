using Dapper;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Infrastructure.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _dbConnection.QueryAsync<Region>("SELECT * FROM Regions");
        }

        public async Task<Region> GetByDDDAsync(string ddd)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Region>("SELECT * FROM Regions WHERE DDD = @DDD", new { DDD = ddd });
        }

        public async Task AddAsync(Region region)
        {
            var sql = "INSERT INTO Regions (DDD, Name) VALUES (@DDD, @Name)";
            await _dbConnection.ExecuteAsync(sql, region);
        }

        public async Task UpdateAsync(Region region)
        {
            var sql = "UPDATE Regions SET Name = @Name WHERE DDD = @DDD";
            await _dbConnection.ExecuteAsync(sql, region);
        }

        public async Task DeleteAsync(string ddd)
        {
            var sql = "DELETE FROM Regions WHERE DDD = @DDD";
            await _dbConnection.ExecuteAsync(sql, new { DDD = ddd });
        }

        public async Task<IEnumerable<Contact>> GetContactsByDDDAsync(string ddd)
        {
            var sql = "SELECT * FROM Contacts WHERE DDD = @DDD";
            return await _dbConnection.QueryAsync<Contact>(sql, new { DDD = ddd });
        }
    }
}
