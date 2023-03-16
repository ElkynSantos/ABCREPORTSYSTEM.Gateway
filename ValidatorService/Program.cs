using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using ValidatorService;
using System;

Console.WriteLine("Hello, World!");



HttpClient client = new HttpClient();

var factory = new ConnectionFactory() { HostName = "localhost" };
var records = new List<Sales>();

using (var connection = factory.CreateConnection())

using (var _channel = connection.CreateModel())
{
    _channel.QueueDeclare(queue: "init",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

    int threads = 1;
    var _consumer = new EventingBasicConsumer(_channel);
    _consumer.Received += (model, ea) =>
     {
         var body = ea.Body.ToArray();
         var message = Encoding.UTF8.GetString(body);
         threads = Int32.Parse(message);

         Task[] tasks = new Task[threads];
         for (int i = 0; i < threads; i++)
         {

             int current = i;
             tasks[i] = Task.Factory.StartNew(() =>
             {
                 using (var channel = connection.CreateModel())
                 {

                     channel.QueueDeclare(queue: current.ToString(),
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                     var consumer = new EventingBasicConsumer(channel);
                     consumer.Received += async (model, ea) =>
                     {
                         var body = ea.Body.ToArray();
                         var registro_json = Encoding.UTF8.GetString(body);



                         //Console.WriteLine($"thread: {current}: {registro_json}");

                         Sales sale = JsonSerializer.Deserialize<Sales>(registro_json);

                         // var response = await client.GetStringAsync($"https://localhost:7073/SucursalC/Automobile/{sale?.Vin}/{sale?.branch_office_id}");

                         HttpResponseMessage response = await client.GetAsync($"https://localhost:7073/SucursalC/Automobile/{sale?.Vin}/{sale?.branch_office_id}");
                  
                         string responseContent = await response.Content.ReadAsStringAsync();
                         Console.WriteLine(responseContent);
                     };
                     channel.BasicConsume(queue: current.ToString(),
                                          autoAck: true,
                                          consumer: consumer);

                 }
             });
         }
     };

    _channel.BasicConsume(queue: "init",
                          autoAck: true,
                          consumer: _consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
};
