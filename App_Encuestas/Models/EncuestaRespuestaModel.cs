namespace App_Encuestas.Models
{
    public class EncuestaRespuestaModel
    {
        public int EncuestaId { get; set; }
        public List<PreguntaRespuestaItem> Preguntas { get; set; }
    }

    public class PreguntaRespuestaItem
    {
        public int PreguntaId { get; set; }
        public string Texto { get; set; }
        public string Tipo { get; set; } // 'Escala', 'SiNo', 'Texto'
        public string Respuesta { get; set; }
    }

}
