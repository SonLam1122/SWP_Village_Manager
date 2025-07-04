using Microsoft.AspNetCore.Mvc;
using Village_Manager.Data;
using Village_Manager.Models;

namespace Village_Manager.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class report_chart : ControllerBase
    {
        private readonly AppDbContext _context;
        public report_chart(AppDbContext context)
        {
            _context = context;
        }

        // RetailOrder, RetailOrderItem và WholesaleOrder, WholesaleOrderItem
        [HttpGet("total-revenue-by-month")]
        public IActionResult TotalRevenueByMonth([FromQuery] int year = 2024)
        {
            // BÁN LẺ
            var retailQuery = from order in _context.RetailOrders
                              where order.ConfirmedAt.HasValue && order.ConfirmedAt.Value.Year == year
                              join item in _context.RetailOrderItems on order.Id equals item.OrderId
                              select new
                              {
                                  Month = order.ConfirmedAt.Value.Month,
                                  Revenue = item.Quantity * item.UnitPrice
                              };

            // BÁN BUÔN
            var wholesaleQuery = from order in _context.WholesaleOrders
                                 where order.ConfirmedAt.HasValue && order.ConfirmedAt.Value.Year == year
                                 join item in _context.WholesaleOrderItems on order.Id equals item.OrderId
                                 select new
                                 {
                                     Month = order.ConfirmedAt.Value.Month,
                                     Revenue = item.Quantity * item.UnitPrice
                                 };

            // GỘP doanh thu cả 2 loại
            var allData = retailQuery.Concat(wholesaleQuery)
                .ToList()
                .GroupBy(x => x.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Month)
                .ToList();

            // Đảm bảo trả về 12 tháng
            var months = Enumerable.Range(1, 12).ToList();
            var chartData = months.Select(m => allData.FirstOrDefault(x => x.Month == m)?.TotalRevenue ?? 0).ToList();

            return Ok(new
            {
                categories = months.Select(m => new DateTime(year, m, 1).ToString("MMM")).ToList(),
                series = new[]
                {
            new { name = "Tổng doanh thu", data = chartData }
        }
            });
        }
    }
}
