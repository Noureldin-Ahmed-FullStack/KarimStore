namespace KarimStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InvoiceHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvoiceHistory()
        {
            Carts = new HashSet<Cart>();
            Invoices = new HashSet<Invoice>();
        }

        [Key]
        public int InvoicesHistoryID { get; set; }

        [StringLength(128)]
        public string CustomerID { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? Total { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime? InvDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
