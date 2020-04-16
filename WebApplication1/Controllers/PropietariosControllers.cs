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
    public class PropietariosController : Controller 
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioUsuario repositorioUsuario;

        public PropietariosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioUsuario = new RepositorioUsuario(configuration);
        }

        // GET: Propietarios
        public ActionResult Index()
        {
            var lista = repositorioPropietario.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        [AllowAnonymous]
        public ActionResult Perfil()
        {
            Propietario propietario= repositorioPropietario.ObtenerPorEmail(User.Identity.Name);
            return View(propietario);
        }

        // GET: Propietarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario p)
        {
            ViewBag.propietarios = repositorioPropietario.ObtenerTodos();

            foreach (var item in (IList<Propietario>)ViewBag.propietarios)
            {
                if (item.Email == p.Email || item.Dni == p.Dni)
                {
                    ViewBag.Error = "Error: Ya existe un propietario con ese email o dni";
                    return View();
                }
            }

            try
            {
                TempData["Nombre"] = p.Nombre;
                if (ModelState.IsValid)
                {
                    repositorioPropietario.Alta(p);
                    TempData["Id"] = "Propietario agregado exitosamente!";
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

        [AllowAnonymous]
        // GET: Propietarios/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorioPropietario.ObtenerPorId(id);
            ViewBag.Usuario = repositorioUsuario.ObtenerPorEmail(p.Email);
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(p);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Propietario p = null;
            try
            {
                p = repositorioPropietario.ObtenerPorId(id);
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                p.Telefono = collection["Telefono"];
                repositorioPropietario.Modificacion(p);
                TempData["Mensaje"] = "Datos modificados con exito!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: Propietarios/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorioPropietario.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Propietarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario p)
        {
            try
            {
                repositorioPropietario.Baja(id);
                TempData["Mensaje"] = "Propietario eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Propietario dispone de inmuebles";
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPass(int id, CambioClaveView cambio)
        {
            Usuario usuario = null;
            try
            {
                usuario = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
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
                if (ModelState.IsValid)
                {
                    usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorioUsuario.Modificacion(usuario);
                    TempData["Mensaje"] = "Contraseña actualizada correctamente";
                    return RedirectToAction(nameof(Index));
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

