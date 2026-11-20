using System;
using System.Collections.Generic;

namespace ACitas.Models;

public partial class Horario
{
    public int HorarioId { get; set; }

    public byte DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool Activo { get; set; }
}
