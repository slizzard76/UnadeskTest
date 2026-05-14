using Bus.Shared.Configuration;
using Bus.Shared.Service;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Bus.Shared.Consumer
{
    public class MessageQueueConsumer : IMessageQueueConsumer
    {
        private readonly IRabbitMqService _rabbitMqService;
        private IChannel _channel;
        private readonly RabbitConfiguration _configuration;

        public MessageQueueConsumer(IOptions<RabbitConfiguration> options, IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
            _configuration = options.Value;
        }
        public async Task StartConsumingAsync(Func<ConsumeEventArgs, Task> messageHandler)
        {
            var connection = await _rabbitMqService.CreateConnectionAsync();

            _channel = await connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(_configuration.QueueName, durable: true, exclusive: false, autoDelete: false);
            await _channel.ExchangeDeclareAsync(_configuration.ExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            await _channel.QueueBindAsync(_configuration.QueueName, _configuration.ExchangeName, string.Empty);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var text = Encoding.UTF8.GetString(body);

                await messageHandler(new ConsumeEventArgs { Body = text});
                await Task.CompletedTask;
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            await _channel.BasicConsumeAsync(_configuration.QueueName, false, consumer);
            await Task.CompletedTask;

        }
    }
}
