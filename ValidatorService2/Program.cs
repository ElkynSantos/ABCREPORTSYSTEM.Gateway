using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ValidatorService2;

Sales sale = null;

List<Sales> sales2 = new List<Sales>();
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

    for (int i = 0; i < 2; i++)
    {

        channel.QueueDeclare(queue: i.ToString(),
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var messageReceived = new ManualResetEvent(false);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            sale = JsonSerializer.Deserialize<Sales>(message);
            Console.WriteLine(sale);
            sales2.Add(sale);


            messageReceived.Set();
        };
        channel.BasicConsume(queue: i.ToString(),
                         autoAck: true,
                         consumer: consumer);


        messageReceived.WaitOne();

    }

    Console.WriteLine("salio");
    Console.WriteLine("Presiona [enter] para salir.");
    Console.ReadLine();
}

