// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

Console.WriteLine("######### RabbitMQ Client APP ############");


var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

// declare the replay queue where the client will receive the response
var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

//declare the requests queue where the client will send the request to the server the serever will consume this queue
var requestQueue = channel.QueueDeclare(queue: "request-queue", exclusive: false);


var consumer = new EventingBasicConsumer(channel);
consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{

    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine($"the client Received the replay to the request id {e.BasicProperties.CorrelationId} {message}");
}

//consume the queue from the server 
channel.BasicConsume(queue: replyQueue,
    autoAck: true,
    consumer: consumer);

while (true)
{
    Task.Delay(2500).Wait();    
    // define a request to the client
    var requestMessage = "What is the current time ?";
    var requestBody = Encoding.UTF8.GetBytes(requestMessage);

    var properties = channel.CreateBasicProperties();
    properties.ReplyTo = replyQueue.QueueName;
    properties.CorrelationId = Guid.NewGuid().ToString();
    //publish the request to the server
    channel.BasicPublish("", "request-queue", properties, requestBody);
    Console.WriteLine($"the client sending a request with id {properties.CorrelationId}");

}

//while (true)
//{
//    Thread.Sleep(1000);
//    channel.BasicConsume(queue: replyQueue,
//    autoAck: true,
//    consumer: consumer);

//}




