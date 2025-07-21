using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RecebeRabbit
{
    public class Program
    {
        public static async Task Main()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "fila", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("Aguardando mensagens na fila...");  

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                await Task.Delay(1000);
                var message = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine($"Mensagem recebida: {message}");
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("fila", false, consumer);

            Console.ReadLine();



        }
    }
}
