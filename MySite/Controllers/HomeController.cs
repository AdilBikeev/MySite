using System;
using System.Data.Entity;
using System.Web.Mvc;
using MySite.Models;
using MySite.CipherData;
using CaptchaProject.Util;
using System.Drawing.Imaging;

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

        public ActionResult Captcha()//Создает капчу, для проверки на робота
        {
            string code = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();
            Session["code"] = code;//формируем код и запоминаем его в сессии
            CaptchaImage captcha = new CaptchaImage(code, 110, 50);//создаем капчу с заданными размерами

            Response.Clear();
            Response.ContentType = "image/jpeg";

            captcha.Image.Save(Response.OutputStream, ImageFormat.Jpeg);//сохраняем картинку в формате jpeg

            captcha.Dispose();
            return null;
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
                    string code = Request.Form["captcha"];
                    if (code != String.Empty && code == Session["code"].ToString()){//если каптчка введена верно
                    
                        dbSite.Users.Load();
                        foreach (Users item in dbSite.Users.Local)
                        {
                            if (item.Login == Cipher.GetMD5Hach(user.Login))
                            {
                                throw new Exception("Пользователь с таким логином уже существует !");
                            }
                            else if (item.Email == user.Email)
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