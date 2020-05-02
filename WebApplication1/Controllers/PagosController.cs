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
    public class PagosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioContrato repositorioContrato;
        private readonly RepositorioPago repositorioPago;

        public PagosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioContrato = new RepositorioContrato(configuration);
            repositorioPago = new RepositorioPago(configuration);
        }

        // GET: Pagos
        public ActionResult Index(int id)
        {
            var lista = repositorioPago.ObtenerTodosPorContratoId(id);
            
            if (lista.Count() == 0)
            {
                TempData["Mensaje"] = "No se registran pagos realizados para este contrato";
                return RedirectToAction("Index", "Contratos");
            }
            else
            {
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];

                ViewBag.Contrato = lista[0].Contrato;
                return View(lista);
            }
        }

        // GET: Pagos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pagos/Create
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Create(int id)
        {
            IList<Pago> p = repositorioPago.ObtenerTodosPorContratoId(id);
            Contrato c = repositorioContrato.ObtenerPorId(id);

            DateTime d2 = c.FechaFin;
            DateTime d1 = c.FechaInicio;
            TimeSpan diff = d2 - d1;
            double totalDias = diff.TotalDays;
            double cantidadPagos = Math.Round(totalDias / 30);

            if (p == null)
            {
                ViewBag.NroPago = 1;
                ViewBag.Contrato = c;
                return View();
            }
            else
            {
                int nroPagoMax = 0;

                foreach (Pago pago in p)
                {
                    if (pago.NroPago > nroPagoMax)
                    {
                        nroPagoMax = pago.NroPago;
                    }
                }

                nroPagoMax++;

                if (nroPagoMax > cantidadPagos)
                {
                    TempData["Mensaje"] = "Se realizaron todos los pagos del contrato vigente";
                    return RedirectToAction("Index", "Contratos");
                }
                else
                {
                    ViewBag.NroPago = nroPagoMax;
                    ViewBag.Contrato = c;
                    return View();
                }
            }
        }

        // POST: Pagos/Create
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago pago)
        {
            try
            {
                repositorioPago.Alta(pago);
                TempData["Id"] = "Pago realizado correctamente";
                return RedirectToAction("Index", "Contratos");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();

            }
        }

        // GET: Pagos/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorioPago.ObtenerPorId(id);
            return View(p);
        }

        // POST: Pagos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioPago.Modificacion(p);
                    TempData["Mensaje"] = "Datos modificados con exito!";
                    return RedirectToAction("Index", new { id = p.ContratoId });
                }
                else
                {
                    TempData["Error"] = "No se pudieron almacenar corectamente los datos";
                    return View(p);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: Pagos/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var p = repositorioPago.ObtenerPorId(id);
            return View(p);
        }

        // POST: Pagos/Delete/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Pago p)
        {
            var c = repositorioPago.ObtenerPorId(id);
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioPago.Baja(id);
                    TempData["Mensaje"] = "Pago eliminado correctamente!";
                    return RedirectToAction("Index", new { id = c.ContratoId });
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el pago correctamente";
                    return View(c);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(c);
            }
        }

    }
}