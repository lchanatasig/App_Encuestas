using System;
using System.Collections.Generic;

namespace App_Encuestas.Models;

public partial class Pregunta
{
    public int PreguntaId { get; set; }

    public string Texto { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public int Orden { get; set; }

    public virtual ICollection<Respuestum> Respuesta { get; set; } = new List<Respuestum>();
}
