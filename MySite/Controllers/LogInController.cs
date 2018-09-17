using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySite.Models;
using System.Data.Entity;

namespace MySite.Controllers
{
    public class LogInController : Controller
    {

        SiteDbEntities db= new SiteDbEntities();
        [HttpGet]
        public ActionResult Autorization()//при первом вызове страницы
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autorization(Users user)
        {
            try
            {
                db.Users.Load();
                foreach(Users item in db.Users.Local)
                {
                    if(item.Login== user.Login && item.Password == user.Password)
                    {
                        ViewBag.Msg = "";
                        HttpCookie cookie = new HttpCookie("User");//в качестве cooki запоминаем эмаил пользователя
                        //{
                        //    Expires = DateTime.Now.AddDays(-1),
                        //    Value = null
                        //};
                        cookie.Expires = DateTime.Now.AddDays(31);
                        cookie.Value = item.Email;
                        Response.Cookies.Add(cookie);
                        return RedirectToAction("Index","Home");//возвращаемся к домашней странцие
                    }
                }
                throw new Exception("Неверный логин или пароль");
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            return View();
        }
    }
}