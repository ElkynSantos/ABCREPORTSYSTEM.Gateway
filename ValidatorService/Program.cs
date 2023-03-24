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
using System.Threading.Channels;
using System.Transactions;
using ValidatorService.Dtos;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

Console.WriteLine("VALIDATOR");

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


  

   
    var _consumer = new EventingBasicConsumer(_channel);
    _consumer.Received +=  async (model, ea) =>
     {



         var body = ea.Body.ToArray();
         var message = Encoding.UTF8.GetString(body);

         var transaction = JsonSerializer.Deserialize<DATAS>(message);
         var data = new DATAS { TransactionId=transaction.TransactionId,
         Errors=transaction.Errors,
         threads=transaction.threads};

      

         var response1 = new ValidatorService.Dtos.Transaction
         {
             TransactionId = transaction.TransactionId,
             Errors = transaction.Errors,
         
         };

       

         object lockObj = new object();
         List<Task> tasks = new List<Task>();
         for (int i = 0; i < (int)data.threads; i++)
         {
           

             int current = i;
             tasks.Add(Task.Run(async () => {
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



                         // Console.WriteLine($"thread: {current}: {registro_json}");

                         Sales sale = JsonSerializer.Deserialize<Sales>(registro_json);

                         HttpResponseMessage response = await client.GetAsync($"https://localhost:7073/SucursalC/Automobile/{sale?.Vin}/{sale?.branch_office_id}");
                         HttpResponseMessage response2 = await client.GetAsync($"https://localhost:7073/SucursalC/Employee/{sale?.Username}/{sale?.branch_office_id}");

                         string responseContent = await response.Content.ReadAsStringAsync();
                         string responseContent2 = await response2.Content.ReadAsStringAsync();
                         lock (lockObj)
                         {
                             response1.Errors.Add(responseContent);
                             response1.Errors.Add(responseContent2);


                         }


                     };
                     channel.BasicConsume(queue: current.ToString(),
                                        autoAck: true,
                                         consumer: consumer);

                 }
                 await Task.Delay(30000);
                 return Task.CompletedTask;
             }));

          
         }

              await Task.Delay(30000);


             response1.Errors.RemoveAll(s => s == "");
            
             string response1Json = JsonSerializer.Serialize(response1);
             byte[] response1Body = Encoding.UTF8.GetBytes(response1Json);

             using (var responseChannel = connection.CreateModel())
             {


                 responseChannel.BasicPublish(exchange: "",
                                              routingKey: "transactionfinal",
                                              basicProperties: null,
                                              body: response1Body);
             }
         
     };

    _channel.BasicConsume(queue: "init",
                          autoAck: true,
                          consumer: _consumer);



    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();



}






