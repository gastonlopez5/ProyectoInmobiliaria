using Microsoft.AspNetCore.Mvc.ModelBinding;
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
		[Key, Display(Name = "Codigo")]
		public int Id { get; set; }
		
		[Required(ErrorMessage = "Fecha de inicio requerida")]
		[DataType(DataType.Date)]
		public DateTime FechaInicio { get; set; }

		[Required(ErrorMessage = "Fecha de finalización requerida")]
		[DataType(DataType.Date)]
		public DateTime FechaFin { get; set; }

		[Required(ErrorMessage = "Importe requerido")]
		[Range(1000, 1000000, ErrorMessage = "Ingrese un valor entre 1000 y 1000000")]
		[RegularExpression(@"^[0-9]{1,1000000}$", ErrorMessage = "Ingrese un número entero")]
		public decimal Importe { get; set; }
		
		[Required(ErrorMessage = "DNI requerido")]
		[StringLength(8, ErrorMessage = "Ingrese un DNI válido")]
		[RegularExpression(@"^[0-9]{1,8}$", ErrorMessage = "Ingrese un número entero")]
		public string DniGarante { get; set; }
		
		[Required(ErrorMessage = "Campo requerido")]
		[MinLength(4, ErrorMessage = "Escriba al menos 5 caracteres")]
		[MaxLength(50, ErrorMessage = "Escriba un máximo de 50 caracteres")]
		[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Valores numéricos no permitidos")]
		public string NombreCompletoGarante { get; set; }
		
		[Required(ErrorMessage = "Campo requerido")]
		[StringLength(10, ErrorMessage = "Ingrese un número válido")]
		[RegularExpression(@"^[0-9]{1,10}$", ErrorMessage = "Ingrese un número")]
		public string TelefonoGarante { get; set; }

		[Required(ErrorMessage = "Email requerido")]
		[DataType(DataType.EmailAddress, ErrorMessage = "Ingrese un email válido")]
		public string EmailGarante { get; set; }
		
		[Required]
		[ForeignKey("InquilinoId")]
		public int InquilinoId { get; set; }
		
		[Required]
		[ForeignKey("InmuebleId")]
		public int InmuebleId { get; set; }
		
		
		public Inquilino Inquilino { get; set; }
		
		[BindNever]
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
