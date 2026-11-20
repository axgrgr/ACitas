using System;
using System.Collections.Generic;

namespace ACitas.Models;

public partial class HistorialCita
{
    public long HistorialId { get; set; }

    public int CitaId { get; set; }

    public string? EstadoAnterior { get; set; }

    public string EstadoNuevo { get; set; } = null!;

    public DateTime FechaCambio { get; set; }

    public string? Nota { get; set; }

    public string? Usuario { get; set; }

    public virtual Cita Cita { get; set; } = null!;
}
