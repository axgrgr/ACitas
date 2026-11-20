using System;
using System.Collections.Generic;

namespace ACitas.Models;

public partial class Cita
{
    public int CitaId { get; set; }

    public Guid Folio { get; set; }

    public int ClienteId { get; set; }

    public int ServicioId { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public string Estado { get; set; } = null!;

    public string? ComentarioCliente { get; set; }

    public string? NotaDueno { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<HistorialCita> HistorialCita { get; set; } = new List<HistorialCita>();

    public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();

    public virtual Servicio Servicio { get; set; } = null!;


    //agregue estas
    public string NombreCliente { get; set; } = null!;
    public string? CorreoCliente { get; set; }

}
