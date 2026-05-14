using Bus.Shared.Configuration;
using Bus.Shared.Models;
using Bus.Shared.Service;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Bus.Shared.Producer
{
    public class MessageQueueProducer : IMessageQueueProducer
    {

        private readonly RabbitConfiguration _rabbitConfig;
        private readonly IRabbitMqService _rabbitMqService;
        
        public MessageQueueProducer(IOptions<RabbitConfiguration> options, IRabbitMqService rabbitMqService)
        { 
            _rabbitConfig = options.Value;
            _rabbitMqService  = rabbitMqService;
        }

        public async Task PublishJobAsync(ProcessingJob job, CancellationToken cancellationToken)
        {
            using var connection = await _rabbitMqService.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var jsonBody = JsonSerializer.Serialize(job);
            var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
            var props = new BasicProperties();
            await channel.BasicPublishAsync(_rabbitConfig.ExchangeName, string.Empty, false, props, bodyBytes);
        }
    }
}
