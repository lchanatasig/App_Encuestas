using System;
using System.Collections.Generic;

namespace App_Encuestas.Models;

public partial class Respuestum
{
    public int RespuestaId { get; set; }

    public int EncuestaId { get; set; }

    public int PreguntaId { get; set; }

    public string Valor { get; set; } = null!;

    public virtual Encuestum Encuesta { get; set; } = null!;

    public virtual Pregunta Pregunta { get; set; } = null!;
}
