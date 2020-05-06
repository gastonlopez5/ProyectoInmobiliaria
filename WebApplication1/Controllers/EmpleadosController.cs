using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "EsDeLaCasa")]
    public class EmpleadosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioEmpleado repositorioEmpleado;
        private readonly RepositorioUsuario repositorioUsuario;

        public EmpleadosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioEmpleado = new RepositorioEmpleado(configuration);
            repositorioUsuario = new RepositorioUsuario(configuration);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var lista = repositorioEmpleado.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: Empleados
        public ActionResult Perfil()
        {
            Empleado empleado = repositorioEmpleado.ObtenerPorEmail(User.Identity.Name);
            return View(empleado);
        }

        // GET: Empleados/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empleado p)
        {
            ViewBag.empleados = repositorioEmpleado.ObtenerTodos();

            foreach (var item in (IList<Empleado>)ViewBag.empleados)
            {
                if (item.Email == p.Email || item.Dni == p.Dni)
                {
                    ViewBag.Error = "Error: Ya existe un empleado con ese email o dni";
                    return View();
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    p.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: p.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    Usuario u = new Usuario();
                    u.Email = p.Email;
                    u.RolId = 2;
                    u.Clave = p.Clave;

                    repositorioUsuario.Alta(u);
                    repositorioEmpleado.Alta(p);
                    TempData["Id"] = "Empleado agregado exitosamente!";
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorioEmpleado.ObtenerPorId(id);
            ViewBag.Usuario = repositorioUsuario.ObtenerPorEmail(p.Email);
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(p);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Empleado p = null;
            Usuario u = null;
            try
            {
                p = repositorioEmpleado.ObtenerPorId(id);
                u = repositorioUsuario.ObtenerPorEmail(p.Email);
                
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                p.Telefono = collection["Telefono"];

                u.Email = p.Email;

                repositorioUsuario.Modificacion(u);
                repositorioEmpleado.Modificacion(p);

                if (User.IsInRole("Empleado"))
                {
                    TempData["Mensaje"] = "Datos guardados correctamente. Ingresar nuevamente por favor.";
                    return RedirectToAction("Logout", "Usuario");
                }
                else
                {
                    TempData["Mensaje"] = "Datos guardados correctamente";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: Empleados/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var p = repositorioEmpleado.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Empleados/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Empleado p)
        {
            Usuario u = null;

            try
            {
                p = repositorioEmpleado.ObtenerPorId(id);
                u = repositorioUsuario.ObtenerPorEmail(p.Email);

                repositorioUsuario.Baja(u.Id);
                repositorioEmpleado.Baja(id);
                TempData["Mensaje"] = "Empleado eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPass(int id, CambioClaveView cambio)
        {
            Usuario usuario = null;
            Empleado empleado = null;

            try
            {
                empleado = repositorioEmpleado.ObtenerPorId(id);
                usuario = repositorioUsuario.ObtenerPorEmail(empleado.Email);

                if (User.IsInRole("Empleado"))
                {
                    // verificar clave antigüa
                    var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: cambio.ClaveVieja ?? "",
                            salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
                    if (usuario.Clave != pass)
                    {
                        TempData["Error"] = "Clave incorrecta";
                        //se rederige porque no hay vista de cambio de pass, está compartida con Edit
                        return RedirectToAction("Edit", new { id = id });
                    }
                }
               
                if (ModelState.IsValid)
                {
                    usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    
                    repositorioUsuario.Modificacion(usuario);
                    TempData["Mensaje"] = "Contraseña actualizada correctamente. Ingresar nuevamente por favor";
                    return RedirectToAction("Logout", "Usuario");
                }
                else
                {
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Error"] += error.ErrorMessage + "\n";
                        }
                    }
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction("Edit", new { id = id });
            }
        }
    }
}