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
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Index()
        {
            var lista = repositorioContrato.ObtenerTodosVigentes();
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
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Create(int id)
        {
            Inmueble i = repositorioInmueble.ObtenerPorId(id);
            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
            ViewBag.inmueble = i;
            ViewBag.inmuebleImporte = i.Costo;
            int resultado2 = 0;

            foreach (var item in (IList<Inquilino>)ViewBag.inquilino)
            {
                if (!item.Id.Equals(" "))
                {
                    resultado2++;
                }
            }
            
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
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var listaContratos = repositorioContrato.ObtenerTodosPorInmueble(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin);
                    
                    if (listaContratos.Count != 0)
                    {
                        ViewBag.Mensaje = "Inmueble NO disponible entre las fechas indicadas";
                        Inmueble i = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
                        ViewBag.inmueble = i;
                        ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                        ViewBag.inmuebleImporte = i.Costo;
                        return View();
                    }
                    else
                    {
                        repositorioContrato.Alta(contrato);
                        TempData["Id"] = "Contrato de alquiler agregado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    Inmueble i = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
                    ViewBag.inmueble = i;
                    ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                    ViewBag.inmuebleImporte = i.Costo;
                    return View();
                }
            }
            catch (Exception ex)
            {
                Inmueble i = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
                ViewBag.inmueble = i;
                ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                ViewBag.inmuebleImporte = i.Costo;
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();

            }
        }

        // GET: Contratos/Edit/5
        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Edit(int id)
        {
            var p = repositorioContrato.ObtenerPorId(id);
            ViewBag.inmueble = repositorioInmueble.ObtenerTodosDisponibles();
            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Contratos/Edit/5
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato p)
        {
            //Mostrar. Se consideró que si se modifica el inmueble disponible se verifique si existen contratos.

            var c = repositorioContrato.ObtenerPorId(id);
            try
            {
                if (ModelState.IsValid)
                {
                    //listaContratos.Contains(c);

                    if (c.InmuebleId == p.InmuebleId)
                    {
                        repositorioContrato.Modificacion(p);
                        TempData["Mensaje"] = "Datos modificados con exito!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var listaContratos = repositorioContrato.ObtenerTodosPorInmueble(p.InmuebleId, p.FechaInicio, p.FechaFin);

                        if (listaContratos.Count != 0)
                        {
                            TempData["Mensaje"] = "Inmueble NO disponible para el período de fechas elegidas";
                            ViewBag.inmueble = repositorioInmueble.ObtenerTodosDisponibles();
                            ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
                            return View(c);
                        }
                        else
                        {
                            repositorioContrato.Modificacion(p);
                            TempData["Mensaje"] = "Datos modificados con exito!";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                } 
                else
                {
                    ViewBag.inmueble = repositorioInmueble.ObtenerTodosDisponibles();
                    ViewBag.inquilino = repositorioInquilino.ObtenerTodos();
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

        // GET: Contratos/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            Contrato c = repositorioContrato.ObtenerPorId(id);
            return View(c);
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
                //repositorioPago.EliminarPagosPorContrato(id);
                repositorioContrato.Baja(id);
                TempData["Mensaje"] = "Contrato eliminado correctamente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hay pagos relacionados a este alquiler";
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult Pago(int id)
        {
            return RedirectToAction("Create", "Pagos", new { id = id });
        }

        public ActionResult ListaPagos(int id)
        {
            return RedirectToAction("Index", "Pagos", new { id = id });
        }

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult RenovarContrato(int id)
        {
            IList<Pago> p = repositorioPago.ObtenerTodosPorContratoId(id);
            Contrato c = repositorioContrato.ObtenerPorId(id);

            DateTime d2 = c.FechaFin;
            DateTime d1 = c.FechaInicio;
            TimeSpan diff = d2 - d1;
            double totalDias = diff.TotalDays;
            double cantidadPagos = Math.Round(totalDias / 30);
            //double cantidadPagos = 3;

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
                double a = cantidadPagos - nroPagoMax;
                TempData["Mensaje"] = "Debe finalizar el contrato antes de poder renovar el mismo. Faltan "+ a +" pagos por realizar";
                return RedirectToAction("Index");
            }

            ViewBag.Contrato = repositorioContrato.ObtenerPorId(id);

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult RenovarContrato(Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //repositorioPago.EliminarPagosPorContrato(contrato.Id);
                    //repositorioContrato.Baja(contrato.Id);

                    repositorioContrato.Alta(contrato);
                    TempData["Id"] = "Contrato de alquiler renovado correctamente!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "No se pudo renovar correctamente el contrato";
                    return RedirectToAction(nameof(Index));
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

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult ContratosVencidos()
        {
            var lista = repositorioContrato.ObtenerTodosVencidos();
            if (lista.Count != 0)
            {
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "No se registran contratos vencidos en el sistema";
                return RedirectToAction("Index");
            }
            
        }

        [Authorize(Policy = "EsDeLaCasa")]
        public ActionResult TerminarContrato(int id)
        {
            IList<Pago> p = repositorioPago.ObtenerTodosPorContratoId(id);
            Contrato c = repositorioContrato.ObtenerPorId(id);

            DateTime d2 = c.FechaFin;
            DateTime d1 = c.FechaInicio;
            TimeSpan diff = d2 - d1;
            double totalDias = diff.TotalDays;
            double cantidadPagos = Math.Round(totalDias / 30);
            //double cantidadPagos = 2;

            double nroPagoMax = 0;
            if (p.Count != 0)
            {
                foreach (Pago pago in p)
                {
                    if (pago.NroPago > nroPagoMax)
                    {
                        nroPagoMax = pago.NroPago;
                    }
                }
            }
            else
            {
                nroPagoMax = cantidadPagos;
            }
            
            int cantidadPagosRealizados = p.Count;

            if (cantidadPagosRealizados < cantidadPagos)
            {
                double nroPagosDeuda = nroPagoMax - cantidadPagosRealizados;

                if (cantidadPagosRealizados < cantidadPagos / 2)
                {
                    TempData["Error"] = "Debe abonar " + c.Importe * 2 + " (2 meses de alquiler) de multa por haber abonado menos de la mitad del Nº total de pagos. Registra " + nroPagosDeuda + " pagos adeudados.";
                }
                else
                {
                    TempData["Error"] = "Debe abonar " + c.Importe + " (1 mes de alquiler) de multa. Registra " + nroPagosDeuda + " pagos adeudados.";
                }
            }

            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(c);
        }

        // POST: Contratos/Delete/5
        [HttpPost]
        [Authorize(Policy = "EsDeLaCasa")]
        [ValidateAntiForgeryToken]
        public ActionResult TerminarContrato(int id, Contrato entidad)
        {
            Contrato c = null;

            try
            {
                c = repositorioContrato.ObtenerPorId(id);
                c.FechaFin = DateTime.Today;
                //c.FechaFin = c.FechaInicio.AddDays(90);

                repositorioInmueble.CambioDisponible(c.InmuebleId, "1");
                repositorioContrato.Modificacion(c);
                //repositorioPago.EliminarPagosPorContrato(id);
                //repositorioContrato.Baja(id);
                TempData["Mensaje"] = "Contrato terminado correctamente!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hay pagos relacionados a este alquiler";
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        public ActionResult ListarTodosContratos(int id)
        {
            var lista = repositorioContrato.ObtenerVigentesVencidosPorInmuebleId(id);
            
            if (lista.Count != 0)
            {
                ViewBag.Contrato = lista[0];
                return View(lista);
            }
            else
            {
                TempData["Mensaje"] = "El Inmueble no tiene contratos registrados en el sistema";
                return RedirectToAction("Index", "Inmuebles");
            }
        }

    }
}