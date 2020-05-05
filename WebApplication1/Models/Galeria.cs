using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Galeria
    {
        [Key]
        public int Id { get; set; }

        public string Ruta { get; set; }

        [ForeignKey("InmuebleId")]
        public int InmuebleId { get; set; }

        public Inmueble Propiedad { get; set; }

        public IList<IFormFile> Archivos { get; set; }
    }
}
