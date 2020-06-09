using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContratosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public ContratosController(DataContext contexto, IConfiguration config, IHostingEnvironment environment)
        {
            this.contexto = contexto;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Contratos
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var contratosByPropietario = contexto.Contrato
                .Include(i => i.Inquilino)
                .Include(e => e.Inmueble)
                .ThenInclude(p => p.Duenio)
                .Include(e => e.Inmueble)
                .ThenInclude(t => t.TipoInmueble)
                .Where(x => x.Inmueble.Duenio.Email == usuario && x.Inmueble.Id == id && x.FechaFin >= DateTime.Now);

                return Ok(contratosByPropietario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Contratos
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Contratos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
