using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Inmueble
    {
        [Key, Display(Name = "Código")]
        public int Id { get; set; }
        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(50, ErrorMessage = "La dirección debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        public string Direccion { get; set; }
        [Required]
        public int Tipo { get; set; }
        [Required]
        public String Uso { get; set; }
        [Required]
        public int Ambientes { get; set; }
        [Required]
        public Decimal Costo { get; set; }
        public Boolean Disponible { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey("PropietarioId")]
        public Propietario Duenio { get; set; }
        [ForeignKey("Tipo")]
        public TipoInmueble TipoInmueble { get; set; }
    }
}
