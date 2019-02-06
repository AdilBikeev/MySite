using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace MySite.WebService
{
    /// <summary>
    /// Сводное описание для EmailService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class EmailService : System.Web.Services.WebService
    {

        [WebMethod]
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            //Указываем от кого письмо
            emailMessage.From.Add(new MailboxAddress("Администрация сайта it-learning", "itlearning2020@gmail.com"));

            //Указываем кому письмо
            emailMessage.To.Add(new MailboxAddress("", email));
            //Указываем темы сообщения
            emailMessage.Subject = subject;

            //Указываем само сообщение и его формат Plain
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using (var client = new SmtpClient())
            {
                //подключение к gmail smtp-серверу для передачи сообщения
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                
                //передача пароля и логина на сервер для аутентификации
                await client.AuthenticateAsync("itlearning2020@gmail.com", "appleitlearning");
                
                //передача самого сообщения
                await client.SendAsync(emailMessage);
                
                //отключение от сервера
                await client.DisconnectAsync(true);
            }
        }
    }
}
