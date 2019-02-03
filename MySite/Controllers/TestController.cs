using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MySite.Models;
using MySite.CipherData;

namespace MySite.Controllers
{
    public class TestController : Controller
    {
        SiteDbEntities siteDb = new SiteDbEntities();

        // GET: Test
        public RedirectResult DeleteQuestion(int id)// id - id вопроса
        {
            int? ID_NameTest=0;
            try
            {
                siteDb.Questions.Load();
                siteDb.Answer_Choice.Load();

                //находим вопрос и его варианты ответа в базе данных
                Questions deleteQuest = siteDb.Questions.First(x => x.Id == id);
                Answer_Choice deleteAnswer = siteDb.Answer_Choice.First(x => x.ID_Question == id);

                ID_NameTest = deleteQuest.ID_NameTest;

                //удаляем всю информацию о вопросе
                siteDb.Questions.Local.Remove(deleteQuest);
                siteDb.Answer_Choice.Local.Remove(deleteAnswer);

                //сохраняем изменения
                siteDb.SaveChanges();
                
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/Error/NotFound/");
            }
            return RedirectPermanent("~/Test/EditTest/" + ID_NameTest);
        }

        [HttpGet]
        public ActionResult AddQuestion(int? id)//id - id названия теста
        {
            if (id == null) return RedirectToAction("NotFound", "Error");
            ViewBag.Id = id; 
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public RedirectResult AddQuestion(int id)//id - id названия теста
        {
            if (ModelState.IsValid)
            {
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
                    siteDb.SaveChanges();

                    int[] Id = (from item in siteDb.Questions.Local
                                where item != null
                                select item.Id).ToArray();

                    //добавляем варианты ответа для вопроса
                    foreach (string answer in answerChoice)
                    {
                        siteDb.Answer_Choice.Add(new Answer_Choice
                        {
                            answer = answer,/*ERROR*/
                            ID_Question = Id.Max()//возвращаем последний id вопроса
                        });
                    }

                    //сохраняем изменения
                    siteDb.SaveChanges();
                }
                catch (Exception exc)
                {
                    ViewBag.Msg = exc.Message;
                    return RedirectPermanent("~/Error/Forbidden/");
                }
            }
            return RedirectPermanent("~/Test/EditTest/" + id);
        }
        
        [HttpGet]
        public ActionResult EditTest(int id)//выводит представление для редактирвоания тестов
        {//id - id названия теста
            string Email = Request.Cookies["User"].Value;
            if (Email == "AdilAdminBikeev1999h@mail.ru")//если это админ/редактор - зашифровать потом!!!!!!!!!!!!!!!!!!!
            {
                siteDb.Questions.Load();//загружаем таблицу вопросов
                siteDb.Answer_Choice.Load();//загружаем таблицу вариантов ответа

                var questions = (from t in siteDb.Questions.Local
                                 where t.ID_NameTest == id//с помощью LINQ запроса находим все вопросы для данного теста
                                 select t).ToList();

                List<Answer_Choice> answerChoice = null;

                string html = "";//для хранения html разметки
                for (int i = 0; i < questions.Count(); i++)
                {
                    answerChoice = (from t in siteDb.Answer_Choice.Local
                                    where t.ID_Question == questions[i].Id//с помощью LINQ запроса находим все варианты ответа для данного вопроса
                                    select t).ToList();

                    html += "<div class=\"onetest\" name=\"onetest\">" +
                                "<font color = \"#000099\" size=\"4\">" +
                                    "<b>" +
                                        (i + 1).ToString() + " вопрос &nbsp&nbsp&nbsp&nbsp" +
                                    "</b>" +
                                   
                                "</font>" +
                                "<span>" +
                                    "<a href=\"/Test/DeleteQuestion/"+ questions[i].Id+ "\" class=\"btn btn-danger btn - lg\">Удалить вопрос</a>" +
                                "</span>" +
                                "<br>" +
                                "<br>" +
                                "<font color = \"#333333\" size=\"3\">" +
                                    "<span>Формулировка вопроса &nbsp&nbsp&nbsp&nbsp&nbsp</span><textarea  name=\"quest" + i + "\" id=\"quest" + i + "\">" + questions[i].question + "</textarea>" +
                                "</font>" +
                                "<div class=\"otv\" id=\"torf1\">" +
                                "</div>";
                    for (int j = 0; j < answerChoice.Count(); j++)
                    {
                        html += "<div class=\"form-check\">" +
                                    "<span>" + j + " Вариант ответа &nbsp&nbsp&nbsp&nbsp&nbsp</span><textarea name=\"quest" + i + "example" + j + "\" id=\"quest" + i + "example" + j + "\">" + answerChoice[j].answer + "</textarea>" +
                                "</div>";
                    }
                    html += "</div>" +
                    "<hr>";
                }
                ViewBag.Id = id;//запинаем id названия теста
                ViewBag.html = html;//запоминаем html разметку
                return View();
            }
            return RedirectToAction("Error","NotFound");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public RedirectResult EditTest_Successful(int id)//выводит представление для редактирвоания тестов
        {
            try
            {
                string Email = Request.Cookies["User"].Value;
                if (Email == "AdilAdminBikeev1999h@mail.ru")//если это админ/редактор - зашифровать потом!!!!!!!!!!!!!!!!!!!
                {
                    //запоминаем все вопросы с вариантами ответа из формы
                    List<string> questions_Form = new List<string>();
                    List<string> answers = new List<string>();

                    //key - номер по списку вопроса
                    //values - все вопросы по заданному ключу
                    Dictionary<int, string[]> answerChoice_Form = new Dictionary<int, string[]>();
                    for (int i = 0; ; i++)
                    {
                        if (Request.Form["quest" + i] == null) break;
                        questions_Form.Add(Request.Form["quest" + i]);

                        for (int j = 0; ; j++)
                        {
                            if (Request.Form["quest" + i + "example" + j] == null) break;
                            answers.Add(Request.Form["quest" + i + "example" + j]);
                        }
                        answerChoice_Form.Add(i,answers.ToArray());
                        answers.Clear();
                    }

                    //загружаем данные из бд
                    siteDb.Questions.Load();
                    siteDb.Answer_Choice.Load();

                    //находим все вопросы по выбранному ранее админу тесту
                    var questions_DB = (from item in siteDb.Questions.Local
                                        where item.ID_NameTest == id
                                        select item).ToArray();

                    List<Answer_Choice> answersChoices_DB = null;

                    //изменяем данные в таблице
                    for (int i = 0; i < questions_DB.Count(); i++)
                    {
                        //находим все варианты ответа по i-ому вопросу
                        answersChoices_DB = (from item in siteDb.Answer_Choice.Local
                                             where item.ID_Question == questions_DB[i].Id
                                             select item).ToList();

                        //изменяем формулировку вопроса
                        siteDb.Questions.Local.First(x => x.Id == questions_DB[i].Id).question = questions_Form[i];

                        //изменяем все варианты ответа
                        for (int j = 0; j < answersChoices_DB.Count(); j++)
                        {
                            siteDb.Answer_Choice.Local.First(x => x.Id == answersChoices_DB[j].Id).answer = answerChoice_Form[i][j];
                        }
                    }

                    //сохраняем изменения
                    siteDb.SaveChanges();
                    return RedirectPermanent("~/Test/ListTest/");
                }else return RedirectPermanent("~/NotFound/Error/");
            }
            catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
                return RedirectPermanent("~/NotFound/Error/");
            }
        }

        public ActionResult ListTest()//выводит представление со списком тестов
        {
            try
            {
                siteDb.Test.Load();//заружаем описание тестов и их название
                siteDb.NameTest.Load();//загружаем навзания тестов

                HttpCookie Email = Request.Cookies["User"];//запоминаем Email авторизованного пользователя
                int countTest = siteDb.Test.Count();//запоминаем кол-во тестов
                string html = "";
                int id_nameTest;//для хранения id названия теста

                int rowFlag = 1;//сигнал для вывода на новую строку теста

                string nameTest = null;//для хранения навзания теста
                for (int i = 0; i < siteDb.Test.Count(); i++)//пробегаемся по всем тестам
                {
                    id_nameTest = int.Parse(siteDb.Test.Local[i].ID_NameTest);
                    nameTest = siteDb.NameTest.First(x => x.Id == id_nameTest).name_test;

                    if (rowFlag == 1) html += "<div class=\"row\">";//выводим тесты в новой строке

                    html += "<div class=\"col-md-4\">" +
                                    "<div class=\"card mb-4 box-shadow\" id=\"test_block\">" +
                                        "<img class=\"card-img-top\" style=\"width: 100%; height: 225px; display: block;\" alt=\"Thumbnail [100%x225]\" src=\"https://uploads.hb.cldmail.ru/category/6/image/4f4f0b8a5d0453fe21336dae56d8df63.png\" data-src=\"holder.js/100px225?theme=thumb&amp;bg=55595c&amp;fg=eceeef&amp;text=Thumbnail\" data-holder-rendered=\"true\">" +
                                            "<div class=\"card-body\">" +
                                                "<div class=\"body__item topics\">" +
                                                    "<div class=\"topics__list\">О тесте</div>" +
                                                        "<ul class=\"topics__list topics-list\">" +
                                                            "<li class=\"topics - list__item\">" + nameTest + "</li>" +
                                                        "</ul>" +
                                                    "</div>" +
                                                    "<div class=\"d-flex justify-content-between align-items-center\">" +
                                                       "<div class=\"btn-group\">" +
                                                           "<div class=\"line_block float\" style=\"width:230px\">";
                    if (Email != null)//если пользователь авторизован
                    {
                        if (Email.Value != "AdilAdminBikeev1999h@mail.ru")//если пользвоатель != редактор  
                        {
                            html += "<p style = \"position: relative; margin: 10px\">" +
                                        "<a href=\"/Test/SelectTest/" + id_nameTest + "\" class=\"btn btn-primary btn - lg\">Начать тест</a>" +
                                    "</p>";
                        }
                        else
                        {//если пользователь == редактор
                            html += "<p style = \"position: relative; margin: 10px\">" +
                                        "<a href=\"/Test/EditTest/" + id_nameTest + "\" class=\"btn btn-primary btn - lg\">Редактировать</a>" +
                                    "</p>";
                        }
                    }
                    else
                    {//если пользователь не авторизован
                        html += "<h2>Авторизация</h2>" +
                                "<h7>Чтобы начать тест, нужно авторизоваться на нашем сайте</h7>" +
                                "<p style = \"position: relative; margin: 10px\">" +
                                    "<a href=\"/Test/SelectTest\" class=\"btn btn-primary btn - lg\">Атворизоваться</a>" +
                                "</p>";
                    }
                                        html += "</div>" +
                                            "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>";
                    if (rowFlag == 2) { html += "</div>"; rowFlag = 1; } else { rowFlag++; }
                }

                #region HTML разметка для тестов
                /*    "<div class=\"row\"> "+
            "< div class=\"col-md-4\">" +
               "<div class=\"card mb-4 box-shadow\" id=\"test_block\">" +
                    "<img class=\"card-img-top\" style=\"width: 100%; height: 225px; display: block;\" alt=\"Thumbnail [100%x225]\" src=\"https://uploads.hb.cldmail.ru/category/6/image/4f4f0b8a5d0453fe21336dae56d8df63.png\" data-src=\"holder.js/100px225?theme=thumb&amp;bg=55595c&amp;fg=eceeef&amp;text=Thumbnail\" data-holder-rendered=\"true\">"+
                   "<div class=\"card-body\">"+
                       "<div class=\"body__item topics\">"+
                           "<div class=\"topics__list\">О тесте</div>"
                           "<ul class=\"topics__list topics-list\">"
                           //    <li class="topics-list__item">Работа с файловой системой и организация файлового ввода-вывода данных</li>
                           "</ul>"+
                       "</div>"+
                       "<div class=\"d-flex justify-content-between align-items-center\">"
                           "<div class=\"btn-group\">"
                               "<div class=\"line_block float\">"
                                   @{

                                       if (user != null)
                                       {//в случаи, если пользователь на странице уже авторизован
                                           if (user.Value != "AdilAdminBikeev1999h@mail.ru")
                                           {
                                               <p style = "position:relative; margin:10px" >
                                                   < a href="~/Test/Select_SharpTest" class="btn btn-primary btn-lg">Начать Тест</a>
                                               </p>
                                           }
                                           else
                                           {//если пользователь == редактор
                                               <p style = "position:relative; margin:10px" >
                                                   < a href="~/Test/EditTest" class="btn btn-primary btn-lg">Редактировать</a>
                                               </p>
                                           }
                                       }
                                       else
                                       {//если пользователь не авторизован
                                           <h2>Авторизация</h2>
                                           <h7>Чтобы начать тест, нужно авторизоваться на нашем сайте</h7>
                                           <p style = "position:relative; margin-top:30px" >
                                               < a href="~/LogIn/Autorization" class="btn btn-primary btn-lg">Авторизоваться</a>
                                           </p>
                                       }
                                   }
                               </div>
                           </div>
                       </div>
                   </div>
               </div>
           </div>
       </div>";
       */
                #endregion

                ViewBag.Html = html;//разпоминаем html разметку
            }catch(Exception exc)
            {
                ViewBag.Msg = exc.Message;
            }
            return View();
        }
        
        public ActionResult Result(int id)//Выводит результат тестирвоания
        {
            try
            {
                if (Request.Cookies["user"] != null)//если пользователь авторизован на странице
                {
                    siteDb.Questions.Load();

                    var questions = (from item in siteDb.Questions.Local
                                 where item.ID_NameTest == id//находим все вопросы для данного теста с соответствующим id
                                 select item).ToList();

                    int maxAnsTruth = questions.Count;//запоминаем кол-во вопросов
                    int noRightAns = 0;//кол-во неправильных ответов

                    string trueAnswer;//для хранения овтета пользвоателя на вопрос
                    for (int i=0; i<maxAnsTruth; i++)//пробегаемся по  вопросам
                    {
                        trueAnswer = Request.Form["quest" + i.ToString()];
                        if (trueAnswer  != questions[i].TrueAnswer.ToString()) noRightAns++;//если ответ на вопрос неверный 
                    }

                    double percentRes = ((double)(maxAnsTruth - noRightAns) / (double)(maxAnsTruth + noRightAns)) * 100;//процентное соотношение  неправильных ответов
                    string email = Request.Cookies["user"].Value;//запоминаем email авторизованного пользователя

                    siteDb.Users.Load();
                    Users user = siteDb.Users.First(x => x.Email == email);

                    siteDb.Result_Test.Load();
                    Result_Test result = siteDb.Result_Test.FirstOrDefault(x => x.ID_Users == user.Id);

                    if(result!=null)//если пользователь впервые проходит данный тест
                    {//обновляем результат прохождения теста
                        int index = siteDb.Result_Test.Local.IndexOf(result);

                        siteDb.Result_Test.Local[index].ID_Users = user.Id;
                        siteDb.Result_Test.Local[index].ID_NameTest = id;
                        siteDb.Result_Test.Local[index].Result = Convert.ToDecimal(percentRes);
                    }
                    else
                    {//добавляем результат теста 
                        result = new Result_Test
                        {
                            ID_Users = user.Id,
                            ID_NameTest = id,
                            Result = Convert.ToDecimal(percentRes)
                        };

                        siteDb.Result_Test.Local.Add(result);
                    }

                    siteDb.SaveChanges();//сохраняем изменения в бд

                    #region Определение того - какое сообщение вывести пользователю
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
                    #endregion
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
        
        public ActionResult SelectTest(int id)//при выборе теста по C#
        {
            try
            {
                if (Request.Cookies["user"] != null)//если пользователь авторизован на странице
                {
                    siteDb.Questions.Load();//загружаем таблицу вопросов
                    siteDb.Answer_Choice.Load();//загружаем таблицу вариантов ответа

                    var questions = (from t in siteDb.Questions.Local
                                    where t.ID_NameTest == id//с помощью LINQ запроса находим все вопросы для данного теста
                                    select t).ToList();

                    List<Answer_Choice> answerChoice=null;

                    string html = "";//для хранения html разметки
                    for(int i=0;i<questions.Count();i++)
                    {
                        answerChoice = (from t in siteDb.Answer_Choice.Local
                                        where t.ID_Question == questions[i].Id//с помощью LINQ запроса находим все варианты ответа для данного вопроса
                                        select t).ToList();

                        html += "<div class=\"onetest\" name=\"onetest\">" +
                                    "<font color = \"#000099\" size=\"4\">" +
                                    "<b>" +
                                        (i+1).ToString() + " вопрос" +
                                    "</b>" +
                                "</font>" +
                                "<br>" +
                                "<font color = \"#333333\" size=\"3\">" +
                                    "<p>" + questions[i].question + "</p>" +
                                "</font>" +
                                "<div class=\"otv\" id=\"torf1\">" +
                                "</div>";
                        for (int j = 0; j < answerChoice.Count(); j++)
                        {
                            html += "<div class=\"form-check\">" +
                                        "<input class=\"form-check-input\" type=\"radio\" name=\"quest" + i+"\" id=\"quest" + i + "example"+j+"\" value=\""+j+"\" checked=\"\">" +
                                        "<label class=\"form-check-label\" for=\"quest" + i + "example" + j+"\">" +
                                            answerChoice[j].answer +
                                        "</label>" +
                                    "</div>";
                        }
                        html+="</div>" +
                        "<hr>";
                    }
                    ViewBag.Id = id;//запинаем id названия теста
                    ViewBag.html = html;//запоминаем html разметку
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