using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RegionalContactsApp.Domain.Entities;
using RegionalContactsApp.Domain.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace RegionalContactsApp.Application.Workservices
{
    public class ContactConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ContactConsumerService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "ContactRegisteredQueue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received message: {message}");

                // A mensagem agora inclui o tipo de operação
                var contactMessage = JsonConvert.DeserializeObject<ContactMessage>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var contactRepository = scope.ServiceProvider.GetRequiredService<IContactRepository>();

                    // Identificando o tipo de operação
                    switch (contactMessage.Operation)
                    {
                        case "Create":
                            // Criação de contato
                            var newContact = contactMessage.Contact;
                            await contactRepository.AddAsync(newContact);
                            break;

                        case "Update":
                            // Atualização de contato
                            var updatedContact = contactMessage.Contact;
                            await contactRepository.UpdateAsync(updatedContact);
                            break;

                        case "Delete":
                            // Exclusão de contato
                            var contactIdToDelete = contactMessage.Contact.Id;
                            await contactRepository.DeleteAsync(contactIdToDelete);
                            break;

                        default:
                            Console.WriteLine("Unknown operation.");
                            break;
                    }
                }
            };

            _channel.BasicConsume(queue: "ContactRegisteredQueue",
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
