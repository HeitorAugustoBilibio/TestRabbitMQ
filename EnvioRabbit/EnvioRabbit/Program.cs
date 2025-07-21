using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class Program
{
    public static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = await factory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync("Ex", ExchangeType.Direct);
        await channel.QueueDeclareAsync(queue: "fila", durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync("fila", "Ex", "routingKey", null);

        while (true)
        {
            Console.WriteLine("Escreva uma mensagem");
            var message = Console.ReadLine();

            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Fechando a Aplicação");
                break;
            }

            var body = Encoding.UTF8.GetBytes(message);
            var props = new BasicProperties();

            await channel.BasicPublishAsync(exchange: "Ex",
                                 routingKey: "routingKey",
                                 false,
                                 basicProperties: props,
                                 body: body);

            Console.WriteLine($"Enviando a mensagem {message}");
        }
       


    }
}
