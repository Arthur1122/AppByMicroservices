using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using MediatR;
using Newtonsoft.Json;
using Ordering.Application.Commands;
using Ordering.Core.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.API.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IMediator _mediator;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IMediator mediator, IOrderRepository orderRepository, IMapper mapper)
        {
            _connection = connection;
            _mediator = mediator;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public void Consume()
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(EventBusConstants.BasketCheckoutQueue, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += RecivedEvent;

            channel.BasicConsume(EventBusConstants.BasketCheckoutQueue, autoAck: true, consumer);
        }
        private async void RecivedEvent(object sender, BasicDeliverEventArgs eventArgs)
        {
            if(eventArgs.RoutingKey == EventBusConstants.BasketCheckoutQueue)
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                var basketCheckoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                var command = _mapper.Map<CheckoutOrderCommand>(basketCheckoutEvent);
                var result = await _mediator.Send(command);
            }
        }

        public void DisConnect()
        {
            _connection.Dispose();
        }
    }
}
