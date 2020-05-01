using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class BusquedaInmuebleView
    {
		[Required(ErrorMessage = "Fecha requerida")]
		[DataType(DataType.Date)]
		public DateTime FechaInicio { get; set; }
		
		[Required(ErrorMessage = "Fecha requerida")]
		[DataType(DataType.Date)]
		public DateTime FechaFin { get; set; }

		[Required(ErrorMessage = "Importe requerido")]
		[Range(1000, 1000000, ErrorMessage = "Ingrese un valor entre 1000 y 1000000")]
		[RegularExpression(@"^[0-9]{1,1000000}$", ErrorMessage = "Ingrese un número")]
		public decimal Importe { get; set; }
		
		public int Tipo { get; set; }
		
		[Required(ErrorMessage = "Debe elegir entre Residencial o Privado")]
		[RegularExpression(@"^[a-zA-Z]{1,100}$", ErrorMessage = "Valores numéricos no permitidos")]
		public String Uso { get; set; }
		
		[Required(ErrorMessage = "Ingrese un valor distinto de cero")]
		[Range(1, 1000000, ErrorMessage = "Ingrese un valor distinto de cero")]
		public int Ambientes { get; set; }
	}
}
