using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.Shared.Service
{
    public interface IRabbitMqService
    {
        Task<IConnection> CreateConnectionAsync();
    }
}
