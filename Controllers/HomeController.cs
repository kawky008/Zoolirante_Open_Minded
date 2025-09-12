using Microsoft.AspNetCore.Mvc;

namespace Zoolirante_Open_Minded.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			ViewData["BannerText"] = "Welcome to Zoolirante";
			return View();
		}
	}
}
