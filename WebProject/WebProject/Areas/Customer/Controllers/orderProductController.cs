
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebProject.Models;
using WebProject.Repositories;
using Microsoft.Extensions.Hosting;


using System.Drawing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class orderProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CartVM vm { get; set; }
        private readonly IWebHostEnvironment _webHostEnvironment;

        public orderProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

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

            // Get the product and its current stock
            var product = _unitOfWork.product.GetT(p => p.productid == productId);
            var currentStock = product.quantity;
            Debug.Print("product quantity: " + quantity);

            // Check if the requested quantity exceeds the available stock
            if (quantity > currentStock)
            {
                // Handle the out-of-stock scenario
                TempData["ErrorMessage"] = "Sorry, the requested quantity exceeds the available stock.";
                return RedirectToAction("Index", "Home");
            }
            // Check if the product is already in the cart
            var existingOrderProduct = _unitOfWork.product_order.GetT(x => x.orderid == existingOrder.orderid && x.productid == productId);

            if (existingOrderProduct == null)
            {

                // If the product is not in the cart, create a new order product
                var orderProduct = new order_product
                {
                    userid = claim.Value,
                    productid = productId,
                    product = product,
                    quantity = Math.Min(quantity, currentStock), // Limit the quantity to the current stock
                    order = existingOrder,
                    isInCart = true
                };
                _unitOfWork.product_order.Add(orderProduct);

                // Add the order detail
                var user = _unitOfWork.user.GetT(x => x.Id == claim.Value);
                var orderDetail = new orderDetail
                {
                    orderid = existingOrder.orderid,
                    productid = productId,
                    Name = user.first_name + " " + user.last_name,
                    PhoneNumber = user.phone_number,
                    Address = user.Address,
                    price = product.price,
                    quantity = quantity,
                    userid = claim.Value
                };
                _unitOfWork.orderDetail.Add(orderDetail);
            }
            else
            {
                // If the product is already in the cart, update the quantity
                existingOrderProduct.quantity = Math.Min(existingOrderProduct.quantity + quantity, currentStock); // Limit the quantity to the current stock
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
                
                var product = _unitOfWork.product.GetT(u => u.productid == productid);

                if (product != null && product.quantity > orderProduct.quantity)
                {
                    orderProduct.quantity++;
                    _unitOfWork.Save();
                }
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
            var orderDet = _unitOfWork.orderDetail.GetT(o => o.orderid == orderId);

            var orderList = _unitOfWork.product_order.GetAll(u => u.userid == claims.Value && u.orderid == orderId, includeProperties: "product,users");

            // Create an instance of OrderDetailsViewModel
            var orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderDetail = orderDet,
                // Assign the list of ordered products
                OrderedProducts = orderList.ToList()
            };

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

            // Calculate the total price based on the updated product_order records
            double totalPrice = orderList.Sum(item => item.product.price * item.quantity);

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
                    userid = orderProduct.userid,
                    total = totalPrice
                };

                orderProduct.isInCart = false;
                var existingOrderDetail = _unitOfWork.orderDetail.GetT(od => od.orderid == orderId && od.productid == orderProduct.productid && od.userid == orderProduct.userid);
                if (existingOrderDetail != null)
                {
                    _unitOfWork.orderDetail.update(existingOrderDetail);
                }
                else
                {
                    _unitOfWork.orderDetail.Add(orderDetail);
                }
            }

            // Update the order status and total price
            UpdateOrderStatus(orderId, "Completed", (int)totalPrice);
            GenerateInvoice(orderId);
            // Save the changes to the database
            _unitOfWork.Save();

            return View("OrderSuccess", orderDetailsViewModel);
        }

        public void productOperation(int orderId)
        {
            var orderList = _unitOfWork.product_order.GetAll(
                o => o.orderid == orderId,
                includeProperties: "product"
            );

            // Decrement the stock quantity for each product in the order
            foreach (var orderProduct in orderList)
            {
                // Retrieve the product from the database
                var product = _unitOfWork.product.GetT(
                    p => p.productid == orderProduct.productid
                );

                // Update the stock quantity
                product.quantity -= orderProduct.quantity;

                // Save the updated product to the database
                _unitOfWork.product.Update(product);
            }

            // Save the changes to the database
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

        public IActionResult GenerateInvoice(int orderId)
        {
            // Retrieve the order from the database
            var order = _unitOfWork.order.GetT(o => o.orderid == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Create a new MigraDoc document
            var document = new Document();

            // Add invoice details to the document
            var section = document.AddSection();
            var paragraph = section.AddParagraph();
            paragraph.AddFormattedText($"Invoice #{order.orderid}", TextFormat.Bold);
            paragraph = section.AddParagraph();
            paragraph.AddFormattedText($"Date: {order.date_order.Value.ToString("dd/MM/yyyy")}", TextFormat.Bold);
            paragraph = section.AddParagraph();
            paragraph.AddFormattedText($"Customer Name: {order.users.first_name} {order.users.last_name}", TextFormat.Bold);
            paragraph = section.AddParagraph();
            paragraph.AddFormattedText($"Customer Email: {order.users.Email}", TextFormat.Bold);
            paragraph = section.AddParagraph();

            // Add order details to the document
            var table = section.AddTable();
            table.Borders.Width = 0.75;
            table.Format.SpaceBefore = "1cm";
            table.AddColumn("4cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");
            table.AddColumn("3cm");
            var headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;
            headerRow.Cells[0].AddParagraph("Product");
            headerRow.Cells[1].AddParagraph("Price");
            headerRow.Cells[2].AddParagraph("Quantity");
            headerRow.Cells[3].AddParagraph("Total");

            foreach (var orderDetail in order.order_products)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(orderDetail.product.name);
                row.Cells[1].AddParagraph($"{orderDetail.product.price:F2} Dt");
                row.Cells[2].AddParagraph(orderDetail.quantity.ToString());
                row.Cells[3].AddParagraph($"{(orderDetail.quantity * orderDetail.product.price):F2} Dt");
            }

            // Calculate the total price
            var totalPrice = order.order_products.Sum(od => od.quantity * od.product.price);

            // Add the total price to the document
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.AddFormattedText($"Total Price: {totalPrice:F2} Dt", TextFormat.Bold);

            // Render the document to a PDF file
            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();

            // Return the PDF file as a MemoryStream
            MemoryStream stream = new MemoryStream();
            renderer.PdfDocument.Save(stream);
            byte[] pdfBytes = stream.ToArray();

            // Save the PDF file to disk
            var pdfFilename = $"Invoice_{order.orderid}_{order.date_order:yyyyMMdd}.pdf";
            var pdfPath = Path.Combine(_webHostEnvironment.WebRootPath, "invoices", pdfFilename);
            System.IO.File.WriteAllBytes(pdfPath, pdfBytes);


            // Return the PDF file as a MemoryStream
            var memoryStream = new MemoryStream(pdfBytes);
            return new FileStreamResult(memoryStream, "application/pdf")
            {
                FileDownloadName = pdfFilename
            };

        }

        public IActionResult DownloadInvoice(int orderId)
        {
            // Get the path of the PDF file
            string pdfPath = Path.Combine(_webHostEnvironment.WebRootPath, "invoices", $"Invoice_{orderId}_{DateTime.Now:yyyyMMdd}.pdf");
            var order = _unitOfWork.order.GetT(o => o.orderid == orderId);

            // Read the PDF file from the disk
            var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var pdfFilename = $"Invoice_{order.orderid}_{order.date_order:yyyyMMdd}.pdf";

            // Return the PDF file as a FileStreamResult with the appropriate content type and download name
            return new FileStreamResult(fileStream, "application/pdf")
            {
                FileDownloadName = pdfFilename
            };
        }



    }


}

