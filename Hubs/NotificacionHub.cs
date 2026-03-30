using Microsoft.AspNetCore.SignalR;

namespace ApiNotificaciones.Hubs;

public class NotificacionHub : Hub
{
    public async Task UnirseUsuario(string usuario)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, usuario);
    }

    public async Task UnirseRol(string rol)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, rol);
    }
}

