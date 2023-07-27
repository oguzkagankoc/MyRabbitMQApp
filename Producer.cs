using System;
using RabbitMQ.Client;
using System.Text;

namespace MyRabbitMQApp
{
    class Producer
    {
        public void SendMessage(string jsonData, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(jsonData);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" [x] Gönderilen Veri ({queueName}): {jsonData}");
            }
        }
    }
}
