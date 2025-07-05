using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Village_Manager.Models
{
    [Table("shipper")]
    public class Shipper
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("full_name")]
        public string? FullName { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("vehicle_info")]
        public string? VehicleInfo { get; set; }
    }
} 