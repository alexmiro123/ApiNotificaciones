using ApiNotificaciones.Interfaces;
using ApiNotificaciones.Resources.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiNotificaciones.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificacionRabbitController : ControllerBase
{

    private readonly INotificacionRabbitService _notificacionRabbitService;

    public NotificacionRabbitController(INotificacionRabbitService notificacionRabbitService)
    {
        _notificacionRabbitService = notificacionRabbitService;
    }

    [AllowAnonymous]
    [HttpPost("SendNotificationSignalR")]
    public async Task<IActionResult> SendNotificationSignalR([FromBody] NotificacionRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new
            {
                success = false,
                message = "Datos inválidos"
            });

        var response = await _notificacionRabbitService.EnviarNotificacion(dto);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        return Ok(response);

    }

    [AllowAnonymous]
    [HttpPost("MarcarComoLeida")]
    public async Task<IActionResult> MarcarComoLeida([FromBody] MarcarLeidasRequestDto dto)
    {
        if (!ModelState.IsValid || dto == null || dto.Ids == null || !dto.Ids.Any())
        {
            return BadRequest(new
            {
                success = false,
                message = "Debe enviar una lista válida de IDs"
            });
        }

        var response = await _notificacionRabbitService.MarcarComoLeida(dto.Ids);

        response.Status = Response.StatusCode;

        if (response.Success)
            return Ok(response);

        return BadRequest(response);
    }

    [AllowAnonymous]
    [HttpGet("GetListNotificacionesNoLeidas/{username}")]
    public async Task<IActionResult> GetListAprobacionHorasExtra(string username)
    {
        var response = await _notificacionRabbitService.ListarNotificacionesNoLeidas(username);
        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        return BadRequest(response);
    }

}

