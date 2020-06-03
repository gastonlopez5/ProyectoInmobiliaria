using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Clave { get; set; }
        [Required]
        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public TipoUsuario TipoUsuario { get; set; }
    }
}
