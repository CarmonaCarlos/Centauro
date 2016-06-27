using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Centauro.DataModel
{
    class Falla_Refri
    {
        public string id { get; set; }
        public string idFalla { get; set; }
        public string Atendido { get; set; }
        public string Observacion { get; set; }
        public string idTecnico { get; set; }
        public string idRefrigerador { get; set; }
        public string Temperatura_Refri{ get; set; }
        public string Fecha { get; set; }
    }
}
