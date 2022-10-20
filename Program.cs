// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

Console.WriteLine("######### RabbitMQ Consumer APP ############");

Random random = new Random();
string instanceID =  RandomString(4);

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

//declare our exchange explicitly 
channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);


// define temp queue to be destroyed after the consumer close the connection with the producer
var queueName =channel.QueueDeclare().QueueName; ;
//bind the queue to the exchange 
channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"{instanceID} - Consumner Received {message}");
}

while (true)
{
    Thread.Sleep(1000);
    channel.BasicConsume(queue: queueName,
    autoAck: true,
    consumer: consumer);

}

 

  string RandomString(int length)
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}
