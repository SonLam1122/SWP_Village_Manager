using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Village_Manager.Models
{
    public class FarmerRegistrationRequest
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        [StringLength(100)]
        public string full_name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string phone { get; set; } = string.Empty;

        [Required]
        public string address { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string status { get; set; } = "pending";

        public DateTime requested_at { get; set; } = DateTime.Now;

        public DateTime? reviewed_at { get; set; }

        public int? reviewed_by { get; set; }

        // Optional: navigation properties
        [ForeignKey("user_id")]
        public User? User { get; set; }

        [ForeignKey("reviewed_by")]
        public User? ReviewedBy { get; set; }
    }
}
