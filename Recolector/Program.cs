

using CsvHelper;
using CsvHelper.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Recolector;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Recolector.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.WriteLine("Hello, World!");


var factory = new ConnectionFactory() { HostName = "localhost" };
var records = new List<Sales>();


using (var StreamReader = new StreamReader(@"C:\Users\elkyn\source\repos\ABCREPORTSYSTEM.Gateway\Recursos\Recursos\sales.csv"))
{
    using (var csv = new CsvReader(StreamReader, CultureInfo.InvariantCulture))
    {
        csv.Context.RegisterClassMap<SalesClassMap>();
        records = csv.GetRecords<Sales>().ToList();
        Sales[] sales = records.ToArray();
        try
        {
            using (var connection = factory.CreateConnection())
            {
                var _channel = connection.CreateModel();

                //aqui recibo de gateway la transaction id
                _channel.QueueDeclare(queue: "transaction",
                          durable: false,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);

                int count = records.Count;

                Task[] tasks = new Task[count / 50 + 1];
                var body2= Encoding.UTF8.GetBytes((count / 50 + 1).ToString());

                Console.Write(count / 50 + 1);
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (model, ea) =>
                {

                    var body = ea.Body.ToArray();
                    var transactionid = Encoding.UTF8.GetString(body);

                    var transaction = JsonSerializer.Deserialize<Transaction>(transactionid);
                    Console.WriteLine(transaction);

                    var data = new DATAS
                    {
                        TransactionId = transaction.TransactionId,
                        Errors = new List<string>(), 
                        threads = count/50+1
                    };

                    Console.WriteLine(data.threads);
                 
                    _channel.QueueDeclare(queue: "init",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                    string dataJson = JsonSerializer.Serialize(data);

                    // Codificar el JSON en UTF-8
                    byte[] dataBytes = Encoding.UTF8.GetBytes(dataJson);


                    _channel.BasicPublish(exchange: "",
                               routingKey: "init",
                               basicProperties: null,
                               body: dataBytes);

                       
                        for (int i = 0; i < tasks.Length; i++)
                        {
                            int current = i;
                            tasks[i] = Task.Factory.StartNew(() =>
                            {

                                //for para enviar los mensajes 
                      
                                int max = (current < count / 50) ? 50 : count % 50;
                                for (int j = current * 50; j < current * 50 + max; j++)
                                {
                                 
                                    Console.WriteLine(j);
                                    using (var channel = connection.CreateModel())
                                    {
                                        channel.QueueDeclare(
                                            queue: current.ToString(),
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                                        string registro_json = JsonSerializer.Serialize(sales[j]);
                                        Console.WriteLine(registro_json);
                                        var body = Encoding.UTF8.GetBytes(registro_json);
                                        channel.BasicPublish(
                                          exchange: "",
                                          routingKey: current.ToString(),
                                          basicProperties: null,
                                          body: body);
                                    }
                                }
                            });
                        }
                    

                };

                _channel.BasicConsume(queue: "transaction",
                                  autoAck: true,
                                  consumer: consumer);

                Console.WriteLine("enter to exit");
                Console.ReadLine();

            }



        }
        catch (Exception ex)
        {
            Console.WriteLine("Entro");

            Console.WriteLine(ex.ToString());
        }


    }
}




public class SalesClassMap : ClassMap<Sales>
{
    public SalesClassMap()
    {
        Map(m => m.Username).Name("username");
        Map(m => m.car_id).Name("car_id");
        Map(m => m.price).Name("price");
        Map(m => m.Vin).Name("vin");
        Map(m => m.branch_office_id).Name("branch_office_id");
        Map(m => m.FirstName).Name("buyer_first_name");
        Map(m => m.LastName).Name("buyer_last_name");
        Map(m => m.BuyerId).Name("buyer_id");




    }
}


