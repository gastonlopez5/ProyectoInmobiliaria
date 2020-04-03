using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Inquilino
    {
		[Key]
		[Display(Name = "Código")]
		public int Id { get; set; }
		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }
		[Required, StringLength(8)]
		public string Dni { get; set; }
		[Required, StringLength(10)]
		public string Telefono { get; set; }
		[DataType(DataType.EmailAddress), Required]
		public string Email { get; set; }
		[Required]
		public string DireccionTrabajo { get; set; }
		[Required, StringLength(8)]
		public string DniGarante { get; set; }
		[Required]
		public string NombreCompletoGarante { get; set; }
		[Required, StringLength(10)]
		public string TelefonoGarante { get; set; }
		[DataType(DataType.EmailAddress), Required]
		public string EmailGarante { get; set; }

	}
}
