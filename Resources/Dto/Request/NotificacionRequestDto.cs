namespace ApiNotificaciones.Resources.Dto.Request;

public class NotificacionRequestDto
{
    public string? Titulo { get; set; }
    public string? Mensaje { get; set; }
    public string? Usuario { get; set; }
    public string? Rol { get; set; }
    public string? Origen { get; set; }
    public long? ReferenciaId { get; set; }
}

