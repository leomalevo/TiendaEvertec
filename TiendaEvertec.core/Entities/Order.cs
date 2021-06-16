using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaEvertec.core.Entities
{
   public class Order
    {
        public int Id { get; set; }
        public int? IdRequest { get; set; }
        public string customer_name { get; set; }

        public string customer_surname { get; set; }

        public string customer_document { get; set; }

        public string customer_address { get; set; }

        public string customer_documenttype { get; set; }

        public string customer_company { get; set; }

        public string customer_email { get; set; }

        public string customer_mobile { get; set; }

        public string status { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public decimal? OrderCost { get; set; }


        public Order() { }

        public Order(int Id, int? IdRequest, string Customer_Name, string  Customer_Surname, string Customer_Address, string Customer_Document, string Customer_DocumentType, string Customer_Company, string Customer_Email, string Customer_Mobile, string Status,DateTime Created_at,DateTime? Updated_at, decimal? OrderCost)
        {
            this.Id = Id;
            this.IdRequest = IdRequest;
            this.customer_name = Customer_Name;
            this.customer_surname = Customer_Surname;
            this.customer_document = Customer_Document;
            this.customer_documenttype = Customer_DocumentType;
            this.customer_address = Customer_Address;
            this.customer_company = Customer_Company;
            this.customer_email = Customer_Email;
            this.customer_mobile = Customer_Mobile;
            this.status = Status;
            this.created_at = Created_at;
            this.updated_at = Updated_at;
            this.OrderCost = OrderCost;
        }
    }
}
