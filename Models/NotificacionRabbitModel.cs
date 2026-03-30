using ApiCartera.Resources.Base;

namespace ApiNotificaciones.Models;

public class NotificacionRabbitModel
{
    public long NOTR_ID { get; set; }
    public string? NOTR_TITULO { get; set; }
    public string? NOTR_MENSAJE { get; set; }
    public string? NOTR_TIPO { get; set; }
    public string? NOTR_USUARIO { get; set; }
    public string? NOTR_ROL { get; set; }
    public string? NOTR_URL { get; set; }
    public string? NOTR_ORIGEN { get; set; }
    public long? NOTR_REFERENCIA_ID { get; set; }
    public int NOTR_LEIDO { get; set; }
    public DateTime NOTR_FECHA { get; set; }
    public DateTime? NOTR_FECHA_LEIDO { get; set; }
    public int NOTR_ESTADO { get; set; }
}

