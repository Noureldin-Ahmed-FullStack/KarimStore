namespace KarimStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfilePic
    {
        [Key]
        public int ImgID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public byte[] ImageFile { get; set; }
    }
}
