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
		[Required, Display(Name = "Nº de Pago")]
		public int NroPago { get; set; }
		[Required]
		public int ContratoId { get; set; }
		[Required, DataType(DataType.Date)]
		public string Fecha { get; set; }
		[Required]
		public decimal Importe { get; set; }
		[ForeignKey("ContratoId")]
		public Contrato Contrato { get; set; }
	}
}
