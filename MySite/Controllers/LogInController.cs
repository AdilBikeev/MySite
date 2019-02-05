using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySite.Models;
using MySite.CipherData;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MySite.Controllers
{
    public class LogInController : Controller
    {
        SiteDbEntities db = new SiteDbEntities();
        [HttpGet]
        public ActionResult Autorization()//при первом вызове страницы
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Autorization(Users user)
        {
            try
            {
                string code = Request.Form["captcha"];
                if (code != String.Empty && code == Session["code"].ToString())
                {//если каптчка введена верно
                    db.Users.Load();
                    foreach (Users item in db.Users.Local)
                    {
                        if (item.Password == Cipher.GetMD5Hach(user.Password) && item.Login == Cipher.GetMD5Hach(user.Login))//расшифровываем даныне из БД и сверяем данные с веденными пользоватеем
                        {
                            ViewBag.Msg = "";
                            HttpCookie cookie = new HttpCookie("User");//в качестве cooki запоминаем эмаил пользователя
                            cookie.Expires = DateTime.Now.AddDays(31);
                            cookie.Value = item.Email;//сохраняем в куки уже зашифрованный email
                            Response.Cookies.Add(cookie);
                            return RedirectToAction("Index", "Home");//возвращаемся к домашней странцие
                        }
                    }
                    throw new Exception("Неверный логин или пароль");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            return View();
        }
    }
}