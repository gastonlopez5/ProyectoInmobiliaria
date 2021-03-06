﻿using System;
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

        [Required(ErrorMessage = "DNI requerido")]
        [StringLength(8, ErrorMessage = "Ingrese un DNI válido")]
        [RegularExpression(@"^[0-9]{1,8}$", ErrorMessage = "Ingrese un número")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MinLength(3, ErrorMessage = "Escriba al menos 4 caracteres")]
        [MaxLength(50, ErrorMessage = "Escriba un máximo de 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Valores numéricos no permitidos")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [MinLength(3, ErrorMessage = "Escriba al menos 4 caracteres")]
        [MaxLength(50, ErrorMessage = "Escriba un máximo de 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Valores numéricos no permitidos")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Email requerido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Ingrese un email válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contraseña requerida")]
        [StringLength(50, ErrorMessage = "La clave debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "Ingrese un número válido")]
        [RegularExpression(@"^[0-9]{1,10}$", ErrorMessage = "Ingrese un número")]
        public string Telefono { get; set; }
    }
}
