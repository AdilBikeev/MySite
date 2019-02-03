using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using MySite.Models;
using MySite.CipherData;
using System.Text;

namespace MySite.Controllers
{
   

    public class HomeController : Controller
    {
        SiteDbEntities dbSite =new SiteDbEntities();

        public ActionResult Index()//возвращает домашнюю страницу
        {
            return View();
        }

        public ActionResult About()//Возращается страница содержащая информацию о приложении
        {
            return View();
        }

        [HttpGet]
        public ActionResult Registration()//возвращает  страницу регистрации пользователя
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ViewResult Registration(Users user)//Возвращает сообщение о результате обработки информации
        {
            try
            {
                if(ModelState.IsValid)
                {
                    dbSite.Users.Load();
                    foreach (Users item in dbSite.Users.Local)
                    {
                        if (item.Login == Cipher.GetMD5Hach(user.Login))
                        {
                            throw new Exception("Пользователь с таким логином уже существует !");
                        }
                        else if (item.Email== user.Email)
                        {
                            throw new Exception("Пользователь с таким Email уже существует !");
                        }
                    }

                    #region Шифруем данные
                    user.Login = Cipher.GetMD5Hach(user.Login);
                    user.Password = Cipher.GetMD5Hach(user.Password);
                    user.PasswordConfirm = user.Password;
                    user.Email = user.Email;
                    #endregion
                    dbSite.Users.Add(user);//добавляем новго пользователя в БД
                    dbSite.SaveChanges();//слхраняем изменения
                    return View("Thanks", (object)"Регистрация пройдена успешно !");
                }
                ViewBag.Msg = "Неверно заполнены поля ввода";
                return View();
            }catch(Exception exc)
            {
                return View("Thanks", (object)exc.Message);
            }
        }
    }
}