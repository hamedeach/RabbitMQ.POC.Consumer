// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

Console.WriteLine("######### RabbitMQ Consumer APP ############");


var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "echoQ",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"Consumner Received {message}");
}

while (true)
{
    Thread.Sleep(1000);
    channel.BasicConsume(queue: "echoQ",
    autoAck: true,
    consumer: consumer);

}
