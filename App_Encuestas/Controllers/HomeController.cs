using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App_Encuestas.Models;

namespace App_Encuestas.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppEncuestasContext _context;

    public HomeController(ILogger<HomeController> logger, AppEncuestasContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var aseguradorasActivas = _context.Aseguradoras
            .Where(a => a.Estado == true)
            .OrderBy(a => a.Nombre)
            .ToList();

        ViewBag.Aseguradoras = aseguradorasActivas;
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
