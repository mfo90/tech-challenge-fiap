using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalContactsApp.Application.Workservices
{
    public class RegionConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RegionConsumerService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
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
                var regionMessage = JsonConvert.DeserializeObject<RegionMessage>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var regionRepository = scope.ServiceProvider.GetRequiredService<IRegionRepository>();

                    // Identificando o tipo de operação
                    switch (contactMessage.Operation)
                    {
                        case "Create":
                            // Criação de região
                            var newRegion = regionMessage.Region;
                            await regionRepository.AddAsync(newRegion);
                            break;

                        case "Update":
                            // Atualização de contato
                            var updatedRegion = regionMessage.Region;
                            await regionRepository.UpdateAsync(updatedContact);
                            break;

                        case "Delete":
                            // Exclusão de contato
                            var regionIdToDelete = regionMessage.Region.ddd;
                            await regionRepository.DeleteAsync(regionIdToDelete);
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
