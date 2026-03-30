using ApiNotificaciones.Interfaces;
using ApiNotificaciones.Messaging.EventBusModels;
using ApiNotificaciones.Resources.Dto.Request;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMQConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: "notificaciones",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var evento = JsonSerializer.Deserialize<NotificacionEventModel>(json);

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<INotificacionRabbitService>();

            await service.EnviarNotificacion(new NotificacionRequestDto
            {
                Titulo = evento.Titulo,
                Mensaje = evento.Mensaje,
                Usuario = evento.Usuario,
                Rol = evento.Rol
            });

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(
            queue: "notificaciones",
            autoAck: true,
            consumer: consumer);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }
}