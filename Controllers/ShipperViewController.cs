using Microsoft.AspNetCore.Mvc;
using Village_Manager.Data;
using Village_Manager.Models.ViewModels;

namespace Village_Manager.Controllers
{
    public class ShipperViewController : Controller
    {
        private readonly AppDbContext _context;
        public ShipperViewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("dashboardshipper")]
        public IActionResult DashboardShipper()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Home");

            // Lấy id của shipper dựa vào userId
            var shipperEntity = _context.Shippers.FirstOrDefault(s => s.UserId == userId);
            if (shipperEntity == null)
                return RedirectToAction("Login", "Home");
            int shipperId = shipperEntity.Id;

            var shipper = _context.Users.FirstOrDefault(u => u.Id == userId);

            var totalDeliveries = _context.Deliveries.Count(d => d.ShipperId == shipperId);
            var deliveryStatuses = _context.Deliveries
                .Where(d => d.ShipperId == shipperId)
                .Select(d => new {
                    d.Id,
                    LastStatus = _context.DeliveryLogs
                        .Where(l => l.DeliveryId == d.Id)
                        .OrderByDescending(l => l.LogTime)
                        .Select(l => l.Status)
                        .FirstOrDefault()
                })
                .ToList();

            var deliveringOrders = deliveryStatuses.Count(ds => new[] { "Đang giao", "in_transit" }.Contains(ds.LastStatus));
            var completedOrders = deliveryStatuses.Count(ds => new[] { "Hoàn thành", "delivered" }.Contains(ds.LastStatus));
            var pendingOrders = _context.Deliveries.Count(d => d.ShipperId == null);

            var recentDeliveries = _context.Deliveries
                .Where(d => d.ShipperId == shipperId)
                .OrderByDescending(d => d.StartTime ?? DateTime.MinValue)
                .Take(4)
                .Select(d => new RecentOrder
                {
                    OrderId = d.OrderId ?? 0,
                    CustomerName = d.OrderId != null
                        ? (_context.RetailOrders.Where(o => o.Id == d.OrderId).Select(o => o.User.Username).FirstOrDefault() ?? "")
                        : "",
                    Status = _context.DeliveryLogs
                        .Where(l => l.DeliveryId == d.Id)
                        .OrderByDescending(l => l.LogTime)
                        .Select(l => l.Status)
                        .FirstOrDefault() ?? "",
                    Address = d.OrderId != null
                        ? (_context.RetailOrders.Where(o => o.Id == d.OrderId)
                            .SelectMany(o => o.User.Addresses)
                            .Select(a => a.AddressLine)
                            .FirstOrDefault() ?? "")
                        : "",
                    Time = d.StartTime.HasValue ? d.StartTime.Value.ToString("HH:mm") : ""
                })
                .ToList();

            var vm = new ShipperDashboardViewModel
            {
                ShipperName = shipper?.Username ?? "",
                ShipperEmail = shipper?.Email ?? "",
                TotalDeliveries = totalDeliveries,
                DeliveringOrders = deliveringOrders,
                CompletedOrders = completedOrders,
                PendingOrders = pendingOrders,
                RecentDeliveries = recentDeliveries
            };

            return View(vm);
        }
        public IActionResult OrdersShipper()
        {
            return View();
        }
        public IActionResult DeliveriesShipper()
        {
            return View();
        }
        public IActionResult ProfileShipper()
        {
            return View();
        }
        public IActionResult HistoryShipper()
        {
            return View();
        }
        public IActionResult NotificationsShipper()
        {
            return View();
        }
        public IActionResult LogsShipper()
        {
            return View();
        }
    }
}
