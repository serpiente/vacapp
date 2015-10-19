using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacapp
{
    public class CowDataContext : DataContext
    {
        public static string DBConnectionString = @"isostore:/Databasesc.sdf";
        public CowDataContext(string connectionString):base(connectionString)
        { }
        public Table<cowDB> cows
        {
            get
            {
                return this.GetTable<cowDB>();
            }
        }
    }
}
