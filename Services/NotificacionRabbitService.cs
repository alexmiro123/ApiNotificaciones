using ApiCartera.Utils;
using ApiNotificaciones.Hubs;
using ApiNotificaciones.Interfaces;
using ApiNotificaciones.Models;
using ApiNotificaciones.Resources.Dto.Request;
using ApiNotificaciones.Resources.Dto.Response;
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
                NOTR_USUARIO = dto.Usuarios != null
                    ? string.Join(",", dto.Usuarios)
                    : null,
                NOTR_ROL = dto.Rol,
                NOTR_FECHA = DateTime.Now,
                NOTR_LEIDO = 0,
                NOTR_ESTADO = 1,
                NOTR_ORIGEN = dto.Origen,
                NOTR_REFERENCIA_ID = dto.ReferenciaId
            };

            _context.NOTIFICACIONRABBIT.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = new NotificacionResponseDto
            {
                id = entity.NOTR_ID,
                titulo = entity.NOTR_TITULO,
                mensaje = entity.NOTR_MENSAJE,
                usuario = entity.NOTR_USUARIO,
                rol = entity.NOTR_ROL,
                fecha = entity.NOTR_FECHA,
                leido = entity.NOTR_LEIDO,
                origen = entity.NOTR_ORIGEN,
                referenciaId = entity.NOTR_REFERENCIA_ID
            };

            // SignalR
            if (dto.Usuarios != null && dto.Usuarios.Any())
            {
                await _hub.Clients.Groups(dto.Usuarios)
                    .SendAsync("RecibirNotificacion", resultDto);
            }

            /*if (!string.IsNullOrEmpty(dto.Rol))
            {
                await _hub.Clients.Group(dto.Rol)
                    .SendAsync("RecibirNotificacion", dto);
            }*/

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

    public async Task<ServiceResponse<List<NotificacionResponseDto>>> ListarNotificacionesNoLeidas(string username)
    {
        var response = new ServiceResponse<List<NotificacionResponseDto>>();

        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                response.Success = false;
                response.Message = "Usuario inválido";
                return response;
            }

            username = username.ToUpper().Trim();

            var data = await _context.NOTIFICACIONRABBIT
                .AsNoTracking()
                .Where(x =>
                    x.NOTR_ESTADO == 1 &&
                    x.NOTR_LEIDO == 0 &&
                    x.NOTR_USUARIO != null &&
                    ("," + x.NOTR_USUARIO + ",").Contains("," + username + ",")
                )
                .OrderByDescending(x => x.NOTR_FECHA)
                .Select(x => new NotificacionResponseDto
                {
                    id = x.NOTR_ID,
                    titulo = x.NOTR_TITULO,
                    mensaje = x.NOTR_MENSAJE,
                    fecha = x.NOTR_FECHA,
                    leido = x.NOTR_LEIDO,
                    origen = x.NOTR_ORIGEN,
                    referenciaId = x.NOTR_REFERENCIA_ID
                })
                .ToListAsync();

            response.Data = data;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> MarcarComoLeida(List<long> ids)
    {
        var response = new ServiceResponse<bool>();

        if (ids == null || !ids.Any())
        {
            response.Success = false;
            response.Message = "Debe enviar al menos un id";
            return response;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Obtener registros
            var notificaciones = await _context.NOTIFICACIONRABBIT
                .Where(x => ids.Contains(x.NOTR_ID) && x.NOTR_LEIDO == 0)
                .ToListAsync();

            if (!notificaciones.Any())
            {
                response.Success = true;
                response.Data = true;
                response.Message = "No hay registros para actualizar";
                return response;
            }

            // Actualizar estado
            foreach (var item in notificaciones)
            {
                item.NOTR_LEIDO = 1;
            }

            // Guardar cambios
            await _context.SaveChangesAsync();

            // Commit
            await transaction.CommitAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "Notificaciones marcadas como leídas";
        }
        catch (Exception ex)
        {
            // Rollback
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = ex.InnerException?.Message ?? ex.Message;
        }

        return response;
    }
}

