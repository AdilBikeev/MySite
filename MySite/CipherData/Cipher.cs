using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MySite.CipherData
{
    public class Cipher
    {
        public static string GetMD5Hach(string text)
        {
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] hashCode = md5Provider.ComputeHash(Encoding.Default.GetBytes(text));
            
            return BitConverter.ToString(hashCode).ToLower().Replace("-", "");
        }
    }
}