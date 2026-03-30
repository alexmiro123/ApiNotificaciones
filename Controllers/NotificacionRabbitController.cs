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


}

