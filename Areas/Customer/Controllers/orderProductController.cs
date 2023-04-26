using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
                ListCart = _unitOfWork.product_order.GetAll(u => u.userid == claim.Value && u.isInCart == true, includeProperties: "product,order")
            };

            double total = 0;
            int qty = 0;
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

            // Check if there's an existing order for the user
            var existingOrder = _unitOfWork.order.GetT(o => o.userid == claim.Value && o.status == "Pending");

            // If there's no existing order, create a new order
            if (existingOrder == null)
            {
                existingOrder = new order
                {
                    // Set the order properties (e.g., userid, order_date, etc.)
                    userid = claim.Value,
                    date_order = DateTime.Now,
                    status = "Pending",
                    total = 0,
                };
                _unitOfWork.order.Add(existingOrder);
                _unitOfWork.Save();
            }


            var existingOrderProduct = _unitOfWork.product_order.GetT(x => x.orderid == existingOrder.orderid && x.productid == productId);


            if (existingOrderProduct == null)
            {
                var orderProduct = new order_product
                {
                    userid = claim.Value,
                    productid = productId,
                    product = _unitOfWork.product.GetT(p => p.productid == productId),
                    quantity = quantity,
                    order = existingOrder ,
                    isInCart = true
                };
                _unitOfWork.product_order.Add(orderProduct);
            }
            else
            {
                existingOrderProduct.quantity += quantity;
            }

            _unitOfWork.Save();

         var userconnected =   _unitOfWork.user.GetT(x=>x.Id==claim.Value);

            var product = _unitOfWork.product.GetT(p => p.productid == productId);
            var existingOrderDetail = _unitOfWork.orderDetail.GetT(od => od.orderid == existingOrder.orderid && od.productid == productId);

            if (existingOrderDetail == null)
            {
                var orderDetail = new orderDetail
                {
                    orderid = existingOrder.orderid,
                    productid = productId,
                    Name = userconnected.first_name + " " + userconnected.last_name,
                    PhoneNumber = userconnected.phone_number,
                    Address = userconnected.Address,
                    price = product.price,
                    quantity = quantity,
                    userid = claim.Value
                };
                _unitOfWork.orderDetail.Add(orderDetail);
            }
            else
            {
                existingOrderDetail.quantity += quantity;
            }

            // Update the order total
            existingOrder.total += product.price * quantity;

            // Save the changes to the database
            _unitOfWork.order.update(existingOrder);
            _unitOfWork.Save();

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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Add this line to get the orderId
            int orderIdd = _unitOfWork.order.GetT(o => o.userid == claims.Value && o.status == "Pending").orderid;

            vm = new CartVM()
            {
                ListCart = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value && u.isInCart == true, includeProperties: "product,order"),
                OrderDetail = new orderDetail(),
                OrderedProducts = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value && u.orderid == orderIdd, includeProperties: "product")
            };
            vm.OrderDetail.users = _unitOfWork.user.GetT(u => u.Id == claims.Value);
            vm.OrderDetail.Name = vm.OrderDetail.users.first_name + " " + vm.OrderDetail.users.last_name;
            vm.OrderDetail.PhoneNumber = vm.OrderDetail.users.phone_number;
            vm.OrderDetail.Address = vm.OrderDetail.users.Address;

            double total = 0;
            foreach (var list in vm.ListCart)
            {
                total += (list.product.price * list.quantity);
            }
            Debug.WriteLine("Total: "+total.ToString());
            vm.OrderDetail.total = total;

            return View(vm);
        }



        public IActionResult OrderSuccess()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            int orderId = _unitOfWork.order.GetT(o => o.userid == claims.Value && o.status == "Pending")?.orderid ?? 0;

            var orderList = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value && u.orderid == orderId, includeProperties: "product,users");

            // Calculate the total price based on the product_order records
            double totalPrice = orderList.Sum(item => item.product.price * item.quantity);


            // Create an instance of OrderDetailsViewModel
            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                // Assign the list of ordered products
                OrderedProducts = orderList.ToList()
            };

            // Copy the order_products data into the order_history table
            foreach (var orderProduct in orderList)
            {
                var orderHistory = new OrderHistory
                {
                    UserId = orderProduct.userid,
                    ProductId = orderProduct.productid,
                    Quantity = orderProduct.quantity,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Completed"
                };
                _unitOfWork.orderHistory.Add(orderHistory);

                // Insert data into the orderDetails table
                var orderDetail = new orderDetail
                {
                    orderid = orderId,
                    productid = orderProduct.productid,
                    Name = orderProduct.users.first_name + " " + orderProduct.users.last_name,
                    PhoneNumber = orderProduct.users.phone_number,
                    Address = orderProduct.users.Address,
                    price = orderProduct.product.price,
                    quantity = orderProduct.quantity,
                    userid = orderProduct.userid
                };

                orderProduct.isInCart = false;
                var existingOrderDetail = _unitOfWork.orderDetail.GetT(od => od.orderid == orderId && od.productid == orderProduct.productid && od.userid == orderProduct.userid);
                if (existingOrderDetail != null)
                {
                    existingOrderDetail.total = totalPrice;
                    _unitOfWork.orderDetail.update(existingOrderDetail);
                }
                else
                {
                    orderDetail.total = totalPrice;
                    _unitOfWork.orderDetail.Add(orderDetail);
                }
            }

            // Update the order status and total price
            UpdateOrderStatus(orderId, "Completed", (int)totalPrice);

            // Save the changes to the database
            _unitOfWork.Save();

            return View("OrderSuccess", orderDetailsViewModel);
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
        //    _unitOfWork.product_order.deleteRange(orderList);
            _unitOfWork.Save();

        }
        public void UpdateOrderStatus(int orderId, string newStatus,int price)
        {
            // Retrieve the order from the database using its ID
            var orderFromDb = _unitOfWork.order.GetT(o => o.orderid == orderId);

            if (orderFromDb != null)
            {
                // Update the status of the order
                orderFromDb.status = newStatus;
                orderFromDb.date_order = DateTime.Now;
                orderFromDb.total = price;

                // Save the changes to the database
                _unitOfWork.order.update(orderFromDb);
                _unitOfWork.Save();
            }
        }
    }
}
