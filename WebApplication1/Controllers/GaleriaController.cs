using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly IHostingEnvironment environment;

        public GaleriaController(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;
            repositorioGaleria = new RepositorioGaleria(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            this.environment = environment;
        }

        // GET: Galeria
        public ActionResult Index(int id)
        {
            var lista = repositorioGaleria.ObtenerTodosPorInmuebleId(id);

            if (lista.Count != 0)
            {
                if (TempData.ContainsKey("Id"))
                    ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

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
        public ActionResult Create(int id)
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Galeria\\" + id);
            Galeria g = new Galeria();
            g.Propiedad = repositorioInmueble.ObtenerPorId(id);
            g.InmuebleId = id;
            g.Ruta = path;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return View(g);
        }

        // POST: Galeria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Galeria p)
        {
            try
            {
                Galeria g = null;
                List<String> permitidos = new List<string>();
                permitidos.AddRange(configuration["Permitidos"].Split());
                long limite_kb = 600;

                for (int i = 0; i < p.Archivos.Count; i++)
                {
                    if (permitidos.Contains(p.Archivos[i].ContentType) && p.Archivos[i].Length <= limite_kb * 1024)
                    {
                        string fileName = Path.GetFileName(p.Archivos[i].FileName);
                        string pathCompleto = Path.Combine(p.Ruta, fileName);

                        if (System.IO.File.Exists(pathCompleto))
                        {
                            ViewBag.Error = "Alguno de los archivos ya existe";
                            ViewBag.Ruta = p.Ruta;
                            ViewBag.InmuebleId = p.Id;
                            return View(p);
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Alguno de los archivos no está permitido o excede el tamaño de 200 kb";
                        ViewBag.Ruta = p.Ruta;
                        ViewBag.InmuebleId = p.Id;
                        return View(p);
                    }
                }

                for (int i = 0; i < p.Archivos.Count; i++)
                {
                    g = new Galeria();
                    string fileName = Path.GetFileName(p.Archivos[i].FileName);
                    string pathCompleto = Path.Combine(p.Ruta, fileName);
                    g.Ruta = Path.Combine("\\Galeria\\" + p.Id, fileName);
                    g.InmuebleId = p.Id;

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        p.Archivos[i].CopyTo(stream);
                    }

                    repositorioGaleria.Alta(g);
                }

                TempData["Id"] = "Fotos agregadas exitosamente!";
                return RedirectToAction(nameof(Index), new { id = p.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
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
            var g = repositorioGaleria.ObtenerPorId(id);
            return View(g);
        }

        // POST: Galeria/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Galeria p)
        {
            try
            {
                var g = repositorioGaleria.ObtenerPorId(id);
                repositorioGaleria.Baja(id);
                TempData["Mensaje"] = "Imagen eliminada correctamente";
                return RedirectToAction(nameof(Index), new { id=g.InmuebleId });
            }
            catch
            {
                return View(p);
            }
        }
    }
}