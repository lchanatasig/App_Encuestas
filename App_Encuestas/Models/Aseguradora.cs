using System;
using System.Collections.Generic;

namespace App_Encuestas.Models;

public partial class Aseguradora
{
    public int AseguradoraId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<Encuestum> Encuesta { get; set; } = new List<Encuestum>();
}
