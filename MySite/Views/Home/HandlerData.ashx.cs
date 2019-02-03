using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MySite.Views.Home
{
    /// <summary>
    /// Сводное описание для HandlerData
    /// </summary>
    public class HandlerData : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var parametrs = context.Request.Form;//запоминаем все элеметны формы

            context.Response.ContentType = "text/plain";

            string msg = "";//для хранения сообщение которое отправится в качестве ответа клиенту
            
            Regex regLogin = new Regex(@"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$");
            if(!regLogin.IsMatch(parametrs["Login"]))//если логин правильного формата
            {
                if (msg == "") msg += "<ul>";
                msg += "<li>Логин должен состоять только 2 - 20 символов букв/цифр, которое должно начинаться с буквы</li>";
            } 

            Regex regPass = new Regex(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$");
            if (regPass.IsMatch(parametrs["Password"]))//если пароль правильного формата
            {
                if (msg == "") msg += "<ul>";
                msg += "<li>Пароль должен состоять только 2 - 20 символов букв/ цифр / спецсимволов</li>";
            }

            if (parametrs["Password"] == parametrs["tryPassword"])//если веденные пароли совпадают
            {
                if (msg == "") msg += "<ul>";
                msg += "<li>Пароли должны совпадать</li>";
            }

            Regex regEmail = new Regex(@"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$");
            if (regEmail.IsMatch(parametrs["Email"]))
            {
                msg = "success";
            }
            else
            {
                if (msg == "") msg += "<ul>";
                msg += "<li>Неверный формат Email. Пример: xx@mail.ru<li></ul>";
            }
            context.Response.Write(msg);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}