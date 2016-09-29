using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Models.Database
{
    public partial class Employees
    {
        public Employees()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
            Orders = new HashSet<Orders>();
        }

        [Column("EmployeeID")]
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(10)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string Title { get; set; }
        [MaxLength(25)]
        public string TitleOfCourtesy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }
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
        public string HomePhone { get; set; }
        [MaxLength(4)]
        public string Extension { get; set; }
        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }
        [Column(TypeName = "ntext")]
        public string Notes { get; set; }
        public int? ReportsTo { get; set; }
        [MaxLength(255)]
        public string PhotoPath { get; set; }

        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<Orders> Orders { get; set; }
        [ForeignKey("ReportsTo")]
        [InverseProperty("InverseReportsToNavigation")]
        public virtual Employees ReportsToNavigation { get; set; }
        [InverseProperty("ReportsToNavigation")]
        public virtual ICollection<Employees> InverseReportsToNavigation { get; set; }
    }
}
