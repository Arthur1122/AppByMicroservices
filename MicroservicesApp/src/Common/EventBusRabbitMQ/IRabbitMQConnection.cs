using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ
{
    public interface IRabbitMQConnection : IDisposable
    {
        bool IsConected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
