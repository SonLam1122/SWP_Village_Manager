namespace Village_Manager.Models
{
    public class DeliveryLog
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }

        public Delivery Delivery { get; set; }
        public User UpdatedByUser { get; set; }
    }
}
