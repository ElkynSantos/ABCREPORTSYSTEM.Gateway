using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Collections.Generic;
using CsvHelper;
using System.Data.Common;
using System.Threading.Channels;
using Newtonsoft.Json;
using System.Globalization;
using RabbitMQ.Client.Events;
using RECOLLECTOR2.Dtos;
namespace RECOLLECTOR2
{
    public class RECOLECTOR : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer Consumer;
        private readonly string _queueName = "transaction";

        public RECOLECTOR()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_queueName, false, false, false, null);
             Consumer = new EventingBasicConsumer(_channel);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Consumer.Received += async (model, content) =>
            {
                await ProcessCsvFile();
            };
            _channel.BasicConsume(_queueName, true, Consumer);
            return Task.CompletedTask;
        }


        private async Task NotificarVAL(List<Sales> batch)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("list", false, false, false, null);
                    var json = JsonConvert.SerializeObject(batch);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(string.Empty, "list", null, body);
                }
            }
        }

        private async Task ProcessCsvFile()
        {
            using var reader = new StreamReader(@"C:\Users\elkyn\source\repos\ABCREPORTSYSTEM.Gateway\Recursos\Recursos\sales.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Sales>().ToList();


            var batch = records.Take(records.Count()).ToList();
            await NotificarVAL(batch);
            /* for (int i = 0; i < records.Count; i += 50)
             {
                 var batch = records.Skip(i).Take(50).ToList();
                 await NotificarVAL(batch);
             }*/
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}

