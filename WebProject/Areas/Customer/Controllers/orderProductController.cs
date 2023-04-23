using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebProject.Models;
using WebProject.Repositories;

namespace WebProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class orderProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CartVM vm { get; set; }

        public orderProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                Order = new Models.order(),
                ListCart = _unitOfWork.product_order.GetAll(u => u.userid == claim.Value, includeProperties: "product,order")
            };

            double total = 0;

            foreach (var list in vm.ListCart)
            {
                total += (list.product.price * list.quantity);
            }

            vm.Order.total = total;

            return View(vm);
        }

        public IActionResult AddToCart(int productId, int quantity)
        {
            // Get the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Create a new order instance
            var order = new order
            {
                // Set the order properties (e.g., userid, order_date, etc.)
                userid = claim.Value,
                // Add other order properties as required
                date_order = DateTime.Now,
                status = "Pending",
                total = 0,
            };

            // Create a new order_product entry
            var orderProduct = new order_product
            {
                userid = claim.Value,
                productid = productId,
                quantity = quantity,
                order = order // Set the order navigation property
            };

            // Add and save the new entry to the database
            _unitOfWork.product_order.Add(orderProduct);
            _unitOfWork.Save();

            // Redirect to the cart page
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Plus(int orderid, int productid)
        {
            var orderProduct = _unitOfWork.product_order.GetT(u => u.orderid == orderid && u.productid == productid);
            if (orderProduct != null)
            {
                orderProduct.quantity++;
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Minus(int orderid, int productid)
        {
            var orderProduct = _unitOfWork.product_order.GetT(u => u.orderid == orderid && u.productid == productid);
            if (orderProduct != null)
            {
                if (orderProduct.quantity > 1)
                {
                    orderProduct.quantity--;
                }
                else
                {
                    _unitOfWork.product_order.delete(orderProduct);
                }
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int orderid, int productid)
        {
            var orderProduct = _unitOfWork.product_order.GetT(u => u.orderid == orderid && u.productid == productid);
            if (orderProduct != null)
            {
                _unitOfWork.product_order.delete(orderProduct);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var clainsIdentity = (ClaimsIdentity)User.Identity;
            var claims = clainsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            vm = new CartVM()
            {
                ListCart = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value, includeProperties: "product,order"),
                OrderDetail = new Models.orderDetail()
            };
            vm.OrderDetail.users = _unitOfWork.user.GetT(u => u.Id == claims.Value);
            vm.OrderDetail.Name = vm.OrderDetail.users.first_name + " " + vm.OrderDetail.users.last_name;
            vm.OrderDetail.PhoneNumber = vm.OrderDetail.users.phone_number;
            vm.OrderDetail.Address = vm.OrderDetail.users.Address;

            foreach (var list in vm.ListCart)
            {
                vm.OrderDetail.price += (list.product.price * list.quantity);
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult OrdrerSuccess()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var orderList = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value);
            _unitOfWork.product_order.deleteRange(orderList);
            productOperation();
            _unitOfWork.Save();
            return View("OrderSuccess");
        }
        public void productOperation()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var orderList = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value, includeProperties: "product");

            // Decrement the stock quantity for each product in the order
            foreach (var orderProduct in orderList)
            {
                // Retrieve the product from the database
                var product = _unitOfWork.product.GetT(p => p.productid == orderProduct.productid);

                // Update the stock quantity
                product.quantity -= orderProduct.quantity;

                // Save the updated product to the database
                _unitOfWork.product.Update(product);
            }
            // Delete the order and save the changes
            _unitOfWork.product_order.deleteRange(orderList);
            _unitOfWork.Save();

        }

    }
}
