using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaEvertec.core.Entities
{
   public  class AdditionalOrderInfo
    {

        public string UserAgent { get; set; }
        public string IP { get; set; }

        public AdditionalOrderInfo(string UserAgent, string IP)
        {
            this.UserAgent = UserAgent;
                this.IP = IP;

        }

    }
}
