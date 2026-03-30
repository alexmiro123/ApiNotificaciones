using ApiCartera.Utils;
using ApiNotificaciones.Models;
using ApiNotificaciones.Resources.Dto.Request;

namespace ApiNotificaciones.Interfaces;

public interface INotificacionRabbitService
{
   
    Task<ServiceResponse<bool>>EnviarNotificacion(NotificacionRequestDto dto);
}

