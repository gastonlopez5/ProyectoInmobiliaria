using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;
using WebApplication2.Models;
using Microsoft.AspNetCore.Hosting;

using System.IO;
using WebApplication3.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_.Net_Core.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InmueblesController : Controller
    {
        private readonly DataContext contexto;
        private readonly IHostingEnvironment environment;

        public InmueblesController(DataContext contexto, IHostingEnvironment environment)
        {
            this.contexto = contexto;
            this.environment = environment;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var listaInmuebles = contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario).Include(e => e.TipoInmueble);
                List<InmuebleFoto> listaInmueblesFoto = new List<InmuebleFoto>();

                foreach(Inmueble i in listaInmuebles)
                {
                    InmuebleFoto inmuebleFoto = new InmuebleFoto
                    {
                        Inmueble = i,
                        Ruta = contexto.Galeria.FirstOrDefault(e => e.InmuebleId == i.Id).Ruta
                    };
                    listaInmueblesFoto.Add(inmuebleFoto);
                }

                return Ok(listaInmueblesFoto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario).Single(e => e.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] Inmueble entidad)
            {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.PropietarioId = contexto.Propietarios.Single(e => e.Email == User.Identity.Name).Id;
                    contexto.Inmuebles.Add(entidad);
                    contexto.SaveChanges();

                    Galeria foto = new Galeria();
                    foto.Ruta = "/Galeria/" + entidad.Id + "/casa2.jpg";
                    foto.InmuebleId = entidad.Id;
                    contexto.Galeria.Add(foto);
                    contexto.SaveChanges();

                    string wwwPath = environment.WebRootPath;
                    string path = wwwPath + "/Galeria/" + entidad.Id + "";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    InmuebleFoto inmuebleFoto = new InmuebleFoto();
                    inmuebleFoto.Inmueble = entidad;
                    inmuebleFoto.Ruta = foto.Ruta;

                    return Ok(inmuebleFoto);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Inmueble entidad)
        {
            try
            {
                Inmueble inmueble = null;

                if (ModelState.IsValid && contexto.Inmuebles.AsNoTracking().SingleOrDefault(e => e.Id == entidad.Id) != null)
                {
                    inmueble = contexto.Inmuebles.SingleOrDefault(x => x.Id == entidad.Id);
                    inmueble.Uso = entidad.Uso;
                    inmueble.Direccion = entidad.Direccion;
                    inmueble.Costo = entidad.Costo;
                    inmueble.Ambientes = entidad.Ambientes;
                    inmueble.Disponible = entidad.Disponible;
                    inmueble.Tipo = entidad.TipoInmueble.Id;
                    contexto.Inmuebles.Update(inmueble);

                    contexto.SaveChanges();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Mensaje msg = null;
                var entidad = contexto.Inmuebles.Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == User.Identity.Name);
                var galeria = contexto.Galeria.Where(x => x.InmuebleId == id);
                var contratosByPropietario = contexto.Contrato
                .Include(i => i.Inquilino)
                .Include(e => e.Inmueble)
                .ThenInclude(p => p.Duenio)
                .Include(e => e.Inmueble)
                .ThenInclude(t => t.TipoInmueble)
                .Where(x => x.Inmueble.Duenio.Email == User.Identity.Name && x.Inmueble.Id == id);

                if (entidad != null && contratosByPropietario.Count() == 0)
                {
                    foreach(Galeria g in galeria)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = wwwPath + g.Ruta;
                        System.IO.File.Delete(path);
                    }

                    contexto.Galeria.RemoveRange(galeria);
                    contexto.Inmuebles.Remove(entidad);
                    contexto.SaveChanges();
                    msg = new Mensaje();
                    msg.Msg = "Inmueble eliminado exitosamente!!";
                    return Ok(msg);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
		}
	}
}
