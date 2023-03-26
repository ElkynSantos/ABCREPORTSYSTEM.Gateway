using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using ValidatorService3.Dtos;
using System.Net.Http;
namespace ValidatorService3;

public class VALIDATOR : BackgroundService
{

    private readonly IConnection _connection;

    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    private static int i = 0;

    HttpClient client = new HttpClient();
    public VALIDATOR()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("list", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
     

    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var salesList = System.Text.Json.JsonSerializer.Deserialize<List<Sales>>(json);
           var error = await Validator(salesList,cancellationToken);

   ;
           await NotificarTransactionFinal(error);
        };
        _channel.BasicConsume("list", true, _consumer);

        return Task.CompletedTask;
    }
    private async Task<List<string>> Validator(List<Sales> sale,
     CancellationToken token)
    {
     
        var errors = new List<string>();

        for (int i = 0; i < sale.Count; i++)
        {
         
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7073/SucursalC/Automobile/{sale[i].vin}/{sale[i].branch_office_id}");
            HttpResponseMessage response2 = await client.GetAsync($"https://localhost:7073/SucursalC/Employee/{sale[i].username}/{sale[i].branch_office_id}");
            string responseContent = await response.Content.ReadAsStringAsync();
            string responseContent2 = await response2.Content.ReadAsStringAsync();
            if (responseContent !="")
            {
                errors.Add(responseContent);
            }
            if (responseContent2 != "")
            {
                errors.Add(responseContent2);
            }


        }
        await Task.Delay(2000, token);

 


        return errors;
    }

    private async Task NotificarTransactionFinal(List<string> stringList)
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
                channel.QueueDeclare("transactionfinal", false, false, false, null);

                var jsonString = JsonConvert.SerializeObject(stringList);
                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.BasicPublish("", "transactionfinal", null, body);
            }
        }
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}

