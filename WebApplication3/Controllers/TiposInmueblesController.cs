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
using Microsoft.Extensions.Configuration;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TiposInmueblesController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public TiposInmueblesController(DataContext contexto, IConfiguration config, IHostingEnvironment environment)
        {
            this.contexto = contexto;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/TiposInmuebles
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(contexto.TipoInmueble);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        

        // POST: api/TiposInmuebles
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/TiposInmuebles/5
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
