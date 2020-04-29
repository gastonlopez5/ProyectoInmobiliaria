using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Contrato : IValidatableObject
    {
		DateTime fechaActual = DateTime.Now;

		[Key, Display(Name = "Codigo")]
		public int Id { get; set; }
		[Required, DataType(DataType.Date)]
		public DateTime FechaInicio { get; set; }
		[Required, DataType(DataType.Date)]
		public DateTime FechaFin { get; set; }
		[Required]
		public decimal Importe { get; set; }
		[Required, StringLength(8)]
		public string DniGarante { get; set; }
		[Required]
		public string NombreCompletoGarante { get; set; }
		[Required, StringLength(10)]
		public string TelefonoGarante { get; set; }
		[DataType(DataType.EmailAddress), Required]
		public string EmailGarante { get; set; }
		[Required]
		public int InquilinoId { get; set; }
		[Required]
		public int InmuebleId { get; set; }
		[ForeignKey("InquilinoId")]
		public Inquilino Inquilino { get; set; }
		[ForeignKey("InmuebleId")]
		public Inmueble Inmueble { get; set; }
		public Propietario Propietario { get; set; }

		IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
		{
			if (FechaFin <= FechaInicio)
			{
				yield return new ValidationResult("Fecha final no puede anterior a la fecha de inicio del contrato");
			} 
		}
	}
}
