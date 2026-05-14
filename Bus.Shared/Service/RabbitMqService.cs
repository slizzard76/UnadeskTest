using Bus.Shared.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Bus.Shared.Service
{
    /// <summary>
    /// Сервис для управления подключением к RabbitMQ.
    /// Реализует логику повторных попыток подключения при недоступности брокера.
    /// </summary>
    public class RabbitMqService : IRabbitMqService
    {
        // Конфигурация RabbitMQ, полученная из опций приложения.
        private readonly RabbitConfiguration _configuration;
        // Максимальное количество попыток подключения (включая первую попытку).
        private const int RetryCounts = 3;

        /// <summary>
        /// Инициализирует новый экземпляр RabbitMqService.
        /// </summary>
        /// <param name="options">Опции, содержащие конфигурацию RabbitMQ.</param>
        public RabbitMqService(IOptions<RabbitConfiguration> options)
        {
            _configuration = options.Value;
        }

        /// <summary>
        /// Асинхронно создает и возвращает подключение к RabbitMQ.
        /// Повторяет попытку подключения в случае ошибки BrokerUnreachableException.
        /// </summary>
        /// <returns>Объект IConnection, представляющий активное подключение.</returns>
        public async Task<IConnection> CreateConnectionAsync()
        {
            IConnection connection = null;
            // Цикл для выполнения попыток подключения.
            // i <= RetryCounts означает, что будет выполнено (RetryCounts + 1) попыток.
            for (int i = 0; i <= RetryCounts; i++)
            {
                try
                {
                    // Создание фабрики подключения с использованием настроенных параметров.
                    ConnectionFactory connectionFactory = new ConnectionFactory
                    {
                        UserName = _configuration.UserName,
                        Password = _configuration.Password,
                        HostName = _configuration.HostName,
                    };

                    // Попытка создания асинхронного соединения.
                    connection = await connectionFactory.CreateConnectionAsync();
                    // Успешное подключение, выходим из цикла.
                    break;
                }
                catch (BrokerUnreachableException e)
                {
                    // Логирование ошибки и информирование пользователя о повторной попытке.
                    Console.WriteLine("Не удалась попытка присоединения к серверу Rabbit, пробуем еще раз");
                    // Пауза перед следующей попыткой (5 секунд).
                    await Task.Delay(5000);
                }
            }
            // Возвращаем полученное соединение (может быть null, если все попытки провалились).
            return connection;
        }
    }   
}
