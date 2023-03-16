using Microsoft.AspNetCore.Mvc;
using ABCREPORTSYSTEM.Gateway.Dtos;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;

namespace ABCREPORTSYSTEM.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Init : ControllerBase
    {

        private static readonly List<ABCREPORTSYSTEM.Gateway.Dtos.Error> Errors = new List<ABCREPORTSYSTEM.Gateway.Dtos.Error>
        {
            new Dtos.Error()
            {
                Id = 1,
                Description = "El empleado no existe en la sucursal"
            },
            new Dtos.Error()
            {
                Id = 2,
                Description = "El auto no existe en la sucursal"
            },
            new Dtos.Error()
            {
                Id = 3,
                Description = "El VIN del auto no es valido"
            },
            new Dtos.Error()
            {
                Id = 4,
                Description = "El apellido y ID del comprador esta vacio"
            }
        };
        private static readonly List<Transaction> Transactions = new List<Transaction>
{
         new Transaction()
         {
                TransactionId = Guid.NewGuid(),
                Errors = Errors
            }
        };


        [HttpGet]

        public  ActionResult<Transaction> Get()
        {
            var transaction = Transactions;
            Transaction body = null;

            foreach (var t in transaction)
            {
                body = t;
            }

            string json = JsonSerializer.Serialize(body);

            byte[] transac = Encoding.UTF8.GetBytes(json);
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("transaction", false, false, false, null);
                        channel.BasicPublish(string.Empty, "transaction", null, transac);
                    }
                }

                return Ok(body.TransactionId);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }


}



