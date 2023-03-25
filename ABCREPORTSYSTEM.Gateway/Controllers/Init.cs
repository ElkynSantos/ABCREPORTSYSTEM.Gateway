using Microsoft.AspNetCore.Mvc;
using ABCREPORTSYSTEM.Gateway.Dtos;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace ABCREPORTSYSTEM.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Init : ControllerBase
    {


        private static readonly List<Transaction> Transactions = new List<Transaction>();



        [HttpGet]
        public async Task<ActionResult<Transaction>> Get()
        {
            var transaction = Transactions;
    

            Transaction nueva = new Transaction();
          

            string json = JsonSerializer.Serialize(nueva);

            byte[] transac = Encoding.UTF8.GetBytes(json);

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("transaction", false, false, false, null);
                        channel.BasicPublish(string.Empty, "transaction", null, transac);

                        // Declare the 'transactionfinal' queue and create a consumer for it.
                        channel.QueueDeclare("transactionfinal", false, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);
                        string result = null;
                        var taskCompletionSource = new TaskCompletionSource<string>();

                        consumer.Received += (model, ea) =>
                        {
                            var responseBytes = ea.Body.ToArray();
                            result = Encoding.UTF8.GetString(responseBytes);
                        
                            taskCompletionSource.SetResult(result);
                        };

                        channel.BasicConsume("transactionfinal", true, consumer);

                       
                        await taskCompletionSource.Task;

                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var transactionResult = JsonSerializer.Deserialize<Transaction>(result, options);

                        Transactions.Add(transactionResult);


                        //channel.QueueDelete("init");
                        return Ok(Transactions.LastOrDefault());
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }


}



