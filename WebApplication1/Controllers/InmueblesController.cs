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
        private readonly RepositorioContrato repositorioContrato;

        public InmueblesController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPropietario = new RepositorioPropietario(configuration);
            repositorioContrato = new RepositorioContrato(configuration);
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
        public ActionResult NoDisponibles()
        {
            var lista = repositorioInmueble.ObtenerTodosDisponibles();
            if (lista.Count != 0)
            {
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "No hay Inmuebles no disponibles";
                return RedirectToAction(nameof(Index));
            }
            
        }

        

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult DisponibilidadPorFecha(int id)
        {
            ViewBag.InmuebleId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult DisponibilidadPorFecha(Contrato p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var lista = repositorioContrato.ObtenerTodosPorInmueble(p.InmuebleId);
                    DateTime d1 = p.FechaInicio;
                    DateTime d2 = p.FechaFin;

                    if (lista.Count != 0)
                    {
                        bool disponible = true;

                        foreach (Contrato c in lista)
                        {
                            if (c.FechaInicio >= d1 || d2 <= c.FechaFin)
                            {
                                disponible = false;
                            }
                        }

                        if (disponible)
                        {
                            TempData["Mensaje"] = "Inmueble disponible para alquilar";
                        }
                        else
                        {
                            TempData["Error"] = "Inmueble NO disponible para alquilar";
                        }
                    }
                    else
                    {
                        TempData["Mensaje"] = "El Inmueble no tiene contratos registrados en el sistema";
                        return RedirectToAction("Disponibles", new { lista = TempData["Lista"] });
                    }
                   
                    return RedirectToAction("Disponibles", new { lista = TempData["Lista"] });
                }
                else
                {
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

        // GET: Inmueble/Create
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Busqueda()
        {
            ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Busqueda(BusquedaInmuebleView p)
        {
            IList<Inmueble> disponibles = new List<Inmueble>();
            int count = 0;

            try
            {
                var listaInmuebles = repositorioInmueble.BuscarDisponibles(p.Tipo, p.Uso, p.Ambientes, p.Importe);

                if (listaInmuebles.Count != 0)
                {
                    foreach (Inmueble i in listaInmuebles)
                    {
                        var listaContratos = repositorioContrato.ObtenerTodosPorInmueble(i.Id);

                        if (listaContratos.Count != 0)
                        {
                            foreach (Contrato c in listaContratos)
                            {
                                if (c.FechaInicio < p.FechaInicio && c.FechaFin > p.FechaInicio && c.FechaFin < p.FechaFin || 
                                    c.FechaInicio > p.FechaInicio && c.FechaInicio < p.FechaFin && c.FechaFin > p.FechaFin || 
                                    c.FechaInicio < p.FechaInicio && c.FechaInicio < p.FechaFin 
                                    && c.FechaFin > p.FechaInicio && c.FechaFin > p.FechaFin)
                                {
                                    count++;
                                }
                            }

                            if (count == 0) { disponibles.Add(i); }
                        } 
                        else
                        {
                            disponibles.Add(i);
                        }
                    }

                    //TempData["Lista"] = disponibles;
                    //return RedirectToAction("Disponibles", disponibles);
                    Disponibles((List<Inmueble>)disponibles);
                }
                else
                {
                    ViewBag.TipoInmueble = repositorioInmueble.ObtenerTodosTipos();
                    TempData["Mensaje"] = "No hay Inmuebles disponibles";
                    return RedirectToAction(nameof(Busqueda));
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
            //var lista = TempData["Lista"];
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
                    TempData["Id"] = "Inmueble agregado exitosamente!";
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
            var lista = repositorioContrato.ObtenerTodosPorId(id);

            if (lista.Count != 0)
            {
                ViewBag.Inmueble = lista[0].Inmueble;
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