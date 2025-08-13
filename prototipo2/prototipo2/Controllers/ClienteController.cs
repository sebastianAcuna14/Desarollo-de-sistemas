using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;
using prototipo2.Servicios;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Azure;

namespace prototipo2.Controllers
{

    public class ClienteController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly IUtilitarios _utilitarios;

        public ClienteController(IConfiguration configuration, IUtilitarios utilitarios, IHostEnvironment environment)
        {
            _configuration = configuration;
            _utilitarios = utilitarios;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult MiPerfil(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Cliente>("ConsultarClienteID",
                new
                {
                    cliente.idCliente
                });

                if (resultado != null)
                {

                    return View(resultado);
                }

                return View();
            }
        }
        [HttpGet]
        public IActionResult EditarPerfil()
        {
            return View();
        }
        public IActionResult EditarPerfil(Cliente cliente)
        {
            {
                using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
                {
                    var resultado = context.Execute("EditarCliente",
                    new
                    {
                        cliente.idCliente,
                        cliente.Nombre,
                        cliente.Apellido,
                        cliente.Telefonos,
                        cliente.Correo,
                        cliente.Contrasena,

                    });

                    if (resultado > 0)
                        return RedirectToAction("ListaEmpleado");

                    return View(cliente);
                }
            }

        }
        [HttpGet]
        public IActionResult InicioSesion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InicioSesion(Cliente cliente, Empleado empleado)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var contrasena = _utilitarios.Encrypt(cliente.Contrasena!);
                var resultadoCliente = context.QueryFirstOrDefault<Cliente>("ValidarInicioSesion",
                    new
                    {
                        cliente.Correo,
                        contrasena
                    });
                if (resultadoCliente != null)
                {
                    var token = _utilitarios.GenerarToken(resultadoCliente.idCliente, "cliente");
                    HttpContext.Session.SetString("JWT", token);
                    HttpContext.Session.SetString("NombreUsuario", resultadoCliente.Nombre ?? "Cliente");
                    HttpContext.Session.SetString("Rol", "Cliente");
                    return RedirectToAction("Index", "Home");
                }
                var Contrasena = _utilitarios.Encrypt(empleado.Contrasena!);
                var resultadoEmpleado = context.QueryFirstOrDefault<Empleado>("ValidarInicioSesionEmpleado",
                    new
                    {
                        empleado.Correo,
                        Contrasena
                    });

                if (resultadoEmpleado != null)
                {
                    var Token = _utilitarios.GenerarToken(resultadoEmpleado.IdEmpleado, "Empleado");
                    HttpContext.Session.SetString("JWT", Token);
                    HttpContext.Session.SetString("Rol", resultadoEmpleado.NombreRol ?? "Empleado"); // "Administrador" o "Empleado"
                    HttpContext.Session.SetString("NombreUsuario", resultadoEmpleado.Nombre ?? "Empleado");

                    return RedirectToAction("Admi", "AdminController1");
                    //HttpContext.Session.SetString("JWT", Token);
                    //HttpContext.Session.SetString("Rol", resultadoEmpleado.NombreRol);
                    //HttpContext.Session.SetString("NombreUsuario", resultadoEmpleado.Nombre ?? "Empleado");
                    //return RedirectToAction("Admi", "AdminController1");
                }


                ViewBag.Mesaje = "No se pudo autenticar";
                return View();
            }

        }
        [HttpGet]
        [Sesiones]
        public async Task<IActionResult> CerrarSesion()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("InicioSesion", "Cliente");
        }
        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RecuperarAcceso(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Cliente>("ValidarCorreo",
                    new { cliente.Correo });

                if (resultado != null)
                {
                    var ContrasennaNotificar = _utilitarios.GenerarContrasenna(50);
                    var contrasena = _utilitarios.Encrypt(ContrasennaNotificar);

                    var resultadoActualizacion = context.Execute("ActualizarContrasenna",
                        new
                        {
                            resultado.idCliente,
                            contrasena
                        });

                    if (resultadoActualizacion > 0)
                    {
                        var ruta = Path.Combine(_environment.ContentRootPath, "Correos.html");
                        var html = System.IO.File.ReadAllText(ruta, UTF8Encoding.UTF8);

                        html = html.Replace("@@Usuario", resultado.Nombre);
                        html = html.Replace("@@contrasena", ContrasennaNotificar);

                        _utilitarios.EnviarCorreo(resultado.Correo!, "Recuperación de Acceso", html);
                        ViewBag.Mensaje = "Se han enviado las instrucciones de recuperación a su correo electrónico.";
                        return View();
                    }
                }

                return View();
            }
        }
        [HttpGet]
        public IActionResult RegistroCliente()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegistroCliente(Cliente cliente)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:connection").Value))
            {

                var contrasena = _utilitarios.Encrypt(cliente.Contrasena);
                var resultado = context.Execute("RegistrarCliente",
                      new
                      {
                          cliente.Nombre,
                          cliente.Apellido,
                          cliente.Cedula,
                          cliente.Correo,
                          cliente.Telefonos,
                          contrasena

                      }

                      );
                if (resultado > 0)
                {
                    return RedirectToAction("InicioSesion", "Cliente");
                }


            }
            ViewBag.Mesaje = "No se pudo registrar";
            return View();

        }






    }
}
