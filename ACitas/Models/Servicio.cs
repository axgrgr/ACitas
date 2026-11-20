using System;
using System.Collections.Generic;

namespace ACitas.Models;

public partial class Servicio
{
    public int ServicioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public short DuracionMinutos { get; set; }

    public decimal? Precio { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
}
