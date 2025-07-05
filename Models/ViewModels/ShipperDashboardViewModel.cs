namespace Village_Manager.Models.ViewModels
{
    public class ShipperDashboardViewModel
    {
        public string ShipperName { get; set; } = "";
        public string ShipperEmail { get; set; } = "";
        public int TotalDeliveries { get; set; }
        public int DeliveringOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public List<RecentOrder> RecentDeliveries { get; set; } = new();
    }

    public class RecentOrder
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Status { get; set; } = "";
        public string Address { get; set; } = "";
        public string Time { get; set; } = "";
    }
} 