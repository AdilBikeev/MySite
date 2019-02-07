using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySite.CipherData;

namespace MySite.WebService
{
    /// <summary>
    /// Сводное описание для VerificationService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class VerificationService : System.Web.Services.WebService
    {

        [WebMethod]
        public static string GetToken()//формирует случайный токен для обычных пользователей
        {
            int length = new Random(DateTime.Now.Millisecond - 100).Next(0, 50);//радномная длина токена
            string token = string.Empty;//для хранения токена
            char c;

            for (int i = 0; i < length; i++)
            {
                c = (char)(new Random(DateTime.Now.Millisecond).Next() % 99 + 32);//рандомный символ

                while( token.IndexOf(c) != -1 || c == '<' || c == '>' || c == '/' || c == '\\') c = (char)(new Random(DateTime.Now.Millisecond).Next() % 99 + 32);//пока есть повторящийся символ

                token += c;
            }

            return token;
        }

        [WebMethod]
        public static string GetTokenForAdmin()//формирует случайный токен для админа
        {
            return Cipher.GetMD5Hach("admin@mail.ru");
        }

        [WebMethod]
        public static bool isAdmin(string token)//проверка пользвателя на админа
        {
            if (token == GetTokenForAdmin()) return true;
            return false;
        }
    }
}
