using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacapp
{
    public class Cow
    {
        public string nombre { get; set; }
        public int nv { get; set; }
        public string ultimo_parto { get; set; }
        public int hato { get; set; }
        public string loc { get; set; }
        public int partos { get; set; }
        public int dias_lac { get; set; }
        public int lts_dia { get; set; }
        public string primer_servicio { get; set; }
    }

    public class RootObject
    {
        public string published_at { get; set; }
        public List<Cow> cows { get; set; }
    }
}
