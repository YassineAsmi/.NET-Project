using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using WebProject.Models;
using WebProject.Repositories;

namespace WebProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ordersController : Controller
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public ordersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 5)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string currentUserId = claim.Value;

            int skipCount = (pageNumber - 1) * pageSize;

            IEnumerable<order> orders = _unitOfWork.order.GetAll(o => o.userid == currentUserId)
                                                        .Skip(skipCount)
                                                        .Take(pageSize)
                                                        .ToList().OrderByDescending(o => o.date_order);
            int totalOrders = _unitOfWork.order.GetAll(o => o.userid == currentUserId).Count();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return View(orders);
        }
        [HttpGet]
        public IActionResult OrderDetails(int orderId)
        {
            // Fetch the order details including related order_product records
            var orderDetail = _unitOfWork.orderDetail.GetT(o => o.orderid == orderId);

            if (orderDetail == null)
            {
                return View("Error");
            }

            // Fetch the related order_product records
            var orderProducts = _unitOfWork.product_order.GetAll(op => op.orderid == orderId, includeProperties: "product");
            // Create a view model to hold both the order details and the ordered products
            var viewModel = new OrderDetailsViewModel
            {
                OrderDetail = orderDetail,
                OrderedProducts = orderProducts.ToList()
            };

            return View(viewModel);
        }
    }
}
