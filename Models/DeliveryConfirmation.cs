namespace Village_Manager.Models
{
    public class DeliveryConfirmation
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ConfirmationImage { get; set; }
        public string? ConfirmationNote { get; set; }
        public DateTime ConfirmedAt { get; set; } = DateTime.Now;

        public Delivery? Delivery { get; set; }
    }
}
