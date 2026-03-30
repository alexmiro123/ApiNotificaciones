using ApiCartera.Utils;
using ApiNotificaciones.Hubs;
using ApiNotificaciones.Interfaces;
using ApiNotificaciones.Models;
using ApiNotificaciones.Resources.Dto.Request;
using ApiTalentoHumano.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ApiNotificaciones.Services;

public class NotificacionRabbitService : INotificacionRabbitService
{
    private readonly IHubContext<NotificacionHub> _hub;
    private readonly ApplicationDbContext _context;

    public NotificacionRabbitService(
        IHubContext<NotificacionHub> hub,
        ApplicationDbContext context)
    {
        _hub = hub;
        _context = context;
    }


    public async Task<ServiceResponse<bool>> EnviarNotificacion(NotificacionRequestDto dto)
    {
        var response = new ServiceResponse<bool>();

        try
        {

            var entity = new NotificacionRabbitModel
            {
                NOTR_TITULO = dto.Titulo,
                NOTR_MENSAJE = dto.Mensaje,
                NOTR_USUARIO = dto.Usuario,
                NOTR_ROL = dto.Rol,
                NOTR_FECHA = DateTime.Now,
                NOTR_LEIDO = 0,
                NOTR_ESTADO = 1,
                NOTR_ORIGEN = dto.Origen,
                NOTR_REFERENCIA_ID = dto.ReferenciaId
            };

            _context.NOTIFICACIONRABBIT.Add(entity);
            await _context.SaveChangesAsync();

            // SignalR
            if (!string.IsNullOrEmpty(dto.Usuario))
            {
                await _hub.Clients.Group(dto.Usuario)
                    .SendAsync("RecibirNotificacion", dto);
            }

            if (!string.IsNullOrEmpty(dto.Rol))
            {
                await _hub.Clients.Group(dto.Rol)
                    .SendAsync("RecibirNotificacion", dto);
            }

            response.Data = true;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return response;
    }
}

