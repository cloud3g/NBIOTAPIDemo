using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace apiDemo.appAccessSecurity
{
    public partial class RefreshToken : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //OJson<InvCheckResult> json = JsonConvert.DeserializeObject<OJson<InvCheckResult>>(maybejson);
            //{"accessToken":"6efa72cf4e329a872e54e8ac99c3e3","tokenType":"bearer","refreshToken":"bc6a9e4c7622234470cf65df40177da0","expiresIn":3600,"scope":"default"}

            apiDemo.Authentication auth = new Authentication();
            TokenModel tm = auth.GetToken();
            string refreshToken = tm.refreshToken;
            Response.Write(action(refreshToken));
        }

        public string action(string refreshToken)
        {
            string appid = HttpConfig.APPID;
            string secret = HttpConfig.SECRET;

            //string param = "appId=" + appid + "&secret=" + secret + "&refreshToken=" + refreshToken;
            AopDictionary param = new AopDictionary();
            param.Add("appId", appid);
            param.Add("secret", secret);
            param.Add("refreshToken", refreshToken);

            string url = HttpConfig.APP_AUTH;
            int timeOut = 6;
            string response = HttpService.Post(url, "POST", param, null, true);
            return response;
        }
    }
}