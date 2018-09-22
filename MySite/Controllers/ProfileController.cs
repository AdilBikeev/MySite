using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySite.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ViewResult Home()//возвращает домашнюю страницу профиля
        {
            return View();
        }

        public ActionResult Exit()//выход из профиля
        {
            Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);//удаляем данные из куки
            return RedirectToAction("Autorization", "LogIn");
        }
    }
}