using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MySite.Models;

namespace MySite.Controllers
{
    public class TestController : Controller
    {
        SiteDbEntities siteDb = new SiteDbEntities();
        //выбранный тест SelectedTest

        // GET: Test
        public ActionResult ListTest()
        {
            return View();
        }

        public ActionResult Result(string SelectedTest)//Выводит результат тестирвоания
        {
            try
            {
                if (Request.Cookies["user"] != null)//если пользователь авторизован на странице
                {
                    switch (SelectedTest)
                    {
                        case "Sharp":
                            {
                                siteDb.Sharp.Load();//загружаем данные из бд с правильными ответами 
                                int maxAnsTruth = siteDb.Sharp.Local.Count();//запоминаем кол-во вопросов
                                int noRightAns = 0;//кол-во неправильных ответов

                                for (int i = 1; i <= maxAnsTruth; i++)//пробегаемся по всем вопросам
                                {
                                    if (Request.Form["quest" + i.ToString()] != siteDb.Sharp.Local.First(x => x.Id == i).numAnswer.ToString())//если пользователь выбрал неправильный ответ
                                    {
                                        noRightAns++;
                                    }
                                }
                                double percentRes = ((double)(maxAnsTruth - noRightAns) / (double)(maxAnsTruth + noRightAns)) * 100;//процентное соотношение  неправильных ответов
                                string email = Request.Cookies["user"].Value;//запоминаем email авторизованного пользователя
                                siteDb.Users.First(x => x.Email == email).Sharp = Convert.ToInt32(percentRes);//запоминаем результат прохождения теста в учетной записи пользователя
                                siteDb.SaveChanges();//сохраняем изменения в БД

                                if (percentRes >= 90)
                                {
                                    ViewBag.Result = "Тест на знание языка C# пройден на оценку - 5";
                                    ViewBag.Message = "Поздравляю ! Вы отлично владеете данным языком и можете похвастаться даже своими проектами. С вашими знаниями " +
                                        "можно устроиться практически в любую компанию со стартовым заработком 50к или даже можете хорошо зарекомендовать себя на фрилансе";
                                }
                                else if (percentRes >= 75 && percentRes < 90)
                                {
                                    ViewBag.Result = "Тест на знание языка C# пройден на оценку - 4";
                                    ViewBag.Message = "Поздравляю ! Вы довольно не плохо владеете данным языком, хотя все же есть пробелы над которыми стоит поработать. С вашими знаниями " +
                                        "можно устроиться максимум на позицию Junior разработчика со стартовым заработком ~30к";
                                }
                                else if (percentRes > 50 && percentRes <= 75)
                                {
                                    ViewBag.Result = "Тест на знание языка C# пройден на оценку - 3";
                                    ViewBag.Message = "Поздравляю ! Вы прошли тест, но увы результаты оставляют желать лучшего. С вашими знаниями " +
                                        "будет довольно сложно устроиться даже на позицию Junior разработчика. Для улучшения результата стоит подтянуть свои знания языка прочитав по ней литературу";
                                }
                                else
                                {
                                    ViewBag.Result = "Тест на знание языка C# пройден на оценку - 2";
                                    ViewBag.Message = "Увы...вы не прошли тест. Это говорит о том, что вы либо не знаете язык совсем, либо вы изучали его не правильно. Рекомендуем вам после " +
                                        "изучения каждой темы по языку закреплять материал на практике.";
                                }
                                break;
                            }
                        default:
                            break;
                    }
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
            return View();
        }

       
        public ActionResult Select_SharpTest()//при выборе теста по C#
        {
            try
            {
                if (Request.Cookies["user"] != null)//если пользователь авторизован на странице
                {
                    return View();
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
    }
}