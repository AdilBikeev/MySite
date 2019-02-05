using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MySite.Models;

namespace MySite.Controllers
{
    public class OrderController : Controller
    {
        SiteDbEntities siteDb = new SiteDbEntities();
        
        // GET: Order
        public ActionResult Create()//возвращает странциу создания заказа
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ViewResult SendMsg()
        {
            try
            {
                if (Regex.IsMatch(Request.Form["linkVK"].ToString(), @"^(https?:\/\/)?(www\.)?vk\.com\/(\w|\d)+?\/?$"))//если ссыфлка вк валидна
                {
                    if (Regex.IsMatch(Request.Form["subject"].ToString(), @"^[а-яА-ЯёЁa-zA-Z0-9]+$"))//валидация subject
                    {
                        if (Regex.IsMatch(Request.Form["requirements"].ToString(), @"^[a-zA-ZА-Яа-яЁё0-9\s]+$"))//валидация требований заказа
                        {
                            string subject = Request.Form["subject"];
                            string email = Request.Cookies["User"].Value;
                            string requirements = Request.Form["requirements"];
                            string time = Request.Form["time"];
                            int ID_NameOrder = 0;

                            siteDb.Name_Order.Load();
                            if(siteDb.Name_Order.Count() == 0 || siteDb.Name_Order.FirstOrDefault(x => x.Name == subject) == null)//если предмета с данным названием нет в базе
                            {
                                siteDb.Name_Order.Add(new Name_Order
                                {
                                    Name = subject
                                });
                            }
                            ID_NameOrder = siteDb.Name_Order.First(x => x.Name == subject).Id;

                            siteDb.Orders.Load();
                            siteDb.Users.Load();
                            siteDb.Orders.Add(new Orders
                            {
                                ID_Name = ID_NameOrder,
                                ID_User = siteDb.Users.First(x => x.Email == email).Id,
                                About_Order = requirements,
                                Workman = "Nobody",
                                Salary_full = 999999,
                                Salary_workman = 99999,
                                Status = "Неактивен",
                                Time = time
                            });
                            siteDb.SaveChanges();

                            ViewBag.Msg = "Заказ успешно оформлен. Вам в скором времени отпишут либо на странциу в ВК, либо на вашу почту, указанную при регистрации";
                            return View("Thanks");
                        }
                        else
                        {
                            throw new Exception("Поле \"Требования к заказу\" должно содержать только цифры/буквы и некоторые символы");
                        }
                    }
                    else
                    {
                        throw new Exception("Неверный формат subject");
                    }
                }
                else
                {
                    throw new Exception("Неверный формат ссылки VK");
                }
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return View("Create");
            }
            /*
                try
                {
                    
                    //запоминаем все данные введенные администартором
                    string[] answerChoice = Request.Form["AnswerChoice"].Split(',');
                    string question = Request.Form["quest"];
                    string TrueAnswer = Request.Form["trueAnswer"];

                    //загружаем данные из бд
                    siteDb.Questions.Load();
                    siteDb.Answer_Choice.Load();

                    //добавляем новый вопрос
                    siteDb.Questions.Add(new Questions
                    {
                        ID_NameTest = id,
                        question = question,
                        TrueAnswer = int.Parse(TrueAnswer.Split(' ')[0])
                    });

                    //сохраняем изменения
                    siteDb.SaveChanges();//ошибка

                    int[] Id = (from item in siteDb.Questions.Local
                                where item != null
                                select item.Id).ToArray();

                    //добавляем варианты ответа для вопроса
                    foreach (string answer in answerChoice)
                    {
                        siteDb.Answer_Choice.Add(new Answer_Choice
                        {
                            answer = answer,
                            ID_Question = Id.Max()//возвращаем последний id вопроса
                        });
                    }

                    //сохраняем изменения
                    siteDb.SaveChanges();
                }
                catch (Exception exc)
                {
                    HttpCookie cookie = new HttpCookie("ERROR");//в качестве cooki запоминаем сообщение об ошибке
                    cookie.Expires = DateTime.Now.AddDays(1);
                    cookie.Value = exc.Message;
                    Response.Cookies.Add(cookie);

                    return RedirectPermanent("~/Error/Forbidden/");
                }
            return RedirectPermanent("~/Test/EditTest/" + id);*/
        }

        [HttpGet]
        public ViewResult ListOrder()
        {
            string html = string.Empty;
            try
            {
               

                siteDb.Orders.Load();
                siteDb.Name_Order.Load();
                siteDb.Users.Load();
                foreach(Orders orders in siteDb.Orders.Local)//пробегаемяспо всем заказам
                {
                    html += "" +
                        "<tr>" +
                            "<td>" + orders.Id + "</td>" +
                            "<td>" + siteDb.Name_Order.First(x => x.Id == orders.ID_Name).Name + "</td>" +
                            "<td>" + orders.Salary_full + "</td>" +
                            "<td>" + orders.Salary_workman + "</td>" +
                            "<td>" + siteDb.Users.First(x => x.Id == orders.ID_User).Email + "</td>" +
                            "<td>" + orders.Workman + "</td>" +
                            "<td>" + orders.Time + "</td>" +
                            "<td>" + orders.Status + "</td>" +
                            "<td>" +
                                "<a class = \"btn btn-success\">Назначить заказ</a>" +
                                "<br/>" +
                            "</td>" +
                        "</tr>";
                }
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return View();
            }
            ViewBag.html = html;
            return View();
        }
    }
}