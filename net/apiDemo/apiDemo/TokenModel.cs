using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiDemo
{
    public class TokenModel
    {
        //{"accessToken":"6efa72cf4e329a872e54e8ac99c3e3","tokenType":"bearer","refreshToken":"bc6a9e4c7622234470cf65df40177da0","expiresIn":3600,"scope":"default"}
        public string accessToken { get; set; }
        public string tokenType { get; set; }
        public string refreshToken { get; set; }
        public int expiresIn { get; set; }
        public string scope { get; set; }
    }
}