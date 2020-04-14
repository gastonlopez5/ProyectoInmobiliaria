using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(8)]
        public string Dni { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [DataType(DataType.EmailAddress), Required]
        public string Email { get; set; }
        [Required]
        public string Telefono { get; set; }
    }
}
