using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace apiDemo.deviceManagement
{
    public partial class Modify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            apiDemo.Authentication auth = new Authentication();
            TokenModel tm = auth.GetToken();

            string appId = HttpConfig.APPID;
            string deviceId = "d5ce916d-d88c-4e97-a314-828345bbdbc0";
            string url = HttpConfig.MODIFY_DEVICE_INFO + "/" + deviceId;

            string manufacturerId = "dxd";
            string manufacturerName = "dxd";
            string deviceType = "dxd";
            string model = "dxd";
            string protocolType = "CoAP";
            string location = "shenzhen";
            string name = "dxd_666666666666664";

            AopDictionary param = new AopDictionary();
            param.Add("manufacturerId", manufacturerId);
            param.Add("manufacturerName", manufacturerName);
            param.Add("deviceType", deviceType);
            param.Add("model", model);
            param.Add("protocolType", protocolType);
            param.Add("location", location);
            param.Add("name", name);

            string paramstr = JsonConvert.SerializeObject(param);

            AopDictionary header = new AopDictionary();
            header.Add(HttpConfig.HEADER_APP_KEY, appId);
            header.Add(HttpConfig.HEADER_APP_AUTH, "Bearer" + " " + tm.accessToken);

            string ret = HttpService.Post(url,"put", paramstr, header, true, "application/json");//json
            Response.Write(ret);
        }
    }
}