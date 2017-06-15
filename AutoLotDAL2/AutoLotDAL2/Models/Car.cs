namespace AutoLotDAL2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Inventory")]
    public partial class Car
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Car()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int CarId { get; set; }

        [StringLength(10)]
        public string Make { get; set; }

        [StringLength(10)]
        public string Color { get; set; }

        [StringLength(10), Column("PetName")]
        public string CarNickName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        [NotMapped]
        public string MakeColor => $"{Make.Trim()} + ({Color.Trim()})";

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
