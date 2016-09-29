using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database {
    public partial class Suppliers {
        public Suppliers() {
            Products = new HashSet<Products>();
        }

        [Column("SupplierID")]
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(40)]
        [Microsoft.AspNetCore.Mvc.Remote("IsCompanyNameAvailable", "Suppliers", "", AdditionalFields = "SupplierId")]
        public string CompanyName { get; set; }
        [MaxLength(30)]
        public string ContactName { get; set; }
        [MaxLength(30)]
        public string ContactTitle { get; set; }
        [MaxLength(60)]
        public string Address { get; set; }
        [MaxLength(15)]
        public string City { get; set; }
        [MaxLength(15)]
        public string Region { get; set; }
        [MaxLength(10)]
        public string PostalCode { get; set; }
        [MaxLength(15)]
        public string Country { get; set; }
        [MaxLength(24)]
        public string Phone { get; set; }
        [MaxLength(24)]
        public string Fax { get; set; }
        [Column(TypeName = "ntext")]
        public string HomePage { get; set; }

        [InverseProperty("Supplier")]
        public virtual ICollection<Products> Products { get; set; }
    }
}
