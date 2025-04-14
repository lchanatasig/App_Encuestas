using App_Encuestas.Models;
using App_Encuestas.Service;
using Microsoft.AspNetCore.Mvc;

namespace App_Encuestas.Controllers
{
    public class EncuestaController : Controller
    {
        private readonly EncuestaService _encuestaService;

        public EncuestaController(EncuestaService encuestaService)
        {
            _encuestaService = encuestaService;
        }

        // GET: /Encuestas/Crear
        [HttpGet]   
        public IActionResult Crear()
        {
            return View();
        }

        /// <summary>
        /// POST: /Encuestas/Crear
        /// </summary>
        /// <param name="numeroCaso"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Crear(string numeroCaso)
        {
            if (string.IsNullOrEmpty(numeroCaso))
            {
                ModelState.AddModelError("", "Debe ingresar un número de caso.");
                return View();
            }

            var encuestaId = _encuestaService.CrearEncuesta(numeroCaso, out string token);

            // Construimos el link para enviar al cliente
            var link = Url.Action("Responder", "Encuesta", new { token = token }, Request.Scheme);

            // Enviamos datos a la vista
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

            if (!string.IsNullOrEmpty(token))
            {
                encuestaId = _encuestaService.ObtenerEncuestaIdDesdeToken(token);
                if (encuestaId == 0)
                    return NotFound("Token inválido o expirado.");
            }
            else if (id.HasValue)
            {
                encuestaId = id.Value;
            }
            else
            {
                return BadRequest("No se proporcionó un identificador válido.");
            }

            var modelo = _encuestaService.ObtenerPreguntas(encuestaId);
            return View(modelo);
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

            return RedirectToAction("Gracias");
        }


        public IActionResult Gracias()
        {
            return Content("¡Gracias por responder la encuesta!");
        }

    }
}
