using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class Sesion
    {
        public string Email { get; set; }
        public Guid IdSession { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
