using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    public partial class Categories
    {
        public Categories()
        {
            Products = new HashSet<Products>();
        }

        [Column("CategoryID")]
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(15)]
        public string CategoryName { get; set; }
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        [Column(TypeName = "image")]
        public byte[] Picture { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Products> Products { get; set; }
    }
}
