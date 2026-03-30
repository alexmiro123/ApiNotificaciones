namespace ApiCartera.Resources.Base;


public abstract class AuditableEntity
{
    public string? CREA_USR { get; set; }
    public DateTime? CREA_FECHA { get; set; }

    public string? MOD_USR { get; set; }
    public DateTime? MOD_FECHA { get; set; }
}


