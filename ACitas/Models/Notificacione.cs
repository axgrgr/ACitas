using System;
using System.Collections.Generic;

namespace ACitas.Models;

public partial class Notificacione
{
    public long NotificacionId { get; set; }

    public int CitaId { get; set; }

    public string Canal { get; set; } = null!;

    public string Destino { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string EstadoEnvio { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public string? DetalleError { get; set; }

    public virtual Cita Cita { get; set; } = null!;
}
