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
    [Authorize(Policy = "EsDeLaCasa")]
    public class ContratosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioInmueble repositorioInmueble;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioPago repositorioPago;

        public ContratosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioInquilino = new RepositorioInquilino(configuration);
            repositorioInmueble = new RepositorioInmueble(configuration);
            repositorioPago = new RepositorioPago(configuration);
        }

        // GET: Contratos
        public ActionResult Index()
        {
            var lista = repositorioContrato.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: Contratos/Details/5
        public ActionResult Details(int id)
        {
            var p = repositorioContrato.ObtenerPorId(id);
            ViewBag.inmueble = p.Inmueble;
            ViewBag.inquilino = p.Inquilino;
            return View(p);
        }


        // GET: Contratos/Create
        public ActionResult Create(int id)
        {
            Inmueble i = repositorioInmueble.ObtenerPorId(id);
            //ViewBag.inmueble = repositorioInmueble.ObtenerTodos();
            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
            ViewBag.inmuebleId = i.Id;
            ViewBag.inmuebleImporte = i.Costo;
            //int resultado = 0;
            int resultado2 = 0;

            /*
            foreach (var item in (IList<Inmueble>)ViewBag.inmueble)
            {
                if (item.Disponible)
                {
                    resultado++;
                }
            }
            */

            foreach (var item in (IList<Inquilino>)ViewBag.inquilino)
            {
                if (!item.Id.Equals(" "))
                {
                    resultado2++;
                }
            }
            
            /*
            if (resultado > 0 && resultado2 > 0)
            {
                return View();
            }
            */

            if (resultado2 > 0)
            {
                return View();
            }
            else
            {
                TempData["Error"] = "No hay inquilinos disponibles";
                return RedirectToAction("Index", "Inmuebles");
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
                    TempData["Id"] = "Contrato de alquiler agregado correctamente";
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
            var p = repositorioContrato.ObtenerPorId(id);
            ViewBag.inmueble = repositorioInmueble.ObtenerTodos();
            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Contratos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Contrato p = null;
            try
            {
                p = repositorioContrato.ObtenerPorId(id);
                p.InquilinoId = Int32.Parse(collection["InquilinoId"]);
                p.InmuebleId = Int32.Parse(collection["InmuebleId"]);
                p.FechaInicio = DateTime.Parse(collection["FechaInicio"]);
                p.FechaFin = DateTime.Parse(collection["FechaFin"]);
                p.Importe = Decimal.Parse(collection["Importe"]);
                p.DniGarante = collection["DniGarante"];
                p.NombreCompletoGarante = collection["NombreCompletoGarante"];
                p.TelefonoGarante = collection["TelefonoGarante"];
                p.EmailGarante = collection["EmailGarante"];
                repositorioContrato.Modificacion(p);
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

        // GET: Contratos/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            IList<Pago> p = repositorioPago.ObtenerTodosPorContratoId(id);
            Contrato c = repositorioContrato.ObtenerPorId(id);

            DateTime d2 = c.FechaFin;
            DateTime d1 = c.FechaInicio;
            TimeSpan diff = d2 - d1;
            double totalDias = diff.TotalDays;
            double cantidadPagos = Math.Round(totalDias / 30);
            //double cantidadPagos = 2;

            int nroPagoMax = 0;
            foreach (Pago pago in p)
            {
                if (pago.NroPago > nroPagoMax)
                {
                    nroPagoMax = pago.NroPago;
                }
            }

            if (nroPagoMax < cantidadPagos)
            {
                if (nroPagoMax < cantidadPagos / 2)
                {
                    TempData["Error"] = "Debe abonar " + c.Importe*2 + " (2 meses de alquiler) por finalizar el contrato antes del " + c.FechaFin.ToString("dd-MM-yyyy") + " y haber abonado menos de la mitad del total de pagos";
                }
                else
                {
                    TempData["Error"] = "Debe abonar " + c.Importe + " (1 mes de alquiler) por finalizar el contrato antes del " + c.FechaFin.ToString("dd-MM-yyyy") + " y haber abonado más de la mitad del total de pagos"; ;
                }
            }
            
            var entidad = repositorioContrato.ObtenerPorId(id);
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            
            return View(entidad);
        }

        // POST: Contratos/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato entidad)
        {
            Contrato c = null;

            try
            {
                c = repositorioContrato.ObtenerPorId(id);

                repositorioInmueble.CambioDisponible(c.InmuebleId, "1");
                repositorioPago.EliminarPagosPorContrato(id);
                repositorioContrato.Baja(id);
                TempData["Mensaje"] = "Contrato y pagos relacionados eliminados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hay pagos relacionados a este alquiler";
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        public ActionResult Pago(int id)
        {
            return RedirectToAction("Create", "Pagos", new { id = id });
        }

        public ActionResult ListaPagos(int id)
        {
            return RedirectToAction("Index", "Pagos", new { id = id });
        }
    }
}