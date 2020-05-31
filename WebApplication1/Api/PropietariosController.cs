using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PropietariosController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        // GET: api/Propietarios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var lista = contexto.Propietarios;
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Propietarios/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value" + id;
        }

        // POST: api/Propietarios
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Propietarios/5
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
