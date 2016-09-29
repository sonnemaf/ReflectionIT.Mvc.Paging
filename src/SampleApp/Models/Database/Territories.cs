using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    public partial class Territories
    {
        public Territories()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
        }

        [Column("TerritoryID")]
        [MaxLength(20)]
        [Key]
        public string TerritoryId { get; set; }
        [Required]
        [Column(TypeName = "nchar(50)")]
        public string TerritoryDescription { get; set; }
        [Column("RegionID")]
        public int RegionId { get; set; }

        [InverseProperty("Territory")]
        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
        [ForeignKey("RegionId")]
        [InverseProperty("Territories")]
        public virtual Region Region { get; set; }
    }
}
