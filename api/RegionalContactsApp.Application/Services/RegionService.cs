using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace RegionalContactsApp.Application.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IConfiguration _configuration;
        private readonly byte[] _key;

        public RegionService(IRegionRepository regionRepository, IContactRepository contactRepository, IConfiguration configuration)
        {
            _regionRepository = regionRepository;
            _contactRepository = contactRepository;
            // Usar uma chave fixa para HMACSHA512 (isso é para simplificação, para produção você deve usar um salt único para cada senha)
            _key = Encoding.UTF8.GetBytes("a-secure-key-of-your-choice");
            _configuration = configuration;
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            return await _regionRepository.GetAllAsync();
        }

        public async Task<Region> GetRegionByDDDAsync(string ddd)
        {
            return await _regionRepository.GetByDDDAsync(ddd);
        }

        public async Task AddRegionAsync(Region region)
        {
            ValidateRegion(region);

            // Criação da mensagem com a operação "Create"
            var regionMessage = new RegionMessage
            {
                Operation = "Create",  // Define a operação como "Create"
                Region = region
            };

            SendMessageToQueue(regionMessage);

            //await _regionRepository.AddAsync(region);
        }

        public async Task UpdateAsync(Region region)
        {
            ValidateRegion(region);

            // Criação da mensagem com a operação "Create"
            var regionMessage = new RegionMessage
            {
                Operation = "Update",  // Define a operação como "Update"
                Region = region
            };

            SendMessageToQueue(regionMessage);

            //await _regionRepository.UpdateAsync(region);
        }

        public async Task DeleteAsync(string ddd)
        {
            var contacts = await _contactRepository.GetContactsByDDDAsync(ddd);
            if (contacts.Any())
            {
                throw new ValidationException("Cannot delete region with contacts.");
            }


            // Criação da mensagem com a operação "Create"
            var regionMessage = new RegionMessage
            {
                Operation = "Delete", // Define a operação como "Delete"
                Region = new Region()  // Cria uma nova instância de Contact
                {
                    DDD = ddd  // Atribui o valor de 'id' à propriedade 'Id' da instância de Contact
                }
            };

            SendMessageToQueue(regionMessage);


            //await _regionRepository.DeleteAsync(ddd);
        }

        private void ValidateRegion(Region region)
        {
            if (string.IsNullOrWhiteSpace(region.DDD))
            {
                throw new ValidationException("DDD is required.");
            }
            if (region.DDD.Length > 2)
            {
                throw new ValidationException("DDD cannot exceed 2 characters.");
            }
            if (string.IsNullOrWhiteSpace(region.Name))
            {
                throw new ValidationException("Name is required.");
            }
            if (region.Name.Length > 100)
            {
                throw new ValidationException("Name cannot exceed 100 characters.");
            }
        }

        public void SendMessageToQueue(RegionMessage regionMessage)
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
                channel.QueueDeclare(queue: "RegionRegisteredQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Serialize the object to JSON
                string jsonMessage = JsonConvert.SerializeObject(regionMessage);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: "RegionRegisteredQueue",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
