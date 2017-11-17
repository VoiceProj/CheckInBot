namespace CheckInBot.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CheckIn")]
    public partial class CheckIn
    {
        [Key]
        [Column(Order = 0, TypeName = "numeric")]
        public decimal ID { get; set; }
        
        [Column(Order = 1, TypeName = "numeric")]
        public decimal UserID { get; set; }
        
        [Column(Order = 2)]
        [StringLength(15)]
        public string Name { get; set; }
        
        [Column(Order = 3)]
        [StringLength(15)]
        public string LastName { get; set; }
        
        [Column(Order = 4)]
        [StringLength(20)]
        public string UserName { get; set; }
        
        [Column(Order = 5)]
        public DateTime TimeStamp { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string IsHere { get; set; }

    }
}
