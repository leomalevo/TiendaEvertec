//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TiendaEvertec.core.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public orders()
        {
            this.orderProducts = new HashSet<orderProducts>();
        }
    
        public int Id { get; set; }
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_mobile { get; set; }
        public string status { get; set; }
        public System.DateTime created_at { get; set; }
        public System.DateTime updated_at { get; set; }
        public Nullable<int> IdRequest { get; set; }
        public string customer_surname { get; set; }
        public string customer_document { get; set; }
        public string customer_documenttype { get; set; }
        public string customer_address { get; set; }
        public string customer_company { get; set; }
        public Nullable<decimal> OrderCost { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<orderProducts> orderProducts { get; set; }
    }
}
