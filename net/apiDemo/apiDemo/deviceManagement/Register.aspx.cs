using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace apiDemo.deviceManagement
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            apiDemo.Authentication auth = new Authentication();
            TokenModel tm = auth.GetToken();

            string appId = HttpConfig.APPID;
            string url = HttpConfig.REGISTER_DEVICE_B;

            AopDictionary param = new AopDictionary();
            param.Add("verifyCode", "666666666666664");
            param.Add("nodeId", "666666666666664");
            //param.Add("endUserId", "666666666666664");
            //param.Add("timeout", 100);

            string paramstr = JsonConvert.SerializeObject(param);

            //Response.Write(tm.accessToken+"\r\n");
            AopDictionary header = new AopDictionary();
            header.Add(HttpConfig.HEADER_APP_KEY, appId);
            header.Add(HttpConfig.HEADER_APP_AUTH, "Bearer" + " " + tm.accessToken);

            //url = url + "?appId=" + appId;
            string ret = HttpService.Post(url,"POST", paramstr, header, true,"application/json");//json
            Response.Write(ret);
        }
    }
}