using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RecuperarClaveView
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        
    }
}
