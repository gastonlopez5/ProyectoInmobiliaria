using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPropietario repositorioPropietario;
        public InmueblesController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
        }

        // GET: Inmueble
        public ActionResult Index()
        {
            var lista = repositorioInmueble.ObtenerTodos();
            if (TempData.ContainsKey("Alta"))
                ViewBag.Alta = TempData["Alta"];
            return View(lista);
        }


        // GET: Inmueble/Create
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Create()
        {
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioInmueble.Alta(p);
                    TempData["Alta"] = "Inmueble agregado exitosamente!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                    ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Inmueble/Edit/5
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Edit(int id)
        {
            var p = repositorioInmueble.ObtenerPorId(id);
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                entidad.Id = id;
                repositorioInmueble.Modificacion(entidad);
                TempData["Alta"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: Inmueble/Delete/5
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {
                repositorioInmueble.Baja(id);
                TempData["Alta"] = "Inmueble eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}