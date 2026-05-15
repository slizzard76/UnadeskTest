namespace Bus.Shared.Configuration
{
    public class RabbitConfiguration
    {
        // Имя хоста RabbitMQ брокера.
        public string HostName { get; set; }
        // Порт RabbitMQ (закомментирован, но оставлен для справки).
        //public int Port { get; set; }
        // Имя пользователя для подключения.
        public string UserName { get; set; }
        // Пароль пользователя.
        public string Password { get; set; }
        // Имя очереди, к которой будет подключаться приложение.
        public string QueueName { get; set; }
        // Имя обменника (exchange), который будет использоваться.
        public string ExchangeName { get; set; }
    }
 }
