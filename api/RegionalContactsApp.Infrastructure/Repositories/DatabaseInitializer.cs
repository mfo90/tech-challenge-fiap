using Dapper;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Infrastructure.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbConnection _dbConnection;

        public DatabaseInitializer(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Initialize()
        {
            var createRegionsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Regions (
                DDD VARCHAR(3) PRIMARY KEY,
                Name VARCHAR(100) NOT NULL
            );
        ";

            var createContactsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Contacts (
                Id SERIAL PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Phone VARCHAR(15) NOT NULL,
                Email VARCHAR(100) NOT NULL,
                DDD VARCHAR(3) NOT NULL,
                CONSTRAINT fk_Contacts_Regions FOREIGN KEY (DDD) REFERENCES Regions(DDD)
            );
        ";

            _dbConnection.Execute(createRegionsTableQuery);
            _dbConnection.Execute(createContactsTableQuery);
        }
    }
}
