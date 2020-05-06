using Microsoft.AspNetCore.Http;
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

        [Required(ErrorMessage = "Debe elegir entre Residencial o Privado")]
        [RegularExpression(@"^[a-zA-Z]{1,100}$", ErrorMessage = "Valores numéricos no permitidos")]
        public String Uso { get; set; }

        [Required(ErrorMessage = "Ingrese un valor distinto de cero")]
        [Range(1, 1000000, ErrorMessage = "Ingrese un valor distinto de cero")]
        public int Ambientes { get; set; }

        [Required(ErrorMessage = "Importe requerido")]
        [Range(1000, 1000000, ErrorMessage = "Ingrese un valor entre 1000 y 1000000")]
        [RegularExpression(@"^[0-9]{1,1000000}$", ErrorMessage = "Ingrese un número entero")]
        public Decimal Costo { get; set; }
        
        public Boolean Disponible { get; set; }
        
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        
        [ForeignKey("PropietarioId")]
        public Propietario Duenio { get; set; }
        
        [ForeignKey("Tipo")]
        public TipoInmueble TipoInmueble { get; set; }

        public String Foto { get; set; }

        [Required(ErrorMessage = "Fotos del Inmueble requeridas")]
        public IList<IFormFile> Archivos { get; set; }
    }
}
