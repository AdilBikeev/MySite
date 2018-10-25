using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.Entity;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySite.Models
{
    public class SiteDb
    {

        public static List<Users> Users;
        public static List<Sharp> Sharp;
        public SiteDb()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.Open();

            MySqlCommand command = new MySqlCommand("SELECT *FROM USERS", conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //
                //DateTime? date = reader[5].;
                Users.Add(new Users(reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), (reader[5] as DateTime), reader[6].ToString(), reader[7].ToString()));
            }
        }
    }
}