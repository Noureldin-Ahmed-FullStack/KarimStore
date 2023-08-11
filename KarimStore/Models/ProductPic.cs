namespace KarimStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductPic
    {
        [Key]
        public int ImageID { get; set; }

        public int? ProductID { get; set; }

        public byte[] ImageFile { get; set; }

        public virtual Product Product { get; set; }
    }
}
