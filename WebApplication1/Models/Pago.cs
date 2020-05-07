using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Pago
    {
		[Key]
		public int Id { get; set; }
		
		public int NroPago { get; set; }
		
		public int ContratoId { get; set; }

		[DataType(DataType.Date)]
		public DateTime Fecha { get; set; }

		[Required(ErrorMessage = "Importe requerido")]
		[Range(1000, 1000000, ErrorMessage = "Ingrese un valor entre 1000 y 1000000")]
		[RegularExpression(@"^[0-9]{1,1000000}$", ErrorMessage = "Ingrese un número entero")]
		public decimal Importe { get; set; }
		
		[ForeignKey("ContratoId")]
		public Contrato Contrato { get; set; }
	}
}
