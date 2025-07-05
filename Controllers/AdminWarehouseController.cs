using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Village_Manager.Data;
using Village_Manager.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Village_Manager.Extensions;

namespace Village_Manager.Controllers;
public class AdminWarehouseController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<AdminWarehouseController> _logger;

    public AdminWarehouseController(AppDbContext context, IConfiguration configuration, IWebHostEnvironment env, ILogger<AdminWarehouseController> logger)
    {
        _context = context;
        _configuration = configuration;
        _env = env;
        _logger = logger;
    }
    // Dashboard: Trang tổng quan quản trị, hiển thị số liệu tổng hợp
    [HttpGet]
    [Route("adminwarehouse")]
    public IActionResult Dashboard()
    {
        // Kiểm tra quyền admin
        if (!HttpContext.Session.IsAdmin())
        {
            Response.StatusCode = 404;
            return View("404");
        }
        // Lấy tổng số khách hàng
        int totalCustomers = _context.Users.Count();
        ViewBag.TotalCustomers = totalCustomers;
        // Lấy tổng số sản phẩm
        int totalProducts = _context.Products.Count();
        ViewBag.TotalProducts = totalProducts;
        // Lấy tổng số đơn hàng
        int totalRetailOrders = _context.RetailOrders.Count();
        int totalOrders = totalRetailOrders;
        ViewBag.TotalOrders = totalOrders;
        // Lấy danh sách category
        var categories = _context.ProductCategory
            .Select(c => new { Name = c.Name, ImageUrl = c.ImageUrl })
            .ToList<dynamic>(); ViewBag.Categories = categories;
        // Tổng doanh thu confirmed
        decimal currentYear = DateTime.Now.Year;

        // Bán lẻ (Retail)
        var retailRevenue = _context.RetailOrders
            .Where(ro => ro.Status == "confirmed"
                && ro.ConfirmedAt.HasValue
                && ro.ConfirmedAt.Value.Year == currentYear)
            .Join(_context.RetailOrderItems,
                    ro => ro.Id,
                    ri => ri.OrderId,
                    (ro, ri) => ri.Quantity * ri.UnitPrice)
            .Sum();

        decimal totalRevenue = (retailRevenue ?? 0);
        ViewBag.TotalRevenue = totalRevenue;

        // last 4 đơn hàng
        var retailOrders = (from ro in _context.RetailOrders
                            join u in _context.Users on ro.UserId equals u.Id
                            join roi in _context.RetailOrderItems on ro.Id equals roi.OrderId into roItems
                            from roiGroup in roItems.DefaultIfEmpty()
                            group roiGroup by new { ro.Id, u.Username, ro.OrderDate, ro.Status } into g
                            select new
                            {
                                OrderId = g.Key.Id,
                                Username = g.Key.Username,
                                DatePlaced = g.Key.OrderDate,
                                OrderStatus = g.Key.Status,
                                TotalPrice = g.Sum(x => (x != null) ? x.Quantity * x.UnitPrice : 0),
                                PaymentStatus = _context.Payments.Any(p =>
                                    p.OrderId == g.Key.Id &&
                                    p.OrderType == "retail" &&
                                    p.PaymentType == "receive") ? "Paid" : "Unpaid"
                            });

        var latestOrders = retailOrders
                            .OrderByDescending(x => x.DatePlaced)
                            .Take(4)
                            .ToList();

        ViewBag.LatestOrders = latestOrders.ToList();

        return View();
    }
    //ListProduct
    [HttpGet]
    [Route("products")]
    public IActionResult Products()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .ToList();
        return View(products);
    }
    // ProductDetail
    [HttpGet]
    [Route("productdetail")]
    public IActionResult ProductDetail(int id)
    {
        var product = _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.Id == id);

        if (product == null)
            return NotFound();

        return View(product);
    }
    //Add Product
    [HttpGet]
    [Route("addproduct")]
    public IActionResult AddProduct()
    {
        return View();
    }

    [HttpPost]
    [Route("addproduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(IFormCollection form, List<IFormFile> images)
    {
        try
        {
            // Create new Product instance
            var product = new Product
            {
                Name = form["name"],
                ProductType = form["product_type"],
                CategoryId = int.TryParse(form["category_id"], out int categoryId) ? categoryId : 0,
                Quantity = int.TryParse(form["quantity"], out int quantity) ? quantity : 0,
                Price = decimal.TryParse(form["price"], out decimal price) ? price : 0,
                ExpirationDate = string.IsNullOrWhiteSpace(form["expiration_date"]) ? null : (DateTime.TryParse(form["expiration_date"], out DateTime expDate) ? expDate : (DateTime?)null),
                ProcessingTime = string.IsNullOrWhiteSpace(form["processing_time"]) ? null : (DateTime.TryParse(form["processing_time"], out DateTime procTime) ? procTime : (DateTime?)null),
                FarmerId = int.TryParse(form["farmer_id"], out int farmerId) ? farmerId : (int?)null
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {
                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        string uploadsFolder = _env.WebRootPath != null ? Path.Combine(_env.WebRootPath, "uploads") : "uploads";
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = !string.IsNullOrEmpty(uploadsFolder) ? Path.Combine(uploadsFolder, uniqueFileName) : uniqueFileName;

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = "/images/" + uniqueFileName,
                            Description = form["image_description"],
                            UploadedAt = DateTime.Now
                        };

                        _context.ProductImages.Add(productImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Redirect("products");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error adding product: " + ex.Message);
            return View();
        }
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    // DeleteProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        var images = _context.ProductImages.Where(p => p.ProductId == id).ToList();

        foreach (var image in images)
        {
                                    var filePath = _env.WebRootPath != null ? Path.Combine(_env.WebRootPath, "uploads", image.ImageUrl) : Path.Combine("uploads", image.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.ProductImages.RemoveRange(images);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Redirect("/products");
    }

    //Update Product
    [HttpGet]
    [Route("updateproduct")]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }


    [HttpPost]
    [Route("updateproduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProduct(Product model)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == model.Id);

        if (product == null)
        {
            return NotFound();
        }

        // Cập nhật chỉ nếu dữ liệu được cung cấp
        if (!string.IsNullOrEmpty(model.Name))
            product.Name = model.Name;

        if (model.CategoryId != 0)
            product.CategoryId = model.CategoryId;

        if (model.Price != 0)
            product.Price = model.Price;

        //if (model.ExpirationDate != default)
        //    product.ExpirationDate = model.ExpirationDate;

        if (!string.IsNullOrEmpty(model.ProductType))
            product.ProductType = model.ProductType;

        if (model.Quantity != 0)
            product.Quantity = model.Quantity;

        //if (model.ProcessingTime != default)
        //    product.ProcessingTime = model.ProcessingTime;

        if (model.FarmerId != 0)
            product.FarmerId = model.FarmerId;

        // Nếu có ảnh mới, thì cập nhật
        if (model.ImageUpdate != null && model.ImageUpdate.Any())
        {
            _context.ProductImages.RemoveRange(product.ProductImages);

            foreach (var file in model.ImageUpdate)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                                    + Path.GetExtension(file.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = "/images/" + fileName
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        return Redirect("/products");
    }
    [HttpGet]
    [Route("searchProduct")]
    public async Task<IActionResult> SearchProduct(string search)
    {
        var productsQuery = _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(search)) ||
                p.ProductType.ToLower().Contains(search)
            );
        }

        var products = await productsQuery.ToListAsync();
        return View("Products", products);

    }
    [HttpGet]
    [Route("alluser")]
    public async Task<IActionResult> AllUser(string searchUser, int page = 1)
    {
        try
        {
            _logger.LogInformation("Starting to load all users...");
            // Lấy thông tin session
            var username = HttpContext.Session.GetString("Username");
            var roleId = HttpContext.Session.GetInt32("RoleId");
            // Kiểm tra quyền admin
            if (string.IsNullOrEmpty(username) || roleId != 1)
            {
                _logger.LogWarning($"Unauthorized access attempt. Username: {username}, RoleId: {roleId}");
                Response.StatusCode = 404;
                return View("404");
            }
            // Phân trang và tìm kiếm
            int pageSize = 10;
            var usersQuery = _context.Users.Include(u => u.Role).AsQueryable();
            if (!string.IsNullOrEmpty(searchUser))
            {
                usersQuery = usersQuery.Where(u => u.Username.Contains(searchUser));
            }
            int totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.SearchUser = searchUser;
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading users: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while loading users.";
            return View(new List<User>());
        }
    }

    // Delete: Xóa user theo id
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("delete/{id}")]
    public IActionResult Delete(int id)
    {
        // Kiểm tra quyền admin
        if (!HttpContext.Session.IsAdmin())
        {
            Response.StatusCode = 404;
            return View("404");
        }
        // Tìm user theo id
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        // Xóa user khỏi database
        _context.Users.Remove(user);
        _context.SaveChanges();
        var currentUserId = HttpContext.Session.GetInt32("UserId");
        LogHelper.SaveLog(_context, currentUserId, $"Xóa user: {user.Username} (ID: {user.Id})");
        return RedirectToAction("AllUser");
    }

    // AddUser (GET): Hiển thị form thêm user mới
    [HttpGet]
    [Route("adduser")]
    public IActionResult AddUser()
    {
        // Kiểm tra quyền admin
        if (!HttpContext.Session.IsAdmin())
        {
            Response.StatusCode = 404;
            return View("404");
        }
        // Lấy danh sách role để hiển thị trong form
        ViewBag.Roles = _context.Roles.ToList();
        return View();
    }

    // AddUser (POST): Xử lý thêm user mới
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("adduser")]
    public async Task<IActionResult> AddUser(User user, string confirmPassword)
    {
        try
        {
            // Kiểm tra quyền admin
            if (!HttpContext.Session.IsAdmin())
            {
                Response.StatusCode = 404;
                return View("404");
            }
            _logger.LogInformation("Starting user creation process...");
            ViewBag.Roles = await _context.Roles.ToListAsync();
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email) || user.RoleId <= 0)
            {
                ModelState.AddModelError("", "Please fill in all required fields");
                return View(user);
            }
            // Kiểm tra xác nhận mật khẩu
            if (user.Password != confirmPassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match");
                return View(user);
            }
            // Kiểm tra username/email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(user);
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(user);
            }
            // Kiểm tra định dạng số điện thoại (10 chữ số)
            if (!string.IsNullOrEmpty(user.Phone) && (user.Phone.Length != 10 || !user.Phone.All(char.IsDigit)))
            {
                ModelState.AddModelError("Phone", "Phone number must be exactly 10 digits");
                return View(user);
            }
            // Tạo user mới
            var newUser = new User
            {
                Username = user.Username.Trim(),
                Email = user.Email.Trim(),
                Password = user.Password,
                RoleId = user.RoleId,
                Phone = user.Phone?.Trim(),
                CreatedAt = DateTime.Now
            };
            // Lưu vào database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Thêm vào bảng Farmer nếu role là farmer
            var farmerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "farmer");
            if (farmerRole != null && newUser.RoleId == farmerRole.Id)
            {
                var newFarmer = new Farmer
                {
                    UserId = newUser.Id,
                    FullName = newUser.Username, // hoặc lấy từ form nếu có trường tên đầy đủ
                    Phone = newUser.Phone,
                    Address = "" // lấy từ form nếu có, hoặc để trống
                };
                _context.Farmers.Add(newFarmer);
                await _context.SaveChangesAsync();
            }

            var shipperRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "shipper");
            if (shipperRole != null && newUser.RoleId == shipperRole.Id)
            {
                var newShipper = new Shipper
                {
                    UserId = newUser.Id,
                    FullName = newUser.Username, // hoặc lấy từ form nếu có
                    Phone = newUser.Phone,
                    VehicleInfo = "" // lấy từ form nếu có
                };
                _context.Shippers.Add(newShipper);
                await _context.SaveChangesAsync();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            LogHelper.SaveLog(_context, currentUserId, $"Thêm user mới: {newUser.Username} (ID: {newUser.Id})");
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("AllUser");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating user: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while creating the user. Please try again.");
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }
    }

    // EditUser (GET): Hiển thị form chỉnh sửa user
    [HttpGet]
    [Route("/edituser/{id}")]
    public async Task<IActionResult> EditUser(int id)
    {
        try
        {
            // Kiểm tra quyền admin
            if (!HttpContext.Session.IsAdmin())
            {
                Response.StatusCode = 404;
                return View("404");
            }
            _logger.LogInformation($"Loading user for edit. UserId: {id}");
            // Lấy user theo id
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User not found. UserId: {id}");
                return NotFound();
            }
            ViewBag.Roles = await _context.Roles.ToListAsync();
            _logger.LogInformation($"Successfully loaded user for edit. UserId: {id}");
            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading user for edit: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while loading user data.";
            return RedirectToAction("AllUser");
        }
    }

    // EditUser (POST): Xử lý cập nhật thông tin user
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("edituser/{id}")]
    public async Task<IActionResult> EditUser(int id, User user, string newPassword)
    {
        try
        {
            // Kiểm tra quyền admin
            if (!HttpContext.Session.IsAdmin())
            {
                Response.StatusCode = 404;
                return View("404");
            }
            _logger.LogInformation($"Processing user update. UserId: {id}");
            ViewBag.Roles = await _context.Roles.ToListAsync();
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || user.RoleId <= 0)
            {
                _logger.LogWarning($"Invalid user data. Username: {user.Username}, Email: {user.Email}, RoleId: {user.RoleId}");
                ModelState.AddModelError("", "Please fill in all required fields");
                return View(user);
            }
            // Kiểm tra định dạng số điện thoại (10 chữ số)
            if (!string.IsNullOrEmpty(user.Phone) && (user.Phone.Length != 10 || !user.Phone.All(char.IsDigit)))
            {
                ModelState.AddModelError("Phone", "Phone number must be exactly 10 digits");
                return View(user);
            }
            // Lấy user hiện tại
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                _logger.LogWarning($"User not found for update. UserId: {id}");
                return NotFound();
            }
            // Kiểm tra username/email đã tồn tại chưa (trừ user hiện tại)
            if (await _context.Users.AnyAsync(u => u.Username == user.Username && u.Id != id))
            {
                _logger.LogWarning($"Username already exists: {user.Username}");
                ModelState.AddModelError("Username", "Username already exists");
                return View(user);
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
            {
                _logger.LogWarning($"Email already exists: {user.Email}");
                ModelState.AddModelError("Email", "Email already exists");
                return View(user);
            }
            // Cập nhật thông tin user
            existingUser.Username = user.Username.Trim();
            existingUser.Email = user.Email.Trim();
            existingUser.RoleId = user.RoleId;
            existingUser.Phone = user.Phone?.Trim();
            // Nếu có nhập mật khẩu mới thì cập nhật
            if (!string.IsNullOrEmpty(newPassword))
            {
                existingUser.Password = newPassword;
                _logger.LogInformation($"Password updated for user. UserId: {id}");
            }
            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Cập nhật hoặc tạo Farmer nếu role là farmer
            var farmerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "farmer");
            if (farmerRole != null && existingUser.RoleId == farmerRole.Id)
            {
                var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == existingUser.Id);
                if (farmer == null)
                {
                    // Nếu chưa có thì tạo mới
                    var newFarmer = new Farmer
                    {
                        UserId = existingUser.Id,
                        FullName = existingUser.Username, // hoặc lấy từ form nếu có trường tên đầy đủ
                        Phone = existingUser.Phone,
                        Address = "" // lấy từ form nếu có, hoặc để trống
                    };
                    _context.Farmers.Add(newFarmer);
                }
                else
                {
                    // Nếu đã có thì cập nhật thông tin
                    farmer.FullName = existingUser.Username; // hoặc lấy từ form nếu có trường tên đầy đủ
                    farmer.Phone = existingUser.Phone;
                    // farmer.Address = ... // lấy từ form nếu có
                }
                await _context.SaveChangesAsync();
            }

            var shipperRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == "shipper");
            if (shipperRole != null && existingUser.RoleId == shipperRole.Id)
            {
                var shipper = await _context.Shippers.FirstOrDefaultAsync(s => s.UserId == existingUser.Id);
                if (shipper == null)
                {
                    shipper = new Shipper
                    {
                        UserId = existingUser.Id,
                        FullName = existingUser.Username,
                        Phone = existingUser.Phone,
                        VehicleInfo = ""
                    };
                    _context.Shippers.Add(shipper);
                }
                else
                {
                    shipper.FullName = existingUser.Username;
                    shipper.Phone = existingUser.Phone;
                    // shipper.VehicleInfo = ... // cập nhật nếu có
                }
                await _context.SaveChangesAsync();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            LogHelper.SaveLog(_context, currentUserId, $"Cập nhật user: {existingUser.Username} (ID: {existingUser.Id})");
            _logger.LogInformation($"User updated successfully. UserId: {id}");
            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("AllUser");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating user: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while updating the user. Please try again.");
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }
    }

    // view to change profile setting
    [HttpGet("profilesetting")]
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        return View("~/Views/AdminWarehouse/ProfileSetting.cshtml", user);
    }

    // view to changge create role 
    [HttpGet("createrole")]
    public IActionResult CreateRole()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        return View("~/Views/AdminWarehouse/CreateRole.cshtml", user);
    }

    // Logs: Hiển thị lịch sử thao tác (log) của admin
    [HttpGet]
    [Route("logs")]
    public IActionResult Logs(int page = 1)
    {
        // Kiểm tra quyền admin
        if (!HttpContext.Session.IsAdmin())
        {
            Response.StatusCode = 404;
            return View("404");
        }
        int pageSize = 20;
        // Lấy danh sách log, join với user để lấy tên
        var logs = (from l in _context.Logs
                    join u in _context.Users on l.UserId equals u.Id into userJoin
                    from u in userJoin.DefaultIfEmpty()
                    orderby l.CreatedAt descending
                    select new Village_Manager.Models.Dto.LogViewModel
                    {
                        Username = u != null ? u.Username : "Unknown",
                        Action = l.Action,
                        CreatedAt = l.CreatedAt
                    })
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)_context.Logs.Count() / pageSize);
        return View("~/Views/AdminWarehouse/Logs.cshtml", logs);
    }

    // Lấy danh sách farmer
    [HttpGet]
    [Route("famer")]
    public IActionResult Famer()
    {
        var farmers = (from f in _context.Farmers
                       join u in _context.Users on f.UserId equals u.Id
                       join r in _context.Roles on u.RoleId equals r.Id
                       where r.Name == "farmer"
                       select new
                       {
                           FarmerId = f.Id,
                           FarmerName = f.FullName,
                           FarmerPhone = f.Phone,
                           FarmerAddress = f.Address,
                           Username = u.Username,
                           Email = u.Email,
                           CreatedAt = u.CreatedAt,
                           UserId = u.Id
                       }).ToList();

        ViewBag.Farmers = farmers.ToList();

        return View();
    }

    //update farmer thông tin
    [HttpPost]
    [Route("famer/update")]
    public IActionResult UpdateFarmer(int FarmerId, string FarmerName, string FarmerPhone, string FarmerAddress)
    {
        var farmer = _context.Farmers.FirstOrDefault(f => f.Id == FarmerId);
        if (farmer != null)
        {
            farmer.FullName = FarmerName;
            farmer.Phone = FarmerPhone;
            farmer.Address = FarmerAddress;

            _context.SaveChanges();
        }

        return RedirectToAction("Famer");
    }

    // xóa role farmer
    [HttpPost]
    [Route("famer/change-role")]
    public IActionResult ChangeRole(int UserId)
    {
        // retail_customer = role_id 5
        int newRoleId = 5;

        var user = _context.Users.FirstOrDefault(u => u.Id == UserId);
        if (user != null)
        {
            user.RoleId = newRoleId;
            _context.SaveChanges();
        }

        return RedirectToAction("Famer");
    }
    [HttpGet]
    [Route("addfamer")]
    public IActionResult AddFamer()
    {
        var pending = _context.FarmerRegistrationRequest
            .Where(r => r.status == "pending")
            .OrderByDescending(r => r.requested_at)
            .ToList();

        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _context.FarmerRegistrationRequest.FindAsync(id);

        if (request == null || request.status != "pending")
            return NotFound();

        request.status = "approved";
        request.reviewed_at = DateTime.Now;
        request.reviewed_by = HttpContext.Session.GetInt32("UserId");

        // Tạo bản ghi mới trong bảng Farmers
        _context.Farmers.Add(new Farmer
        {
            UserId = request.user_id,
            FullName = request.full_name,
            Phone = request.phone,
            Address = request.address
        });

        var user = await _context.Users.FindAsync(request.user_id);
        if (user != null)
        {
            user.RoleId = 5;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("AddFamer");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _context.FarmerRegistrationRequest.FindAsync(id);

        if (request == null || request.status != "pending")
            return NotFound();

        request.status = "rejected";
        request.reviewed_at = DateTime.Now;
        request.reviewed_by = HttpContext.Session.GetInt32("UserId");

        await _context.SaveChangesAsync();
        return RedirectToAction("AddFamer");

    }
}


