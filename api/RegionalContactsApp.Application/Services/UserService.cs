using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly byte[] _key;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            // Usar uma chave fixa para HMACSHA512 (isso é para simplificação, para produção você deve usar um salt único para cada senha)
            _key = Encoding.UTF8.GetBytes("a-secure-key-of-your-choice");
            _configuration = configuration;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task RegisterAsync(string username, string password, string role)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = CreatePasswordHash(password),
                Role = role
            };

            SendMessageToQueue(username);

            //await _userRepository.AddUserAsync(user);
        }

        private string CreatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512(_key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            using (var hmac = new HMACSHA512(_key))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(passwordBytes);
                var hashString = Convert.ToBase64String(hash);
                return hashString == storedHash;
            }
        }

        private void SendMessageToQueue(string username)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "UserRegisteredQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = $"User Registered: {username}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "UserRegisteredQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
