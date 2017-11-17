namespace CheckInBot.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Registration")]
    public partial class Registration
    {
        [Key]
        [Column(TypeName = "numeric")]
        public decimal ID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }
    }
}
