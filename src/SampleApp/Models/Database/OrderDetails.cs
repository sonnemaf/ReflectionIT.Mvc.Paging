using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    [Table("Order Details")]
    public partial class OrderDetails
    {
        [Column("OrderID")]
        public int OrderId { get; set; }
        [Column("ProductID")]
        public int ProductId { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Orders Order { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("OrderDetails")]
        public virtual Products Product { get; set; }
    }
}
