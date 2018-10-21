using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class Users
    {
        #region Закрытые поля
            int id;
            string login;
            string password;
            string email;
            string name;
            string surname;
            DateTime dateBirthDay;
            string aboutOneself;
            int sharp;
        #endregion

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string LOGIN
        {
            get { return login; }
            set { login = value; }
        }

        public string PASSWORD
        {
            get { return password; }
            set { password = value; }
        }

        public string EMAIL
        {
            get { return email; }
            set { email = value; }
        }

        public string NAME
        {
            get { return name; }
            set { name = value; }
        }

        public string SURNAME
        {
            get { return surname; }
            set { surname = value; }
        }

        public DateTime DATEBIRTHDAY
        {
            get { return dateBirthDay; }
            set { dateBirthDay = value; }
        }

        public string ABOUTONESELF
        {
            get { return aboutOneself; }
            set { aboutOneself = value; }
        }

        public int SHARP
        {
            get { return sharp; }
            set { sharp = value; }
        }

        public Users(string login, string email, string name, string surname, DateTime dateBirthDay, string aboutOneSelf, int sharp)
        {
            LOGIN = login;
            EMAIL = email;
            NAME = name;
            SURNAME = surname;
            DATEBIRTHDAY = dateBirthDay;
            ABOUTONESELF = aboutOneSelf;
            SHARP = sharp;
        }
    }
}