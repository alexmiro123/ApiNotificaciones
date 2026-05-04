using ApiCartera.Utils;
using ApiNotificaciones.Models;
using ApiNotificaciones.Resources.Dto.Request;
using ApiNotificaciones.Resources.Dto.Response;

namespace ApiNotificaciones.Interfaces;

public interface INotificacionRabbitService
{
   
    Task<ServiceResponse<bool>>EnviarNotificacion(NotificacionRequestDto dto);
    Task<ServiceResponse<bool>> MarcarComoLeida(List<long> ids);
    Task<ServiceResponse<List<NotificacionResponseDto>>> ListarNotificacionesNoLeidas(string username);


}

