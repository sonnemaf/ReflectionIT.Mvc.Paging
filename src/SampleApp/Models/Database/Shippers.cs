using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    public partial class Shippers
    {
        public Shippers()
        {
            Orders = new HashSet<Orders>();
        }

        [Column("ShipperID")]
        [Key]
        public int ShipperId { get; set; }
        [Required]
        [MaxLength(40)]
        public string CompanyName { get; set; }
        [MaxLength(24)]
        public string Phone { get; set; }

        [InverseProperty("ShipViaNavigation")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
