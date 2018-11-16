using System;
using System.Collections.Generic;
using System.Web;

namespace apiDemo
{
    public class DemoException : Exception 
    {
        public DemoException(string msg) : base(msg) 
        {

        }
     }
}