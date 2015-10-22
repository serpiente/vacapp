using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Vacapp
{
    [Table]
    public class cowDB
    {
        [Column(IsPrimaryKey = true)]
        public int nv
        {
            get;
            set;
        }
        [Column]
        public string nombre
        {
            get;
            set;
        }
        [Column]
        public string ultimo_parto
        {
            get;
            set;
        }
        [Column]
        public int hato
        {
            get;
            set;
        }       
        [Column]
        public string loc
        {
            get;
            set;
        }
        [Column]
        public int partos
        {
            get;
            set;
        }
        [Column]
        public int dias_lac
        {
            get;
            set;
        }
        [Column]
        public int lts_dia
        {
            get;
            set;
        }
        [Column]
        public string primer_servicio
        {
            get;
            set;
        }

    }
}
