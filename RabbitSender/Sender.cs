using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitTesting
{
    class Sender
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = false
            };

            using var connection = factory.CreateConnection("Sender");
            using var taskChannel = connection.CreateModel();

            taskChannel.QueueDeclare(queue: "tasks",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            bool shouldCancel = false;
            while (!shouldCancel)
            {
                Console.WriteLine("Publishing message...");
                taskChannel.BasicPublish(exchange: String.Empty,
                                         routingKey: "tasks",
                                         mandatory: false,
                                         basicProperties: null,
                                         body: Encoding.UTF8.GetBytes("hello"));

                Console.WriteLine("Type something in to cancel; press [enter] to continue.");
                var input = Console.ReadLine();

                shouldCancel = !String.IsNullOrWhiteSpace(input);
            }

            Console.WriteLine(" Done ");
        }
    }
}
