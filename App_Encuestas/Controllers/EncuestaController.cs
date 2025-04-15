using App_Encuestas.Models;
using App_Encuestas.Service;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
namespace App_Encuestas.Controllers
{
    public class EncuestaController : Controller
    {
        private readonly EncuestaService _encuestaService;
        private readonly AppEncuestasContext _context;

        public EncuestaController(EncuestaService encuestaService, AppEncuestasContext context)
        {
            _encuestaService = encuestaService;
            _context = context;
        }

        // GET: /Encuestas/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        /// <summary>
        /// Metodo generar Encuesta
        /// </summary>
        /// <param name="numeroCaso"></param>
        /// <param name="aseguradoraId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Crear(string numeroCaso, int aseguradoraId)
        {
            if (string.IsNullOrEmpty(numeroCaso))
            {
                ModelState.AddModelError("", "Debe ingresar un número de caso.");
                return View();
            }

            if (aseguradoraId <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar una aseguradora válida.");
                return View();
            }

            var encuestaId = _encuestaService.CrearEncuesta(numeroCaso, aseguradoraId, out string token);

            var link = Url.Action("Responder", "Encuesta", new { token = token }, Request.Scheme);

            ViewBag.Link = link;
            ViewBag.EncuestaId = encuestaId;

            return View("Crear");
        }


        /// <summary>
        /// Método para responder la encuesta
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Responder(string token = "", int? id = null)
        {
            int encuestaId;
            var encuesta = (Encuestum)null;

            if (!string.IsNullOrEmpty(token))
            {
                encuestaId = _encuestaService.ObtenerEncuestaIdDesdeToken(token);
                if (encuestaId == 0)
                    return RedirectToAction("ErrorSurvey");

                encuesta = _context.Encuesta.FirstOrDefault(e => e.EncuestaId == encuestaId);
            }
            else if (id.HasValue)
            {
                encuestaId = id.Value;
                encuesta = _context.Encuesta.FirstOrDefault(e => e.EncuestaId == encuestaId);
            }
            else
            {
                return RedirectToAction("ErrorSurvey");
            }

            if (encuesta == null || encuesta.Respondido == true)
            {
                return RedirectToAction("ErrorSurvey");
            }

            var modelo = _encuestaService.ObtenerPreguntas(encuestaId);
            return View(modelo);
        }


        [HttpGet]
        public IActionResult ErrorSurvey()
        {
            return View();
        }

        /// <summary>
        /// Método para guardar las respuestas de la encuesta
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Responder(EncuestaRespuestaModel modelo)
        {
            var mensajes = _encuestaService.GuardarRespuestas(modelo);

            TempData["Mensajes"] = string.Join("||", mensajes); // separador para múltiples mensajes

            return View("Gracias");
        }


        public IActionResult Gracias()
        {
            return View();
        }


        /// <summary>
        /// Vista de reporte
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DescargarReporte()
        {

            return View();
        }

        /// <summary>
        /// Exportar Excel
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExportarExcel(DateTime fechaInicio, DateTime fechaFin)
        {
            var data = _encuestaService.ObtenerReportePorRango(fechaInicio, fechaFin);

            if (data.Rows.Count == 0)
            {
                TempData["Mensaje"] = "No hay datos en el rango seleccionado.";
                return RedirectToAction("DescargarReporte");
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Respuestas");
            worksheet.Cell(1, 1).InsertTable(data);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Reporte_Encuestas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx";
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
    }
}
