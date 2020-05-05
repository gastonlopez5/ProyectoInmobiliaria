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
    public class GaleriaController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioGaleria repositorioGaleria;

        public GaleriaController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioGaleria = new RepositorioGaleria(configuration);
        }

        // GET: Galeria
        public ActionResult Index(int id)
        {
            var lista = repositorioGaleria.ObtenerTodosPorInmuebleId(id);

            if (lista.Count != 0)
            {
                ViewBag.Inmueble = lista[0].Propiedad;
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "No hay imágenes cargadas para el inmueble seleccionado";
                return RedirectToAction("Index", "Inmuebles");
            }
        }

        // GET: Galeria/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Galeria/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Galeria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Galeria/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Galeria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Galeria/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Galeria/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}