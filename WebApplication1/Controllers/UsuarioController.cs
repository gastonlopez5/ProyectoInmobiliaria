using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;
using System.Net.Mail;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioUsuario repositorioUsuario;

        public UsuarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioUsuario = new RepositorioUsuario(configuration);
        }

        // GET: Usuario
        public ActionResult Index()
        {
            var lista = repositorioUsuario.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        public ActionResult Perfil()
        {
            Usuario u = null;
            string old = User.Identity.Name;

            if(User.Identity.Name == old)
            {
               u = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
            } 
            else
            {
                u = repositorioUsuario.ObtenerPorEmail(old);
            }
            
            return View(u);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            ViewBag.TipoUsuario = repositorioUsuario.ObtenerTiposUsuario();
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            ViewBag.Usuarios = repositorioUsuario.ObtenerTodos();

            foreach (var item in (IList<Usuario>)ViewBag.Usuarios)
            {
                if (item.Email == usuario.Email )
                {
                    ViewBag.Error = "Error: Ya existe un usuario con ese email";
                    return View();
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorioUsuario.Alta(usuario);
                    TempData["Id"] = "Usuario agregado exitosamente!";
                    return RedirectToAction(nameof(Index));
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

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorioUsuario.ObtenerPorId(id);
            ViewBag.TipoUsuario = repositorioUsuario.ObtenerTiposUsuario();
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Usuario p = null;
            try
            {
                p = repositorioUsuario.ObtenerPorId(id);
                p.Email = collection["Email"];

                repositorioUsuario.Modificacion(p);
                TempData["Mensaje"] = "Email actualizado correctamente. Ingresar nuevamente por favor.";
                return RedirectToAction("Logout");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(p);
            }
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPass(int id, CambioClaveView cambio)
        {
            Usuario usuario = null;
            try
            {
                usuario = repositorioUsuario.ObtenerPorId(id);
                
                if (ModelState.IsValid)
                {
                    usuario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    
                    repositorioUsuario.Modificacion(usuario);
                    TempData["Mensaje"] = "Contraseña actualizada correctamente. Ingresar nuevamente por favor";
                    return RedirectToAction("Logout");
                }
                else
                {
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Error"] += error.ErrorMessage + "\n";
                        }
                    }
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction("Edit", new { id = id });
            }
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorioUsuario.ObtenerPorId(id);
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(p);
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                repositorioUsuario.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(usuario);
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (TempData.ContainsKey("Error"))
                ViewBag.Id = TempData["Error"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = repositorioUsuario.ObtenerPorEmail(loginView.Usuario);
                if (p == null || p.Clave != hashed)
                {
                    ViewBag.Mensaje = "Email o Contraseña incorrectos";
                    return View();
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, p.Email),
                    new Claim(ClaimTypes.Role, p.TipoUsuario.Rol),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.
                    AllowRefresh = true,
                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction(nameof(Index), "Home");
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Home/Login
        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index), "Home");
        }

        [AllowAnonymous]
        public ActionResult RecuperarPass()
        {
            if(TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarPass(RecuperarClaveView recuperar)
        {
            try
            {
                var usuarios = repositorioUsuario.ObtenerTodos();
                var resultado = 0;
                String body = "";

                foreach (Usuario item in usuarios)
                {
                    if(item.Email.ToString() == recuperar.Email.ToString())
                    {
                        item.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: "1234",
                            salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
                        
                        repositorioUsuario.Modificacion(item);
                        body = "1234";
                        resultado++;
                    }
                }
                if (resultado != 0)
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("gastonlopez5@gmail.com");
                    mail.To.Add(recuperar.Email.ToString());
                    mail.Subject = "Inmobiliaria López - Recuperación de Clave Personal";
                    mail.Body = "Cuando ingrese nuevamente al sistema modifique su contraseña. \n\nContraseña: " + body;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 25;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = new System.Net.NetworkCredential("gastonlopez5@gmail.com", "50110392");
                    smtp.Send(mail);

                    TempData["Mensaje"] = "Hemos enviado un mail a su correo";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Error"] = "El Usuario ingresado no existe";
                    //se rederige porque no hay vista de cambio de pass, está compartida con Edit
                    return RedirectToAction("RecuperarPass");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction("Login");
            }
        }


    }
}