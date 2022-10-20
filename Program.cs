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

// set the consumer prefetch count to 1 
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var random = new Random();

var consumer = new EventingBasicConsumer(channel);
consumer.Received += Consumer_Received;


void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    var delaytime = random.Next(2, 5);
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"Consumner Received {message}  - the processing time {delaytime}");
    // simualte that the task take a time
    Task.Delay(TimeSpan.FromSeconds(delaytime)).Wait();

    //implement the manually ack
    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
}

while (true)
{
    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
    channel.BasicConsume(queue: "echoQ",
    autoAck: false, // disable the auto ack 
    consumer: consumer);

}
