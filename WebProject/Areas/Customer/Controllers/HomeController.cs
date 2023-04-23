using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebProject.Models;
using WebProject.Repositories;

namespace WebProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index(string searchQuery)
        {
            IEnumerable<product> products;

            if (string.IsNullOrEmpty(searchQuery))
            {
                products = _unitOfWork.product.GetAll();
            }
            else
            {
                products = _unitOfWork.product.GetAll(p => p.name.Contains(searchQuery));
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int? productId)
        {
            if (productId == null)
            {
                // Return an appropriate response, like a NotFound view or BadRequest
                return NotFound();
            }
            
            order Order = new order()
            {
                date_order = DateTime.Now,
                status = "Pending",
                total = 0
                
            };
            _unitOfWork.order.Add(Order);
            order_product order_Product = new order_product()
            {
                product = _unitOfWork.product.GetT(x => x.productid == productId),
                quantity = 1,
                productid = productId.Value
            };

            return View(order_Product);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(order_product order_Product)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                order_Product.userid = claims.Value;
                var cartItem = _unitOfWork.product_order.GetT(x => x.productid == order_Product.productid && x.userid == claims.Value);
                if(cartItem == null)
                {
                    _unitOfWork.product_order.Add(order_Product);
                    _unitOfWork.Save();
                 //HttpContext.Session.SetInt32("SessionCart", _unitOfWork.product_order.GetAll(x => x.userid == claims.Value).ToList().Count);
                }
              //  else
             //   {
              //      _unitOfWork.product_order.IncrementCartItem(cartItem, order_Product.quantity);
              //      _unitOfWork.Save();
               // }
            }
            return RedirectToAction("Index");
        }
   
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}