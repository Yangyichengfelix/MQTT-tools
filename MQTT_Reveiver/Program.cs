using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Xml.Linq;

namespace MQTT_Reveiver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "192.168.1.40",  UserName="yicheng", Password="yicheng"  };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "NEW",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    var consumer = new EventingBasicConsumer(channel);


                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
             

                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        JsonSerializerSettings settings = new JsonSerializerSettings
                        {
                            DateParseHandling = DateParseHandling.None,
                        };
                        try
                        {

                        var obje = JsonConvert.DeserializeObject<Payload>(message, settings);
                        Console.WriteLine(" obje name is null ? " + ( obje.Name));
                        Console.WriteLine(" obje timeStamp  " + obje.Timestamp.ToString("f"));
                        Console.WriteLine(" obje s1  "+ obje.Sensor1);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);


                    };
                    channel.BasicConsume(queue: "NEW",
                                                             autoAck: false,//making it false as we are doing it manually above
                                                             consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
        public async Task Insert(string mac2sName)
        {
            await using var con = new NpgsqlConnection($"Host=192.168.1.40;Port=5432;Database={mac2sName};Username=admin;Password=123456;");
            await con.OpenAsync();
 
            await using (var cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "LISTEN lastworkchange;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
            await con.CloseAsync();
        }
        private class Payload
        {
            public string Name { get; set; }
            public DateTime Timestamp { get; set; }
            public float Sensor1 { get; set; }

        }
    }
    
}