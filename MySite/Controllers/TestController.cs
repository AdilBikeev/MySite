using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MySite.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult ListTest()
        {
            return View();
        }

        public ActionResult Result()//Выводит результат тестирвоания
        {
            return View();
        }

        public ActionResult Select_SharpTeest()//при выборе теста по C#
        {
            return View();
        }
    }
}