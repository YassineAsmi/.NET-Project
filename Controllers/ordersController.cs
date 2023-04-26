using Microsoft.AspNetCore.Mvc;
using WebProject.Data;

namespace WebProject.Controllers
{
	public class ordersController : Controller
	{
		private readonly AppDbContext appDbContext;
		public ordersController(AppDbContext context)
		{
			appDbContext = context;
		}
		public IActionResult Index()
		{
			var data = appDbContext.products.ToList();
			return View(data);
		}
	}
}
