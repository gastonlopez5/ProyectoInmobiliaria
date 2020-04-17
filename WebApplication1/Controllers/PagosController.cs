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
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pagos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pagos/Create
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
                int nroPagoMax = 1;
                foreach (Pago pago in p)
                {
                    if (pago.NroPago > nroPagoMax)
                    {
                        nroPagoMax = pago.NroPago;
                    }
                }

                ViewBag.NroPago = nroPagoMax;
                ViewBag.Contrato = c;
                return View();
            }
        }

        // POST: Pagos/Create
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

        // GET: Pagos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pagos/Edit/5
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

        // GET: Pagos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pagos/Delete/5
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