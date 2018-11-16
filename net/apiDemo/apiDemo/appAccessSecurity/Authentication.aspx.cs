using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace apiDemo
{
    public partial class Authentication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(action());
        }

        public string action()
        {
            string appid = HttpConfig.APPID;
            string secret = HttpConfig.SECRET;

            //var param = "appId=" + appid + "&secret=" + secret;
            AopDictionary param = new AopDictionary();
            param.Add("appId", appid);
            param.Add("secret", secret);

            string url = HttpConfig.APP_AUTH;
            int timeOut = 6;
            //string response = HttpService.Post(url, param, null, true);
            string response = HttpService.Post(url, "POST", param, null, true);
            return response;
        }

        public TokenModel GetToken()
        {
            string token = action();
            TokenModel tm = JsonConvert.DeserializeObject<TokenModel>(token);
            return tm;
        }
    }
}