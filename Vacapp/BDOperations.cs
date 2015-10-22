using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacapp
{
    class BDOperations
    {
        /*
         * Add a cow to local database 
         */
        public static void AddCow(Cow cowJSON)
        {
            using (CowDataContext context = new CowDataContext(CowDataContext.DBConnectionString))
            {
                try
                {
                    cowDB cow = new cowDB();
                    cow.nv = cowJSON.nv;
                    cow.nombre = cowJSON.nombre;
                    cow.dias_lac = cowJSON.dias_lac;
                    cow.hato = cowJSON.hato;
                    cow.loc = cowJSON.loc;
                    cow.lts_dia = cowJSON.lts_dia;
                    cow.partos = cowJSON.partos;
                    cow.ultimo_parto = cowJSON.ultimo_parto;
                    cow.primer_servicio = cowJSON.primer_servicio;
                    System.Diagnostics.Debug.WriteLine("agregando");
                    context.cows.InsertOnSubmit(cow);                   
                    context.SubmitChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);                    
                }
            }
        }

        /*
        * get cows from  local database 
        */
        public static IList<cowDB> GetAllCows()
        {
            IList<cowDB> list = null;
            using (CowDataContext context = new CowDataContext(CowDataContext.DBConnectionString))
            {
                IQueryable<cowDB> query = from c in context.cows select c;
                list = query.ToList();
            }
            return list;
        }
             
        public static List<Cow> GetCows()
        {
            IList<cowDB> list = GetAllCows();
            List<Cow> allCows = new List<Cow>();
            foreach (cowDB c in list)
            {
                Cow n = new Cow();
                n.dias_lac = c.dias_lac;
                n.hato = c.dias_lac;
                n.loc = c.loc;
                n.lts_dia = c.lts_dia;
                n.nombre = c.nombre;
                n.nv = c.nv;
                n.partos = c.partos;
                n.primer_servicio = c.primer_servicio;
                n.ultimo_parto = c.ultimo_parto;
                allCows.Add(n);
            }
            return allCows;
        }

        public static Cow getCow(int nvc)
        {
            Cow cowToReturn = null;
            using (CowDataContext context = new CowDataContext(CowDataContext.DBConnectionString))
            {
                 IQueryable<cowDB> entityQuery = from c in context.cows where c.nv==nvc select c ;
                 cowDB cowDB = entityQuery.FirstOrDefault();
                if(cowDB!=null)
                {
                    cowToReturn = new Cow();
                    cowToReturn.dias_lac = cowDB.dias_lac;
                    cowToReturn.hato = cowDB.dias_lac;
                    cowToReturn.loc = cowDB.loc;
                    cowToReturn.lts_dia = cowDB.lts_dia;
                    cowToReturn.nombre = cowDB.nombre;
                    cowToReturn.nv = cowDB.nv;
                    cowToReturn.partos = cowDB.partos;
                    cowToReturn.primer_servicio = cowDB.primer_servicio;
                    cowToReturn.ultimo_parto = cowDB.ultimo_parto;
                }
            }
            return cowToReturn;
           
        }

        public static void deleteAllCows()
        {
            using (CowDataContext context = new CowDataContext(CowDataContext.DBConnectionString))
            {
                IQueryable<cowDB> entityQuery = from c in context.cows select c;
                IList<cowDB> entityToDelete = entityQuery.ToList();
                context.cows.DeleteAllOnSubmit(entityToDelete);
                context.SubmitChanges();
            }
        }


    }
}
