using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySite.Models;
using MySite.CipherData;

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
                    string html = string.Empty;
                    Users user = null;
                    if (email == "admin@mail.ru")//если пользователь - админ
                    {
                        html += "<b>Список пользователей сайта</b><ul>";
                        siteDb.Users.Load();
                        user = siteDb.Users.First(x => x.Email == email);

                        List<Users> usersList = siteDb.Users.Local.ToList();
                        foreach(Users userItem in usersList)
                        {
                            html += "<li>" +
                                        "<i>" + userItem.Email + "&nbsp&nbsp&nbsp&nbsp Имя: " + userItem.Name + "&nbsp;&nbsp;&nbsp;&nbsp; Фамилия: " + userItem.Surname + "</i>" +
                                    "</li>";
                        }
                        html += "</ul>";
                    }else
                    {
                        user = siteDb.Users.First(x => x.Email == email);
                        html += "<b>Список заказов</b><ul>";
                        /*добавить вывод информации о заказах*/
                        html += "</ul>";
                    }
                    ViewBag.Html = html;
                    return View(user);
                    /*
                    List<Result_Test> testsComplete = null;
                    Users user = null;

                    if (email != "admin@mail.ru")
                    {//если пользователь не админ
                        
                        user = siteDb.Users.First(x => x.Email == email);//находим в БД плзователя с указанным в куки Email
                        
                        siteDb.Result_Test.Load();
                        testsComplete = (from item in siteDb.Result_Test
                                                           where item.ID_Users == user.Id//находим все данные о пройденных тестах
                                                           select item).ToList();

                        if (testsComplete.Count != 0)//если пользоватлеь проходил хотяб 1 тест
                        {
                            siteDb.NameTest.Load();
                            foreach (Result_Test item in testsComplete)//пробегаемся по всем пройденным тестам
                            {
                                html += "<li>" +
                                            "<i>" + siteDb.NameTest.First(x => x.Id == item.ID_NameTest).name_test + "&nbsp;&nbsp;&nbsp;&nbsp;</i>" +
                                            "<div class=\"progress\">" +
                                                "<div class=\"progress-bar progress-bar-success progress-bar-striped\" role=\"progressbar\" style=\"width:" + item.Result + "%\"" +
                                                " aria-valuemin=\"0\" aria-valuemax=\"100\"></div>" +
                                            "<div>" +
                                        "</li>";
                            }
                        }
                        else
                        {
                            html += "<h4>Вы не прошли ни одного теста</h4>";
                        }
                        return View(user);
                    }
                    else
                    {
                        siteDb.Result_Test.Load();
                        foreach (Users userItem in siteDb.Users.Local)//пробегаемся по всем пользователям
                        {
                            testsComplete = (from item in siteDb.Result_Test
                                                where item.ID_Users == userItem.Id//находим все данные о пройденных тестах
                                                select item).ToList();

                            if (testsComplete.Count != 0)//если пользоватлеь проходил хотяб 1 тест
                            {
                                siteDb.NameTest.Load();
                                foreach (Result_Test item in testsComplete)//пробегаемся по всем пройденным тестам
                                {
                                    html += "<li>" +
                                                "<i>" + userItem.Email+ "&nbsp&nbsp&nbsp&nbsp" +  siteDb.NameTest.First(x => x.Id == item.ID_NameTest).name_test + "&nbsp;&nbsp;&nbsp;&nbsp;</i>" +
                                                "<div class=\"progress\">" +
                                                    "<div class=\"progress-bar progress-bar-success progress-bar-striped\" role=\"progressbar\" style=\"width:" + item.Result + "%\"" +
                                                    " aria-valuemin=\"0\" aria-valuemax=\"100\"></div>" +
                                                "<div>" +
                                            "</li>";
                                }
                            }
                            else
                            {
                                html += "<li>" +
                                                "<i>" + userItem.Email + "&nbsp;&nbsp;&nbsp;&nbsp; - пользователь ещё не прошел  ни один тест</i>" +  
                                        "</li>";
                            }
                        }
                    }
                    ViewBag.Html = html;
                    return View(user);*/
                }
                else
                {
                    throw new Exception("Доступ к этой странцие имеют только авторизованные пользвоатели");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return View("Error");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit()//возвращаю страницу для изменения данных профиля
        {
            ViewBag.returnUrl = "~/Profile/Edit";

            try
            {
                siteDb.Users.Load();
                string Email = Request.Cookies["User"].Value;
                Users user = siteDb.Users.Local.First(x => x.Email == Email);

                return View(user);//возвращаем данные о пользователя для вставки их в поля ввода
            } catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
           
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Users user)//возвращаю страницу для изменения данных профиля
        {
            try
            {
                if (ModelState.IsValid)//если валидация пройдена успешно
                {
                    siteDb.Users.Load();//загружаем данные из БД

                    string Email = Request.Cookies["User"].Value;

                    Users newUser = siteDb.Users.First(x => x.Email == Email);

                    int index = siteDb.Users.Local.IndexOf(newUser);//находим индекс авторизованного пользователя в коллекции

                    //запоминание отредактированные данные о пользователе
                    siteDb.Users.Local[index].Name =user.Name;
                    siteDb.Users.Local[index].Surname = user.Surname;
                    siteDb.Users.Local[index].AboutOneself = user.AboutOneself;
                    siteDb.Users.Local[index].DateBirthDay = user.DateBirthDay;
                    siteDb.Users.Local[index].PasswordConfirm = siteDb.Users.Local[index].Password;

                    try
                    {
                        siteDb.SaveChanges();//сохраняем измененияы
                        throw new Exception("Изменения успешно сохранены !");
                    }catch(DbEntityValidationException ex)
                    {
                        foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                        {
                            ViewBag.Msg += "Object: " + validationError.Entry.Entity.ToString();
                            foreach (DbValidationError err in validationError.ValidationErrors)
                            {
                                ViewBag.Msg += err.ErrorMessage + "";
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Неверно заполнены поля ввода");
                }
            }
            catch (Exception exc)
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
        
        [HttpGet]
        public ActionResult Delete()//удаление аккаунта
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Delete(string msg)//удаление аккаунта
        {
            try
            {
                var user = Request.Cookies["User"];

                siteDb.Users.Load();//загружаем списко пользователей

                Users delUser = siteDb.Users.First(x => x.Email == user.Value);
                siteDb.Users.Remove(delUser); //удаляем пользователя
                siteDb.SaveChanges();//сохраняем изменения
                Response.Cookies["User"].Expires = DateTime.Now.AddDays(-1);//удаляем данные из куки
            }catch(Exception exc)
            {
                msg = exc.Message;
            }
            finally
            {
                ViewBag.Msg = msg;
            }
   

            return RedirectToAction("Autorization", "LogIn");
        }

        [HttpPost]
        public ActionResult DeleteSelected()//удаление аккаунта с указанным email
        {
            try
            {
                if (Request.Form["email"] != null)
                {
                    string email = Request.Form["email"];//получаем
                    ViewBag.email = email;

                    siteDb.Users.Load();//загружаем списко пользователей

                    Users delUser = siteDb.Users.First(x => x.Email == email);//находим пользователя с указанным email  в БД
                    siteDb.Users.Remove(delUser); //удаляем пользователя
                    siteDb.SaveChanges();//сохраняем изменения
                    return RedirectToAction("Home", "Profile");
                }
                else
                {
                    throw new Exception("Вы не указали email");
                }
            }
            catch (Exception exc)
            {
                ViewBag.msg = exc.Message;
                return RedirectToAction("NotFound", "Error");
            }
        }
    }
}