﻿namespace AutoLotDAL2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CreditRisk
    {
        [Key]
        public int CustId { get; set; }

        [StringLength(50)]
        [Index("IDX_CreditRisk_Name",IsUnique =true,Order =2)]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Index("IDX_CreditRisk_Name", IsUnique = true, Order = 1)]
        public string LastName { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
