namespace ApiNotificaciones.Messaging;

using ApiNotificaciones.Messaging.EventBusModels;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQPublisher
{
    public async Task PublicarAsync(NotificacionEventModel evento)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "notificaciones",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(evento);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "notificaciones",
            body: body);
    }
}