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
    public class InmueblesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioGaleria repositorioGaleria;
        private readonly IHostingEnvironment environment;

        public InmueblesController(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioGaleria = new RepositorioGaleria(configuration);
            this.environment = environment;
        }

        // GET: Inmueble
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Index()
        {
            var lista = repositorioInmueble.ObtenerTodosDisponibles();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(lista);
        }

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult TodosNoDisponibles()
        {
            var lista = repositorioInmueble.ObtenerTodosNoDisponibles();
            if (lista.Count != 0)
            {
                ViewBag.Mensaje = "Listado de inmuebles NO disponibles";
                return View("Disponibles", lista);
            }
            else
            {
                TempData["Mensaje"] = "No hay Inmuebles no disponibles";
                return RedirectToAction(nameof(Index));
            }

        }


        // GET: Inmueble/Create
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Busqueda()
        {
            ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Busqueda(BusquedaInmuebleView p)
        {
            List<Inmueble> noDisponibles = new List<Inmueble>();
            List<Inmueble> disponibles = new List<Inmueble>();

            try
            {
                var listaInmuebles = repositorioInmueble.BuscarDisponibles(p.Uso, p.Importe);

                if (listaInmuebles.Count != 0)
                {
                    foreach (Inmueble i in listaInmuebles)
                    {
                        var listaContratos = repositorioContrato.ObtenerTodosPorInmueble(i.Id, p.FechaInicio, p.FechaFin);

                        if (listaContratos.Count != 0)
                        {

                            noDisponibles.Add(i);

                        }
                        else
                        {
                            disponibles.Add(i);
                        }
                    }

                    if (disponibles.Count != 0)
                    {
                        return View("Disponibles", disponibles);
                    }
                    else if (noDisponibles.Count != 0)
                    {
                        ViewBag.Mensaje = "Existen Inmuebles con contratos vigentes";  // Mostrar
                        return View("Disponibles", noDisponibles);
                    }
                }
                else
                {
                    ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                    TempData["Mensaje"] = "No hay Inmuebles disponibles.";
                    return RedirectToAction("Busqueda");
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Disponibles(List<Inmueble> lista)
        {
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }


        // GET: Inmueble/Create
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Create(int id)
        {
            ViewBag.PropietarioId = id;
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

                    if (p.Archivos != null && p.Id > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Galeria\\"+p.Id);
                        Galeria g = null;

                        List<String> permitidos = new List<string>();
                        permitidos.AddRange(configuration["Permitidos"].Split());
                        long limite_kb = 600;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        for (int i = 0; i < p.Archivos.Count; i++)
                        {
                            if (permitidos.Contains(p.Archivos[i].ContentType) && p.Archivos[i].Length <= limite_kb * 1024)
                            {
                                string fileName = Path.GetFileName(p.Archivos[i].FileName);
                                string pathCompleto = Path.Combine(path, fileName);

                                if (System.IO.File.Exists(pathCompleto))
                                {
                                    ViewBag.Error = "Alguno de los archivos ya existe";
                                    ViewBag.PropietarioId = p.PropietarioId;
                                    ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                                    repositorioInmueble.Baja(p.Id);
                                    return View(p);
                                }
                            }
                            else
                            {
                                ViewBag.Error = "Alguno de los archivos no está permitido o excede el tamaño de 200 kb";
                                ViewBag.PropietarioId = p.PropietarioId;
                                ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                                repositorioInmueble.Baja(p.Id);
                                return View(p);
                            }
                        }

                        for (int i = 0; i < p.Archivos.Count; i++)
                        {
                            g = new Galeria();
                            string fileName = Path.GetFileName(p.Archivos[i].FileName);
                            string pathCompleto = Path.Combine(path, fileName);
                            g.Ruta = Path.Combine("\\Galeria\\" + p.Id, fileName);
                            g.InmuebleId = p.Id;

                            using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                            {
                                p.Archivos[i].CopyTo(stream);
                            }

                            repositorioGaleria.Alta(g);
                        }

                        TempData["Id"] = "Inmueble agregado exitosamente!";
                        return RedirectToAction("Index", "Propietarios");
                    }
                    else
                    {
                        TempData["Error"] = "Debe agregar fotos al Inmueble!";
                        repositorioInmueble.Baja(p.Id);
                        return RedirectToAction("Index", "Propietarios");
                    }
                }
                else
                {
                    ViewBag.PropietarioId = p.PropietarioId;
                    ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                    return View(p);
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
                TempData["Mensaje"] = "Datos guardados correctamente";
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
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Inmueble/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {
                repositorioInmueble.Baja(id);
                TempData["Mensaje"] = "Inmueble eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: Inmueble/Delete/5
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult GenerarContrato(int id)
        {
            return RedirectToAction("Create", "Contratos", new { id = id });
        }

        public ActionResult InmueblesPorPropietario(int id)
        {
            IList<Inmueble> lista = null;

            if (User.IsInRole("Propietario"))
            {
                Propietario p = repositorioPropietario.ObtenerPorEmail(User.Identity.Name);
                lista = repositorioInmueble.BuscarPorPropietario(p.Id);
            }
            else
            {
                lista = repositorioInmueble.BuscarPorPropietario(id);
            }

            if (lista.Count() != 0)
            {
                ViewBag.Propietario = lista[0].Duenio;
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "El Propietario no tiene Inmuebles registrados en el sistema";
                return RedirectToAction("Index", "Propietarios");
            }
        }

        public ActionResult ListarContratos(int id)
        {
            var lista = repositorioContrato.ObtenerTodosPorInmuebleId(id);

            if (lista.Count != 0)
            {
                ViewBag.Contrato = lista[0];
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "El Inmueble no tiene contratos registrados en el sistema";
                return RedirectToAction("Index");
            }
        }
    }
}