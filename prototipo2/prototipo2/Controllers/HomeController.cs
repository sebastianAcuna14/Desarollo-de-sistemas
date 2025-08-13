using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using prototipo2.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using prototipo2.Servicios;

namespace prototipo2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        private SqlConnection Conexion()
        {
            return new SqlConnection(_configuration.GetConnectionString("Connection"));
        }


        [Sesiones]
        public IActionResult Index()
        {
            try
            {
                using var con = Conexion();
                var productosDestacados = con.Query<Producto>(
                    "ObtenerProductosCatalogo",
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return View(productosDestacados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar productos destacados");
                return View(new List<Producto>());
            }
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
}