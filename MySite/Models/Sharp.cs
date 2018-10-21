using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MySite.Models
{
    public class Sharp
    {
        int numAnswer;

        public int NUMANSWER
        {
            get { return numAnswer; }
            set { numAnswer = value; }
        }

        public Sharp(int numAnswer)
        {
            NUMANSWER = numAnswer;
        }
    }
}