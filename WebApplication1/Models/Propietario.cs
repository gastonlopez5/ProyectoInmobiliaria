using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Propietario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [DataType(DataType.EmailAddress), Required]
        public string Email { get; set; }
        [DataType(DataType.Password), Required]
        public string Clave { get; set; }
    }
}
