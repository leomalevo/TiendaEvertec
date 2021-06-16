using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TiendaEvertec.Models
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