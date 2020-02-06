using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitReceiver
{
    class Receiver
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true,
            };

            Console.WriteLine("Making connections and channel...");
            using var connection = factory.CreateConnection("Receiver");
            using var taskChannel = connection.CreateModel();

            taskChannel.QueueDeclare(queue: "tasks",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var taskConsumer = new AsyncEventingBasicConsumer(taskChannel);
            taskConsumer.Received += async (sender, args) =>
            {
                Console.WriteLine("Got a message!");

                try
                {
                    var cancel = connection.CreateModel();
                    Console.WriteLine("Created a channel!");
                    cancel.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
                await Task.CompletedTask;
            };

            taskChannel.BasicConsume(queue: "tasks",
                                     autoAck: true,
                                     consumer: taskConsumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
