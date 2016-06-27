using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Centauro.DataModel
{
    class Usuario
    {
        public string id { get; set; }
        public string Nombre { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public string telefono { get; set; }
        public string fechaNacimiento { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string contrasena { get; set; }
        public string Tipo { get; set; }
    }
}
