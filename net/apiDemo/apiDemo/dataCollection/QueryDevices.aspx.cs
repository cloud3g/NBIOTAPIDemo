using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace apiDemo.dataCollection
{
    public partial class QueryDevices : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            apiDemo.Authentication auth = new Authentication();
            TokenModel tm = auth.GetToken();

            string appId = HttpConfig.APPID;
            string urlQueryDevices = HttpConfig.QUERY_DEVICES;

            int pageNo = 0;
            int pageSize = 10;

            string charset = "utf-8";

            //string param = "appId=" + appId + "&pageNo=" + pageNo + "&pageSize=" + pageSize;
            //urlQueryDevices = urlQueryDevices + "?" + param;
            AopDictionary param = new AopDictionary();
            param.Add("appId", appId);
            param.Add("pageNo", pageNo);
            param.Add("pageSize", pageSize);
            urlQueryDevices = urlQueryDevices + "?" + HttpService.BuildQuery(param, charset);

            //string header = HttpConfig.HEADER_APP_KEY + ":" + appId + "&" + HttpConfig.HEADER_APP_AUTH + ":" + "Bearer" + " " + tm.accessToken;
            AopDictionary header = new AopDictionary();
            header.Add(HttpConfig.HEADER_APP_KEY, appId);
            header.Add(HttpConfig.HEADER_APP_AUTH, "Bearer" + " " + tm.accessToken);

            int timeOut = 6;
            //string contentType = "application/json";//application/json //application/x-www-form-urlencoded
            //string response = HttpService.Post(urlQueryDevices, param, header, contentType, "utf-8", true, timeOut);
            string response = HttpService.Get(urlQueryDevices, header, true);
            Response.Write(response);
        }
    }
}