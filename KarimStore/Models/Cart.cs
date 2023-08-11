namespace KarimStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart
    {
        public int CartID { get; set; }

        public int? InvoiceHistoryID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        public int ProductID { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? UnitPrice { get; set; }

        public int? Quantity { get; set; }

        public virtual InvoiceHistory InvoiceHistory { get; set; }
    }
}
