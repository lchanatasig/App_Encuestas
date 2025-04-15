using App_Encuestas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace App_Encuestas.Service
{
    public class EncuestaService
    {


        private readonly AppEncuestasContext _context;

        public EncuestaService(AppEncuestasContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método para crear una encuesta
        /// </summary>
        /// <param name="numeroCaso"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public int CrearEncuesta(string numeroCaso, int aseguradoraId, out string token)
        {
            int encuestaId = 0;
            token = "";

            using var conn = _context.Database.GetDbConnection() as SqlConnection;
            using var cmd = new SqlCommand("sp_CrearEncuesta", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@numero_caso", numeroCaso);
            cmd.Parameters.AddWithValue("@aseguradora_id", aseguradoraId);

            var tokenParam = new SqlParameter("@token", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            var idParam = new SqlParameter("@encuesta_id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(tokenParam);
            cmd.Parameters.Add(idParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            token = tokenParam.Value?.ToString();
            encuestaId = (int)idParam.Value;

            return encuestaId;
        }

        /// <summary>
        /// Método para obtener las preguntas de la encuesta
        /// </summary>
        /// <param name="encuestaId"></param>
        /// <returns></returns>
        public EncuestaRespuestaModel ObtenerPreguntas(int encuestaId)
        {
            var preguntas = _context.Preguntas
                .OrderBy(p => p.Orden)
                .Select(p => new PreguntaRespuestaItem
                {
                    PreguntaId = p.PreguntaId,
                    Texto = p.Texto,
                    Tipo = p.Tipo 
                }).ToList();

            return new EncuestaRespuestaModel
            {
                EncuestaId = encuestaId,
                Preguntas = preguntas
            };
        }


        /// <summary>
        /// Método para guardar las respuestas de la encuesta
        /// </summary>
        /// <param name="modelo"></param>
        public List<string> GuardarRespuestas(EncuestaRespuestaModel modelo)
        {
            var resultados = new List<string>();
            using var conn = _context.Database.GetDbConnection() as SqlConnection;
            conn.Open();

            foreach (var item in modelo.Preguntas)
            {
                using var cmd = new SqlCommand("sp_InsertarRespuesta", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@encuesta_id", modelo.EncuestaId);
                cmd.Parameters.AddWithValue("@pregunta_id", item.PreguntaId);
                cmd.Parameters.AddWithValue("@valor", item.Respuesta ?? "");

                var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 250)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(mensajeParam);

                cmd.ExecuteNonQuery();
                resultados.Add(mensajeParam.Value?.ToString());
            }

            return resultados;
        }


        /// <summary>
        /// Método para obtener el ID de la encuesta desde el token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int ObtenerEncuestaIdDesdeToken(string token)
        {
            var encuesta = _context.Encuesta.FirstOrDefault(e => e.Token == token);
            return encuesta?.EncuestaId ?? 0;
        }


        public DataTable ObtenerReportePorRango(DateTime fechaInicio, DateTime fechaFin)
        {
            var tabla = new DataTable();

            using var conn = _context.Database.GetDbConnection() as SqlConnection;
            using var cmd = new SqlCommand("sp_ObtenerRespuestasPivot", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
            cmd.Parameters.AddWithValue("@fechaFin", fechaFin);

            conn.Open();

            using var reader = cmd.ExecuteReader();
            tabla.Load(reader);

            return tabla;
        }

    }
}
