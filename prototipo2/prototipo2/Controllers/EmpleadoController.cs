using Dapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using prototipo2.Models;
using prototipo2.Servicios;
using System.Linq;
namespace prototipo2.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilitarios _utilitarios;
        public EmpleadoController(IConfiguration configuration, IUtilitarios utilitarios)
        {
            _configuration = configuration;
            _utilitarios = utilitarios;

        }



        [Sesiones]
        public IActionResult CrearEmpleado()
        {
            return View();
        }
        [Sesiones]
        public IActionResult ListaEmpleado()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Query<Empleado>("ObtenerEmpleado").ToList();
                return View(resultado);
            }
        }
        [HttpGet]
        public IActionResult EditarEmpleado(int id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Empleado>("ConsultarEmpleadoID",
                    new
                    {
                        IdEmpleado = id
                    });

                return View(resultado);
            }
        }
        [HttpPost]
        public IActionResult EditarEmpleado(Empleado empleado)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Execute("EditarEmpleado",
                new
                {
                    empleado.IdEmpleado,
                    empleado.Nombre,
                    empleado.Apellido,
                    empleado.Cedula,
                    empleado.Telefono,
                    empleado.Correo,
                    empleado.Contrasena,
                    empleado.IdRol
                });

                if (resultado > 0)
                    return RedirectToAction("ListaEmpleado");

                return View(empleado);
            }
        }
        [HttpPost]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var contrasena = _utilitarios.Encrypt(empleado.Contrasena);
                var resultado = context.Execute("RegistrarEmpleado",
                          new
                          {
                              empleado.Nombre,
                              empleado.Apellido,
                              empleado.Cedula,
                              empleado.Telefono,
                              empleado.Correo,
                              contrasena



                          }

                          );
                if (resultado > 0)
                {
                    return RedirectToAction("ListaEmpleado", "Empleado");
                }

                return View(empleado);
            }
        }

        [HttpGet]
        public IActionResult EliminarEmpleado(int id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Empleado>("ConsultarEmpleadoID",
                    new
                    {
                        IdEmpleado = id
                    });

                return View(resultado);
            }
        }

        [HttpPost]
        public IActionResult EliminarEmpleado(Empleado empleado)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.Execute("EliminarEmpleado",
                      new
                      {
                          empleado.IdEmpleado
                      });


                return RedirectToAction("ListaEmpleado");

            }
        }

    }


}

