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
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            int totalUsers = _unitOfWork.user.GetAll().Count();
            int totalProducts = _unitOfWork.product.GetAll().Count();
            int totalOrders = _unitOfWork.order.GetAll().Count();

            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var totalEarnedThisMonth = _unitOfWork.order.GetAll(o => o.date_order >= startDate && o.date_order <= endDate).Sum(o => o.total);
            ViewBag.TotalEarnedThisMonth = totalEarnedThisMonth;
            int ordersThisMonth = _unitOfWork.order.GetAll(o => o.date_order >= startOfMonth && o.date_order <= endOfMonth).Count();

            // Calculate the most ordered product
            var mostOrderedProduct = _unitOfWork.product_order
                .GetAll()
                .GroupBy(po => po.productid)
                .OrderByDescending(g => g.Sum(po => po.quantity))
                .FirstOrDefault();

            string mostOrderedProductName = "";
            int mostOrderedProductQuantity = 0;

            if (mostOrderedProduct != null)
            {
                mostOrderedProductName = mostOrderedProduct.First().product.name;
                mostOrderedProductQuantity = mostOrderedProduct.Sum(po => po.quantity);
            }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.OrdersThisMonth = ordersThisMonth;
            ViewBag.MostOrderedProductName = mostOrderedProductName;
            ViewBag.MostOrderedProductQuantity = mostOrderedProductQuantity;

            return View();
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
        public IActionResult Details(order_product order_Product,int quantity)
        {
            if (ModelState.IsValid)
            {
                order_Product.quantity = quantity;
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