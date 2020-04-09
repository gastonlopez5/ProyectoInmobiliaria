using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ContratosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioInquilino repositorioInquilino;

        public ContratosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
        }

        // GET: Contratos
        public ActionResult Index()
        {
            var lista = repositorioContrato.ObtenerTodos();
            if (TempData.ContainsKey("Alta"))
                ViewBag.Alta = TempData["Alta"];
            if (TempData.ContainsKey("Error2"))
                ViewBag.Error2 = TempData["Error2"];
            return View(lista);
        }

        // GET: Contratos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contratos/Create
        public ActionResult Create()
        {
            ViewBag.inmueble = repositorioInmueble.ObtenerTodos();
            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
            int resultado = 0;
            int resultado2 = 0;

            foreach (var item in (IList<Inmueble>)ViewBag.inmueble)
            {
                if (item.Disponible)
                {
                    resultado++;
                }
            }

            foreach (var item in (IList<Inquilino>)ViewBag.inquilino)
            {
                if (!item.Id.Equals(" "))
                {
                    resultado2++;
                }
            }

            if (resultado > 0 && resultado2 > 0)
            {
                return View();
            }
            else
            {
                TempData["Error2"] = "No hay inmuebles o inquilinos disponibles";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contratos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                repositorioInmueble.CambioDisponible(contrato.InmuebleId, "false");
                if (ModelState.IsValid)
                {
                    repositorioContrato.Alta(contrato);
                    TempData["Alta"] = "Contrato de alquiler agregado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    repositorioInmueble.CambioDisponible(contrato.Id, "true");
                    ViewBag.inmueble = repositorioInmueble.ObtenerTodos();
                    ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.inmueble = repositorioInmueble.ObtenerTodos();
                ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();

            }
        }

        // GET: Contratos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contratos/Edit/5
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

        // GET: Contratos/Delete/5
        public ActionResult Delete(int id)
        {
            var entidad = repositorioContrato.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Contratos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato entidad)
        {
            try
            {
                repositorioContrato.Baja(id);
                TempData["Alta"] = "Se eliminó correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hay pagos relacionados a este alquiler";
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }
    }
}