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
    public class InquilinosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioInquilino repositorioInquilino;

        public InquilinosController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioInquilino = new RepositorioInquilino(configuration);
        }

        // GET: Inquilinos
        public ActionResult Index()
        {
            var lista = repositorioInquilino.ObtenerTodos();
            if (TempData.ContainsKey("Alta"))
                ViewBag.Alta = TempData["Alta"];
            return View(lista);
        }

        // GET: Inquilinos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inquilinos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino p)
        {
            ViewBag.inquilinos = repositorioInquilino.ObtenerTodos();

            foreach (var item in (IList<Inquilino>)ViewBag.inquilinos)
            {
                if (item.Dni == p.Dni)
                {
                    ViewBag.Error2 = "Error: Ya existe un inquilino con ese DNI";
                    return View();
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    repositorioInquilino.Alta(p);
                    TempData["Alta"] = "Inquilino agregado exitosamente!";
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Inquilinos/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorioInquilino.ObtenerPorId(id);
            
            return View(p);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Inquilino p = null;
            try
            {
                p = repositorioInquilino.ObtenerPorId(id);
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Telefono = collection["Telefono"];
                p.Email = collection["Email"];
                p.DireccionTrabajo = collection["DireccionTrabajo"];
                repositorioInquilino.Modificacion(p);
                TempData["Alta"] = "Datos guardados correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // GET: Inquilinos/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorioInquilino.ObtenerPorId(id);
           
            return View(p);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inquilino p)
        {
            try
            {
                repositorioInquilino.Baja(id);
                TempData["Alta"] = "Inquilino eliminado";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Inquilino con contrato vigente";
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }
    }
}