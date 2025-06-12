using Microsoft.AspNetCore.Mvc;
using Village_Manager.Services.IService;

namespace Village_Manager.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? id )
        {
              
                var product = await _productService.GetProductByIdAsync(id ?? 0);
                if (product != null)
                {
                    ViewBag.Product = product;
                }
                else
                {
                    ViewBag.Error = "Product not found.";
                }
                return View();
            }
           
            
        }
    }

