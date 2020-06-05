using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication2.Models
{
    public class FotoPerfil
    {
        [Key]
        public int Id { get; set; }

        public string Ruta { get; set; }

        [ForeignKey("PropietarioId")]
        public int PropietarioId { get; set; }

        [BindNever]
        public Propietario Duenio { get; set; }
    }
}
