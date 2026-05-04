namespace ApiNotificaciones.Resources.Dto.Request;

public class NotificacionRequestDto
{
    public string? Titulo { get; set; }
    public string? Mensaje { get; set; }
    public List<string>? Usuarios { get; set; }
    public string? Rol { get; set; }
    public string? Origen { get; set; }
    public string? Fecha { get; set; }
    public int? Leido { get; set; }
    public long? ReferenciaId { get; set; }
}

