using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaEvertec.core.Entities
{
   

    public static class OrderStatus
    {

        public static string PAYED { get { return "PAYED"; }  }

        public static string REJECTED { get { return "REJECTED"; } }
        public static string CREATED { get { return "CREATED"; } }

        public static string PENDING { get { return "PENDING"; } }
    }



}
