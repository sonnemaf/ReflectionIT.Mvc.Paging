using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    public partial class Orders
    {
        public Orders()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        [Column("OrderID")]
        [Key]
        public int OrderId { get; set; }
        [Column("CustomerID", TypeName = "nchar(5)")]
        public string CustomerId { get; set; }
        [Column("EmployeeID")]
        public int? EmployeeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RequiredDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        [Column(TypeName = "money")]
        public decimal? Freight { get; set; }
        [MaxLength(40)]
        public string ShipName { get; set; }
        [MaxLength(60)]
        public string ShipAddress { get; set; }
        [MaxLength(15)]
        public string ShipCity { get; set; }
        [MaxLength(15)]
        public string ShipRegion { get; set; }
        [MaxLength(10)]
        public string ShipPostalCode { get; set; }
        [MaxLength(15)]
        public string ShipCountry { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
        [ForeignKey("CustomerId")]
        [InverseProperty("Orders")]
        public virtual Customers Customer { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("Orders")]
        public virtual Employees Employee { get; set; }
        [ForeignKey("ShipVia")]
        [InverseProperty("Orders")]
        public virtual Shippers ShipViaNavigation { get; set; }
    }
}
