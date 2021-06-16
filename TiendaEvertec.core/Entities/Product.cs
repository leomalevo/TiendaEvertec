using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaEvertec.core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public string ProductDescription { get; set; }

        public int Quantity { get; set; }

        public decimal ValuePerUnit { get; set; }
    }
}
