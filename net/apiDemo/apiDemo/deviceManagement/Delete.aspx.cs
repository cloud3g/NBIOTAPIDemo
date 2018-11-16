using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace apiDemo.deviceManagement
{
    public partial class Delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            apiDemo.Authentication auth = new Authentication();
            TokenModel tm = auth.GetToken();

            string appId = HttpConfig.APPID;
            string deviceId = "d5ce916d-d88c-4e97-a314-828345bbdbc0";
            string url = HttpConfig.DELETE_DEVICE + "/" + deviceId;

            AopDictionary header = new AopDictionary();
            header.Add(HttpConfig.HEADER_APP_KEY, appId);
            header.Add(HttpConfig.HEADER_APP_AUTH, "Bearer" + " " + tm.accessToken);

            string ret = HttpService.Post(url, "DELETE", "", header, true);//form
            Response.Write(ret);
        }
    }
}