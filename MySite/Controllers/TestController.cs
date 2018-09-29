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
            switch (SelectedTest)
            {
                case "Sharp":
                {
                        siteDb.Sharp.Load();//загружаем данные из бд с правильными ответами 
                        int maxAnsTruth = siteDb.Sharp.Local.Count();//запоминаем кол-во вопросов
                        int rightAns = 0;//кол-во правильных ответов

                        for(int i = 1; i <= maxAnsTruth; i++)//пробегаемся по всем вопросам
                        {
                            if(Request.Form["quest" + i.ToString()] == siteDb.Sharp.Local.First(x => x.Id == i).numAnswer.ToString())//если пользователь выбрал правильный ответ
                            {
                                rightAns++;
                            }
                        }
                        double percentRes = ((maxAnsTruth - rightAns) / (maxAnsTruth + rightAns)) * 100;//процентное соотношение  неправильных ответов
                        if (percentRes <=10)
                        {
                            ViewBag.Result = "Тест на знание языка C# пройден на оценку - 5";
                            ViewBag.Message = "Поздравляю ! Вы отлично владеете данным языком и можете похвастаться даже своими проектами. С вашими знаниями " +
                                "можно устроиться практически в любую компанию со стартовым заработком 50к или даже можете хорошо зарекомендовать себя на фрилансе";
                        }else if(percentRes > 10 && percentRes <= 30)
                        {
                            ViewBag.Result = "Тест на знание языка C# пройден на оценку - 4";
                            ViewBag.Message = "Поздравляю ! Вы довольно не плохо владеете данным языком, хотя все же есть пробелы над которыми стоит поработать. С вашими знаниями " +
                                "можно устроиться максимум на позицию Junior разработчика со стартовым заработком ~30к";
                        }else if (percentRes > 30 && percentRes < 50)
                        {
                            ViewBag.Result = "Тест на знание языка C# пройден на оценку - 3";
                            ViewBag.Message = "Поздравляю ! Вы прошли тест, но увы результаты оставляют желать лучшего. С вашими знаниями " +
                                "будет довольно сложно устроиться даже на позицию Junior разработчика. Для улучшения результата стоит подтянуть свои знания языка прочитав по ней литературу";
                        }else  
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
            return View();
        }

       
        public ActionResult Select_SharpTeest()//при выборе теста по C#
        {
            return View();
        }
    }
}