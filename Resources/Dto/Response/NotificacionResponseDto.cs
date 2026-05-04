namespace ApiNotificaciones.Resources.Dto.Response;

public class NotificacionResponseDto
{
    public long id { get; set; }
    public string? titulo { get; set; }
    public string? mensaje { get; set; }
    public string? usuario { get; set; }
    public string? rol { get; set; }
    public DateTime fecha { get; set; }
    public int leido { get; set; }
    public string? origen { get; set; }
    public long? referenciaId { get; set; }
}

