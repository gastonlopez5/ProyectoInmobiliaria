using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Models;
using WebApplication2.Models;

namespace WebApplication1.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public PropietariosController(DataContext contexto, IConfiguration config, IHostingEnvironment environment)
        {
            this.contexto = contexto;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Propietarios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                PropietarioFoto propietarioFoto = new PropietarioFoto();

                var propietario = contexto.Propietarios.SingleOrDefault(x => x.Email == usuario);
                var ruta = contexto.FotoPerfil.SingleOrDefault(e => e.PropietarioId == propietario.Id).Ruta;
                var pass = contexto.Usuarios.SingleOrDefault(a => a.Email == usuario).Clave;

                propietarioFoto.Id = propietario.Id;
                propietarioFoto.Dni = propietario.Dni;
                propietarioFoto.Nombre = propietario.Nombre;
                propietarioFoto.Apellido = propietario.Apellido;
                propietarioFoto.Email = propietario.Email;
                propietarioFoto.Telefono = propietario.Telefono;
                propietarioFoto.Ruta = ruta;
                propietarioFoto.Clave = pass;

                return Ok(propietarioFoto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Propietarios/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Propietarios.SingleOrDefault(x => x.Email == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = contexto.Usuarios.FirstOrDefault(x => x.Email == loginView.Usuario);
                if (p == null || p.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Propietarios
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Propietarios/5
        [HttpPut()]
        public async Task<IActionResult> Put(PropietarioClave entidad)
        {
            try
            {
                Propietario propietario = null;
                Usuario usuario = null;

                if (ModelState.IsValid && contexto.Usuarios.AsNoTracking().SingleOrDefault(e => e.Email == User.Identity.Name) != null)
                {
                    entidad.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                password: entidad.Clave,
                                                salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                                                prf: KeyDerivationPrf.HMACSHA1,
                                                iterationCount: 1000,
                                                numBytesRequested: 256 / 8));
                    
                    usuario = contexto.Usuarios.SingleOrDefault(x => x.Email == User.Identity.Name);
                    usuario.Email = entidad.Email;
                    usuario.Clave = entidad.Clave;
                    contexto.Usuarios.Update(usuario);

                    propietario = contexto.Propietarios.SingleOrDefault(x => x.Email == User.Identity.Name);
                    propietario.Nombre = entidad.Nombre;
                    propietario.Apellido = entidad.Apellido;
                    propietario.Dni = entidad.Dni;
                    propietario.Email = entidad.Email;
                    propietario.Telefono = entidad.Telefono;
                    contexto.Propietarios.Update(propietario);

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

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
