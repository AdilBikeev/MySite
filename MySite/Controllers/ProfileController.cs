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

        [HttpGet]
        public ActionResult Edit()//возвращаю страницу для изменения данных профиля
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(Users user)//возвращаю страницу для изменения данных профиля
        {
            try
            {
                if(ModelState.IsValid)//если валидация пройдена успешно
                {
                    siteDb.Users.Load();//загружаем данные из БД

                    string Email = Request.Cookies["User"].Value;

                    Users newUser = siteDb.Users.First(x => x.Email == Email);

                    int index = siteDb.Users.Local.IndexOf(newUser);//находим индекс авторизованного пользователя в коллекции

                    //запоминание отредактированные данные о пользователе
                    siteDb.Users.Local[index].Name = user.Name;
                    siteDb.Users.Local[index].Surname = user.Surname;
                    siteDb.Users.Local[index].AboutOneself = user.AboutOneself;
                    siteDb.Users.Local[index].DateBirthDay = user.DateBirthDay;

                    siteDb.SaveChanges();//сохраняем измененияы
                    throw new Exception("Изменения успешно сохранены !");
                }
                else
                {
                    throw new Exception("Неверно заполнены поля ввода");
                }
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            return View();
        }

        public ActionResult Exit()//выход из профиля
        {
            Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);//удаляем данные из куки
            return RedirectToAction("Autorization", "LogIn");
        }
    }
}