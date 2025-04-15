using System;
using System.Collections.Generic;

namespace App_Encuestas.Models;

public partial class Encuestum
{
    public int EncuestaId { get; set; }

    public string NumeroCaso { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public string? Token { get; set; }

    public bool? Respondido { get; set; }

    public int? AseguradoraId { get; set; }

    public virtual Aseguradora? Aseguradora { get; set; }

    public virtual ICollection<Respuestum> Respuesta { get; set; } = new List<Respuestum>();
}
