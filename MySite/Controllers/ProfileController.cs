using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySite.Models;

namespace MySite.Controllers
{
    public class ProfileController : Controller
    {
        SiteDbEntities siteDb = new SiteDbEntities();

        // GET: Profile
        public ViewResult Home()//возвращает домашнюю страницу профиля
        {
            try
            {
                if (Request.Cookies["user"] != null)//если пользователь авторизован на странице
                {
                    siteDb.Users.Load();//загрыжаем данные о пользователях из БД
                    string email = Request.Cookies["user"].Value;
                    Users user = siteDb.Users.First(x => x.Email == email);//находим в БД плзователя с указанным в куки Email
                    return View(user);
                }
                else
                {
                    throw new Exception("Доступ к этой странцие имеют только авторизованные пользвоатели");
                }
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return View("Error");
            }
        }

        public ActionResult Exit()//выход из профиля
        {
            Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);//удаляем данные из куки
            return RedirectToAction("Autorization", "LogIn");
        }
    }
}