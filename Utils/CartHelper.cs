using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Village_Manager.Models;
using Village_Manager.Data;

namespace Village_Manager.Utils
{
    public static class CartHelper
    {
        public static List<CartItem> GetCartWithProducts(HttpContext context, AppDbContext db)
        {
            var cartSession = context.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var productIds = cartSession.Select(c => c.ProductId).ToList();
            var cartItems = new List<CartItem>();

            if (productIds.Any())
            {
                var products = db.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Include(p => p.ProductImages)
                    .ToList();

                foreach (var item in cartSession)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        cartItems.Add(new CartItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Product = product
                        });
                    }
                }
            }

            return cartItems;
        }
    }
}