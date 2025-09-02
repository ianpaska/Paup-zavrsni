using GameShop3.Models;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace GameShop3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.TitleText = "Dobrodošli u GameShop3!";
            var top = Store.Games.OrderByDescending(g => g.Popularity).Take(5).ToList();
            return View(top);
        }

        public ActionResult About() { return View(); }
        public ActionResult Contact() { return View(); }
    }
}
