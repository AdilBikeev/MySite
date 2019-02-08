using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MySite.Models;
using MySite.WebService;

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
        [ValidateInput(false)]//ОПТИМИЗИРОВАТЬ
        public async Task<ViewResult> SendMsg()
        {
            try
            {
                string code = Request.Form["captcha"];
                if (code != String.Empty && code == Session["code"].ToString())
                {//если каптчка введена верно
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
                                string linkVK = Request.Form["linkVK"];
                                int ID_NameOrder = 0;

                                siteDb.Name_Order.Load();
                                if (siteDb.Name_Order.Count() == 0 || siteDb.Name_Order.FirstOrDefault(x => x.Name == subject) == null)//если предмета с данным названием нет в базе
                                {
                                    siteDb.Name_Order.Add(new Name_Order
                                    {
                                        Name = subject
                                    });
                                    ID_NameOrder = siteDb.Name_Order.Local[siteDb.Name_Order.Local.Count - 1].Id + 1;
                                }
                                else ID_NameOrder = siteDb.Name_Order.First(x => x.Name == subject).Id;

                                siteDb.Orders.Load();
                                siteDb.Users.Load();
                                siteDb.Orders.Add(new Orders
                                {
                                    ID_Name = ID_NameOrder,
                                    ID_User = siteDb.Users.First(x => x.Email == email).Id,
                                    About_Order = requirements + ". Ссылка на страницу в ВК: " + linkVK,
                                    Workman = "Nobody",
                                    Salary_full = 999999,
                                    Salary_workman = 99999,
                                    Status = "Неактивен",
                                    Time = time
                                });
                                siteDb.SaveChanges();

                                EmailService emailService = new EmailService();
                                await emailService.SendEmailAsync("itlearning2020@gmail.com", "Новый заказ на сайте it-learning", "На сайте появился новыый заказ<br/>" +
                                    "Перейдите по <a href = \"http://it-learning.somee.com/Order/ListOrder\">ссылке</a> чтобы узнать о заказе больше");

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
                }
                else
                {
                    throw new Exception("Код с картинки введён неверно");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return View("Create");
            }
        }

        [HttpGet]//ОПТИМИЗИРОВАТЬ
        public ViewResult GetListOrders()
        {
            string html = string.Empty;
            try
            {
                if(Request.Cookies["user"] != null && Request.Cookies["__sc1_592658302759876"] != null)
                {
                    
                    string email = Request.Cookies["User"].Value;
                    string token = Request.Cookies["__sc1_592658302759876"].Value;
                    siteDb.Orders.Load();
                    siteDb.Name_Order.Load();
                    siteDb.Users.Load();
                    if (siteDb.Orders.Local.Count != 0)
                    {
                        List<Orders> ordersList = siteDb.Orders.Local.ToList();
                        if (email == "admin@mail.ru" && VerificationService.isAdmin(token))//если пользователь  админ
                        {
                                foreach (Orders orders in ordersList)//пробегаемяспо всем заказам
                                {
                                    html += "" +
                                        "<tr>" +
                                            "<td>" + orders.Id + "</td>" +
                                            "<td>" + siteDb.Name_Order.First(x => x.Id == orders.ID_Name).Name + "</td>" +
                                            "<td>" + orders.Salary_full + "</td>" +
                                            "<td>" + orders.Salary_workman + "</td>";
                                if (siteDb.Users.FirstOrDefault(x => x.Id == orders.ID_User) != null)
                                {
                                    html += "<td>" + siteDb.Users.First(x => x.Id == orders.ID_User).Email + "</td>";
                                }
                                else
                                {
                                    html += "<td>Пользователь удалил аккаунт</td>";
                                }
                                    html += "" +
                                            "<td>" + orders.Workman + "</td>" +
                                            "<td>" + orders.Time + "</td>" +
                                            "<td>" + orders.Status + "</td>" +
                                            "<td>" +
                                                "<a href=\"/Order/AppointOrder/" + orders.Id + "\"class = \"btn btn-success\">Изменить данные</a>" +
                                                "<br/>" +
                                                "<a href=\"/Order/InfoOrder/" + orders.Id + "\"class = \"btn btn-info\">Подробнее</a>" +
                                                "<br/>" +
                                                "<a href=\"/Order/CLoseOrder/" + orders.Id + "\"class = \"btn btn-danger\">Закрыть заказ</a>" +
                                                "<br/>" +
                                            "</td>" +
                                        "</tr>";
                                }
                        }
                        else
                        {//если пользователь - рабочий
                            foreach (Orders orders in ordersList)//пробегаемяспо всем заказам
                            {
                                html += "" +
                                    "<tr>" +
                                        "<td>" + orders.Id + "</td>" +
                                        "<td>" + siteDb.Name_Order.First(x => x.Id == orders.ID_Name).Name + "</td>" +
                                        "<td>Secret</td>" +
                                        "<td>" + orders.Salary_workman + "</td>" +
                                        "<td>Secret</td>" +
                                        "<td>" + orders.Workman + "</td>" +
                                        "<td>" + orders.Time + "</td>" +
                                        "<td>" + orders.Status + "</td>" +
                                        "<td>" +
                                            "<a href=\"/Order/TakeTheOrder/" + orders.Id + "\"class = \"btn btn-success\">Взяться за заказ</a>" +
                                            "<br/>" +
                                            "<a href=\"/Order/InfoOrder/" + orders.Id + "\"class = \"btn btn-info\">Подробнее</a>" +
                                            "<br/>" +
                                        "</td>" +
                                    "</tr>";
                            }
                        }

                    }
                    else throw new Exception("Извините, но заказов ещё нет");
                }
                else
                {
                    throw new Exception("Информация о заказах доступно только авторизованным пользевателям");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            
            ViewBag.html = html;
            return View();
        }

        [HttpGet]
        public ActionResult TakeTheOrder(int ID)//возвращает форму для подачи заявки на оформление работы на рабочего
        {
            Orders orders = null;
            try
            {
                if (Request.Cookies["user"] != null && Request.Cookies["__sc1_592658302759876"] != null)
                {
                    siteDb.Orders.Load();
                    orders = siteDb.Orders.First(x => x.Id == ID);
                }
                else
                {
                    throw new Exception("");
                }
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/Shared/_LayoutError");
            }
            return View(orders);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]//ОПТИМИЗИРОВАТЬ
        public async Task<ActionResult> TakeTheOrder()
        {
            string ID_Order = Request.Form["Id"];
            string msgWorkman = Request.Form["msg"];
            string email = Request.Cookies["User"].Value;
            try
            {
                if(msgWorkman != string.Empty)//если сообщение от рабочего в заявке не пустое
                {
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(email, "Заявка на получение заказа на сайте it-learning", "Ваша заявка " +
                        "<br/>\'ID заказа: " + ID_Order + "\'" +
                        "<br/>\'Ваше сообщение: " + msgWorkman + "\'" +
                        "<br/>\'Ваш email: " + email + "\'" +
                        " успешно отправлена администраици сайта ! В скором времени вам отпишут и сообщат результат обрабокти заявки");
                }
                else
                {
                    throw new Exception("*Поле сообщение не было введено в заявке, поэтому заявка была отколена автоматически");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            return RedirectToAction("ListOrder");
        }

        public ActionResult CLoseOrder(int ID)
        {
            try
            {
                if (Request.Cookies["user"] != null && Request.Cookies["__sc1_592658302759876"] != null)
                {
                    siteDb.Orders.Load();
                    siteDb.Orders.Remove(siteDb.Orders.First(x => x.Id == ID));
                    siteDb.SaveChanges();
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/Shared/_LayoutError");
            }
            return View("ListOrder");
        }

        [HttpGet]
        public ActionResult InfoOrder(int ID)//возвращает информацию о заказе
        {
            try
            {
                if (Request.Cookies["user"] != null && Request.Cookies["__sc1_592658302759876"] != null)
                {
                    siteDb.Orders.Load();
                    string info = siteDb.Orders.First(x => x.Id == ID).About_Order;
                    string email = Request.Cookies["User"].Value;
                    string token = Request.Cookies["__sc1_592658302759876"].Value;
                    if( (email != "admin@mail.ru" && siteDb.Orders.First(x => x.Id == ID).Workman != email) || (email == "admin@mail.ru" && (!VerificationService.isAdmin(token))) )//если пользователь не имеет отношения к заказу
                    {
                        int index = info.IndexOf("Ссылка на страницу в ВК");
                        if(index != -1)
                        {
                            info = info.Remove(info.IndexOf("Ссылка на страницу в ВК"));
                        }
                    }
                    ViewBag.Info = info;
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/Shared/_LayoutError");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AppointOrder(int ID)
        {
            Orders orders = null;
            try
            {
                if (Request.Cookies["user"] != null && Request.Cookies["__sc1_592658302759876"] != null)
                {
                    siteDb.Orders.Load();
                    orders = siteDb.Orders.First(x => x.Id == ID);
                }
                else
                {
                    throw new Exception("");
                }
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/Shared/_LayoutError");
            }
            return View(orders);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]//ОПТИМИЗИРОВАТЬ
        public async Task<ActionResult> AppointOrder(Orders orders)
        {
            string info = orders.About_Order;
            int index = info.IndexOf("Ссылка на страницу в ВК");
            info = info.Remove(0, index);
            try
            {
                siteDb.Orders.Load();
                siteDb.Orders.First(x => x.Id == orders.Id).Salary_full = orders.Salary_full;
                siteDb.Orders.First(x => x.Id == orders.Id).Salary_workman = orders.Salary_workman;
                siteDb.Orders.First(x => x.Id == orders.Id).Workman = orders.Workman;
                siteDb.Orders.First(x => x.Id == orders.Id).Status = "Активен";

                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(orders.Workman, "Заявка на получение заказа на сайте it-learning", "Ваша заявка одобрена" +
                    "<br/>\'ID заказа: " + orders.Id + "\'" +
                    "<br/>\'Ваша ЗП за выполненный заказ: " + orders.Salary_workman + "\'" +
                    "<br/>\'Ваш email: " + orders.Workman + "\'" +
                    "<br/>\'" + info + "\'" +
                    "<br/> Если этой информации было вам недостаточно, то вы всегда можете посетить страницу <a href = \"http://it-learning.somee.com/Order/ListOrder\">список заказов</a> и узнать подробности заказа." +
                    "или же вы можете написать нам на почту itlearning2020@gmail.com . " +
                    "<br/><b>Обязательно</b> после выполнения заказа - сообщить об этом админисрацию ! При невыполнении условий договорра - ваше взаимодействие с нами будет прекращено навсегда.");

                siteDb.SaveChanges();
            }
            catch (Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            ViewBag.Msg = "Назначение заказа пройдено успешно";
            return View("Thanks");
        }
    }
}