using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Village_Manager.Data;
using Village_Manager.Models;

namespace Village_Manager.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public LocationController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
            [HttpPost("save")]
            public async Task<IActionResult> SaveLocation([FromBody] LocationRequest request)
            {
                if (request.UserId <= 0)
                    return BadRequest("UserId không hợp lệ.");

                // Kiểm tra xem đã có bản ghi chưa
                var existing = await _context.UserLocations
                    .FirstOrDefaultAsync(l => l.UserId == request.UserId);

                // Gọi API reverse geocode
                string apiKey = _config["LocationIQ:ApiKey"];
                string reverseUrl = $"https://us1.locationiq.com/v1/reverse.php?key={apiKey}&lat={request.Latitude}&lon={request.Longitude}&format=json";

                string address = "";
                using (var http = new HttpClient())
                {
                    try
                    {
                        var response = await http.GetStringAsync(reverseUrl);
                        using var doc = JsonDocument.Parse(response);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("display_name", out var displayName))
                        {
                            address = displayName.GetString();
                        }
                    }
                    catch
                    {
                        address = "Không xác định";
                    }
                }

                if (existing != null)
                {
                    // Cập nhật bản ghi cũ
                    existing.Label = request.Label;
                    existing.Address = address;
                    existing.Latitude = request.Latitude;
                    existing.Longitude = request.Longitude;
                    existing.CreatedAt = DateTime.Now;
                }
                else
                {
                    // Tạo bản ghi mới nếu chưa có
                    var location = new UserLocation
                    {
                        UserId = request.UserId,
                        Label = request.Label,
                        Address = address,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude
                    };

                    _context.UserLocations.Add(location);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Location saved or updated successfully." });
            }
        

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptGeolocation([FromBody] AcceptGeoRequest req)
        {
            var user = await _context.Users.FindAsync(req.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.HasAcceptedGeolocation = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Geolocation accepted" });
        }

    }
    public class AcceptGeoRequest
    {
        public int UserId { get; set; }
    }
    public class LocationRequest
    {
        public int UserId { get; set; }
        public string Label { get; set; } = "";
        public string Address { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
