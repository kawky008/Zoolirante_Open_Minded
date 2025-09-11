using Microsoft.AspNetCore.Mvc;

namespace Zoolirante_Open_Minded.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
