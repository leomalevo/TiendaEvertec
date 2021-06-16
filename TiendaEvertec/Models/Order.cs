using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TiendaEvertec.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string customer_name { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string customer_surname { get; set; }

        [Required]
        [Display(Name = "Documento")]
        public string customer_document { get; set; }

        [Required]
        [Display(Name = "Direccion")]
        public string customer_address { get; set; }

        [Required]
        [Display(Name = "Tipo Documento")]
        public string customer_documenttype { get; set; }

        [Display(Name = "Company")]
        public string customer_company { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string customer_email { get; set; }

        [Required]
        [Display(Name = "Telefono Celular")]
        public string customer_mobile { get; set; }

        [Display(Name = "Estado")]
        public string status { get; set; }
        [Display(Name = "Fecha Creacion")]
        public DateTime created_at { get; set; }
        [Display(Name = "Fecha Actualizacion")]
        public DateTime updated_at { get; set; }

        [Display(Name = "Costo de la Orden")]
        public decimal? OrderCost { get; set; }


        public Order() { }

    }
}