using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Village_Manager.Data;
using Village_Manager.Utils;
using Village_Manager.Models;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    // Hiển thị sản phẩm + tìm kiếm + lịch sử tìm kiếm
    public async Task<IActionResult> Search(
    string? search,
    List<int>? categoryIds,
    int? minPrice,
    int? maxPrice,
    string? remove,
    string? clear)
    {
        // Cart info
        ViewBag.CartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        ViewBag.CartCount = ((List<CartItem>)ViewBag.CartItems).Sum(i => i.Quantity);

        // Search history
        const string sessionKey = "SearchHistory";
        var history = HttpContext.Session.Get<List<string>>(sessionKey) ?? new List<string>();
        if (!string.IsNullOrWhiteSpace(clear)) history.Clear();
        else if (!string.IsNullOrWhiteSpace(remove)) history.Remove(remove);
        else if (!string.IsNullOrWhiteSpace(search) && !history.Contains(search)) history.Add(search);
        HttpContext.Session.Set(sessionKey, history);
        ViewBag.SearchHistory = history;

        // Base query
        var query = _context.Products
            .Include(p => p.Category)          // liên kết với ProductCategory
            .Include(p => p.ProductImages)
            .AsQueryable();

        // Filter logic
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        if (categoryIds?.Any() == true)
            query = query.Where(p => categoryIds.Contains(p.CategoryId));         // FK từ Product → ProductCategory

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        ViewBag.Categories = await _context.ProductCategories.ToListAsync();

        var products = await query.ToListAsync();
        return View("Search", products);
    }



    // Thêm vào giỏ hàng
    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        // B1: lấy giỏ hàng từ session hoặc tạo mới
        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

        // B2: kiểm tra sản phẩm đã có trong giỏ chưa
        var item = cart.FirstOrDefault(ci => ci.ProductId == productId);
        if (item != null)
        {
            item.Quantity = (item.Quantity ?? 0) + quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }

        // B3: lưu lại danh sách đơn giản (không navigation)
        HttpContext.Session.Set("Cart", cart);

        return RedirectToAction("Search");
    }

    // Trang giỏ hàng
    [HttpGet]
    public IActionResult Cart()
    {
        var cartItems = CartHelper.GetCartWithProducts(HttpContext, _context);
        return View(cartItems); // model là List<CartItem>
    }

    [HttpGet("shop/removefromcart/{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.Set("Cart", cart);
        }

        // Điều hướng quay về đúng trang (popup hoặc cart)
        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
            return Redirect(referer);
        return RedirectToAction("Cart");
    }

    public IActionResult Checkout()
    {
        // Lấy giỏ hàng từ Session
        var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        foreach (var item in cartItems)
        {
            item.Product = _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefault(p => p.Id == item.ProductId);
        }
        return View(cartItems);
    }

    public IActionResult PlaceOrder()
    {
        // Xoá giỏ hàng:
        HttpContext.Session.Remove("Cart");
        // Điều hướng đến trang thành công
        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        // Hiển thị trang thành công
        return View();
    }

    public IActionResult Tracking(string orderId)
    {
        // Có thể dùng ViewBag.OrderId = orderId; nếu muốn
        return View();
    }

    public IActionResult Detail(int id)
    {
        var product = _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Farmer)
            .Include(p => p.Category) // Nếu có navigation property tới Category
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }
        var relatedProducts = _context.Products
       .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
       .Include(p => p.ProductImages)
       .ToList();

        ViewBag.RelatedProducts = relatedProducts;
        return View(product); // Truyền đối tượng Product xuống View
    }
}
