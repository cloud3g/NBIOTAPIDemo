using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace apiDemo
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {
        public static X509Certificate2 ca;

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //if (errors == SslPolicyErrors.None)
            //    return true;
            //return false;
            return true;

            //if (null != ca)
            //{
            //    /*
            //     * 根证书未安装到“受信任的根证书颁发机构”时，默认是无法形成可信证书链的。（chain中将只有服务器证书本身）
            //     * 需更改链策略，然后重新构建证书链。
            //    */
            //    // 将我们的根证书放到链引擎可搜索到的地方
            //    chain.ChainPolicy.ExtraStore.Add(ca);
            //    //不执行吊销检查
            //    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            //    //忽略CA未知情况、不做时间检查
            //    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority | X509VerificationFlags.IgnoreNotTimeNested | X509VerificationFlags.IgnoreNotTimeValid;
            //    //重新构建可信证书链
            //    bool isOk = chain.Build(certificate as X509Certificate2);
            //    if (isOk)
            //    {
            //        X509Certificate2 cacert = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;//获取最前面的证书，认为是根证书
            //        //与本地根证书比较
            //        if (ca.GetPublicKeyString().Equals(cacert.GetPublicKeyString()) && ca.Thumbprint.Equals(cacert.Thumbprint))
            //        {
            //            HttpWebRequest req = sender as HttpWebRequest;
            //            //var host = req.Address.Host; //180.101.147.89 
            //            //CERT信息验证=JKS证书
            //            //CN=www.iotplatform-demo.com, OU=CN, O=Huawei, L=SZ, S=GD, C=CN
            //            if (null != req && certificate.Subject.Contains("CN=" + req.Address.Host))//180.101.147.89 
            //            {
            //                return true;//根证书可信且服务器证书确实是指定服务器的，验证通过
            //            }
            //        }
            //    }
            //}
            //return false;

        }

        //主要
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="headers">请求头冒号分隔</param>
        /// <param name="isUseCert">是否使用证书</param>
        /// <param name="charset">utf-8</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, string header = null, bool isUseCert = false, string charset = "utf-8")
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls2;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    if (File.Exists(HttpConfig.TRUSTCAPATH_CERT))
                    {
                        ca = new X509Certificate2(HttpConfig.TRUSTCAPATH_CERT);//HttpConfig.TRUSTCAPWD
                    }
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                #region 设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;
                #endregion

                #region 请求头
                if (!string.IsNullOrEmpty(header))
                {
                    //string[] headerarr = header.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] headerarr = header.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in headerarr)
                    {
                        request.Headers.Add(item);//冒号分隔
                    }
                }
                #endregion

                #region 证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + HttpConfig.SELFCERTPATH, HttpConfig.SELFCERTPWD);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "Post used cert");
                }
                #endregion

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charset));//Encoding.UTF8
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new DemoException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new DemoException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="headers">请求头</param>
        /// <param name="isUseCert"></param>
        /// <param name="charset">utf-8</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, IDictionary<string, string> header = null, bool isUseCert = false, string charset = "utf-8")
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls2;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    if (File.Exists(HttpConfig.TRUSTCAPATH_CERT))
                    {
                        ca = new X509Certificate2(HttpConfig.TRUSTCAPATH_CERT);//HttpConfig.TRUSTCAPWD
                    }
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                #region 设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;
                #endregion

                #region 请求头
                if (header != null && header.Count > 0)
                {
                    BuildHeader(request, header);
                }
                #endregion

                #region 证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + HttpConfig.SELFCERTPATH, HttpConfig.SELFCERTPWD);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "Post used cert");
                }
                #endregion

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charset));//Encoding.UTF8
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new DemoException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new DemoException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="param">参数 &</param>
        /// <param name="headers">header头 :</param>
        /// <param name="isUseCert">false</param>
        /// <param name="contentType">application/x-www-form-urlencoded application/json</param>
        /// <param name="charset">utf-8</param>
        /// <param name="timeout">6</param>
        /// <returns>返回结果 失败抛WebException异常</returns>
        public static string Post(string url, string method = "POST", string param = null, string header = null, bool isUseCert = false, string contentType = "application/x-www-form-urlencoded", string charset = "utf-8", int timeout = 6)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;

                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls2;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    if (File.Exists(HttpConfig.TRUSTCAPATH_CERT))
                    {
                        ca = new X509Certificate2(HttpConfig.TRUSTCAPATH_CERT);//HttpConfig.TRUSTCAPWD
                    }
                }

                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.Timeout = timeout * 1000;

                #region 设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;
                #endregion

                #region 请求类型
                if (string.IsNullOrEmpty(contentType))
                {
                    //request.ContentType = "text/xml";
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    switch (contentType)
                    {
                        case "json":
                            request.ContentType = "application/json";
                            break;
                        //case "xml":
                        //    request.ContentType = "application/xml";
                        //    break;
                        //case "stream":
                        //    request.ContentType = "application/octet-stream";
                        //    break;
                        //case "all":
                        //    request.ContentType = "*/*";
                        //    break;
                        case "form":
                            request.ContentType = "application/x-www-form-urlencoded";
                            break;
                    }
                    request.ContentType = contentType;//参数类型
                }
                #endregion

                #region 请求头
                if (!string.IsNullOrEmpty(header))
                {
                    string[] headerarr = header.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in headerarr)
                    {
                        request.Headers.Add(item);//冒号分隔
                    }
                }
                #endregion

                #region 证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + HttpConfig.SELFCERTPATH, HttpConfig.SELFCERTPWD);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "Post used cert");
                }
                #endregion

                #region 请求参数
                //请求参数
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(param);
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(param);
                byte[] data = Encoding.GetEncoding(charset).GetBytes(param);
                request.ContentLength = data.Length;

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                #endregion

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charset));
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            #region trycatch
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new DemoException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new DemoException(e.ToString());
            }
            #endregion
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="param">AopDictionary 参数</param>
        /// <param name="headers">AopDictionary 请求头</param>
        /// <param name="isUseCert">false</param>
        /// <param name="contentType">application/x-www-form-urlencoded</param>
        /// <param name="charset">utf-8</param>
        /// <param name="timeout">6</param>
        /// <returns>返回结果 失败抛WebException异常</returns>
        public static string Post(string url, string method = "POST", string param = null, IDictionary<string, string> header = null, bool isUseCert = false, string contentType = "application/x-www-form-urlencoded", string charset = "utf-8", int timeout = 6)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;

                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls2;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    if (File.Exists(path + HttpConfig.TRUSTCAPATH_CERT))
                    {
                        ca = new X509Certificate2(path + HttpConfig.TRUSTCAPATH_CERT);//HttpConfig.TRUSTCAPWD
                    }
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.Timeout = timeout * 1000;

                #region 设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);           //网关服务器端口:端口
                //request.Proxy = proxy;
                #endregion

                #region 请求类型
                if (string.IsNullOrEmpty(contentType))
                {
                    //request.ContentType = "text/xml";
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    switch (contentType)
                    {
                        case "json":
                            request.ContentType = "application/json";
                            break;
                        //case "xml":
                        //    request.ContentType = "application/xml";
                        //    break;
                        //case "stream":
                        //    request.ContentType = "application/octet-stream";
                        //    break;
                        //case "all":
                        //    request.ContentType = "*/*";
                        //    break;
                        case "form":
                            request.ContentType = "application/x-www-form-urlencoded";
                            break;
                    }
                    request.ContentType = contentType;//参数类型
                }
                #endregion

                #region 请求头
                if (header != null && header.Count > 0)
                {
                    BuildHeader(request, header);
                }
                #endregion

                #region 证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + HttpConfig.SELFCERTPATH, HttpConfig.SELFCERTPWD);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "Post used cert");
                }
                #endregion

                #region 参数
                if ((method.ToLower() != "get" && param != null))
                {
                    #region 请求参数
                    //byte[] data = System.Text.Encoding.UTF8.GetBytes(param);
                    //byte[] data = System.Text.Encoding.UTF8.GetBytes(BuildQuery(param, charset));
                    byte[] data = Encoding.GetEncoding(charset).GetBytes(param);
                    request.ContentLength = data.Length;
                    #endregion

                    //往服务器写入数据
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charset));
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new DemoException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new DemoException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="param">AopDictionary 参数</param>
        /// <param name="headers">AopDictionary 请求头</param>
        /// <param name="isUseCert">false</param>
        /// <param name="contentType">application/x-www-form-urlencoded</param>
        /// <param name="charset">utf-8</param>
        /// <param name="timeout">6</param>
        /// <returns>返回结果 失败抛WebException异常</returns>
        public static string Post(string url, string method = "POST", IDictionary<string, string> param = null, IDictionary<string, string> header = null, bool isUseCert = false, string contentType = "application/x-www-form-urlencoded", string charset = "utf-8", int timeout = 6)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            string result = "";//返回结果

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;

                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls2;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    if (File.Exists(path + HttpConfig.TRUSTCAPATH_CERT))
                    {
                        ca = new X509Certificate2(path + HttpConfig.TRUSTCAPATH_CERT);//HttpConfig.TRUSTCAPWD
                    }
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = method;
                request.Timeout = timeout * 1000;

                #region 设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);           //网关服务器端口:端口
                //request.Proxy = proxy;
                #endregion

                #region 请求类型
                if (string.IsNullOrEmpty(contentType))
                {
                    //request.ContentType = "text/xml";
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    switch (contentType)
                    {
                        case "json":
                            request.ContentType = "application/json";
                            break;
                        //case "xml":
                        //    request.ContentType = "application/xml";
                        //    break;
                        //case "stream":
                        //    request.ContentType = "application/octet-stream";
                        //    break;
                        //case "all":
                        //    request.ContentType = "*/*";
                        //    break;
                        case "form":
                            request.ContentType = "application/x-www-form-urlencoded";
                            break;
                    }
                    request.ContentType = contentType;//参数类型
                }
                #endregion

                #region 请求头
                if (header != null && header.Count > 0)
                {
                    BuildHeader(request, header);
                }
                #endregion

                #region 证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + HttpConfig.SELFCERTPATH, HttpConfig.SELFCERTPWD);
                    request.ClientCertificates.Add(cert);
                    Log.Debug("WxPayApi", "Post used cert");
                }
                #endregion

                #region param
                if ((method.ToLower() != "get" && param != null))
                {
                    //请求参数
                    //byte[] data = System.Text.Encoding.UTF8.GetBytes(param);
                    //byte[] data = System.Text.Encoding.UTF8.GetBytes(BuildQuery(param, charset));
                    byte[] data = Encoding.GetEncoding(charset).GetBytes(BuildQuery(param, charset));
                    request.ContentLength = data.Length;

                    //往服务器写入数据
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charset));
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                Log.Error("Exception message: {0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Error("HttpService", e.ToString());
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Error("HttpService", "StatusCode : " + ((HttpWebResponse)e.Response).StatusCode);
                    Log.Error("HttpService", "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
                throw new DemoException(e.ToString());
            }
            catch (Exception e)
            {
                Log.Error("HttpService", e.ToString());
                throw new DemoException(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }



        //GET新增param
        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="param">参数</param>
        /// <param name="headers">请求头冒号分隔</param>
        /// <param name="isUseCert">是否使用证书</param>
        /// <param name="charset">utf-8</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, string param = null, string header = null, bool isUseCert = false, string charset = "utf-8")
        {
            url = url + "?" + param;
            return Get(url, header, isUseCert, charset);
        }

        ///// <summary>
        ///// 处理http GET请求，返回数据
        ///// </summary>
        ///// <param name="url">请求的url地址</param>
        ///// <param name="param">参数</param>
        ///// <param name="headers">请求头</param>
        ///// <param name="isUseCert">是否使用证书</param>
        ///// <param name="charset">utf-8</param>
        ///// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url, IDictionary<string, string> param = null, IDictionary<string, string> headers = null, bool isUseCert = false, string charset = "utf-8")
        {
            url = url + "?" + BuildQuery(param, charset);
            return Get(url, headers, isUseCert, charset);
        }




        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");

                    string encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));

                    postData.Append(encodedValue);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }

        /// <summary>
        /// 遍历组装header头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        public static void BuildHeader(HttpWebRequest request, IDictionary<string, string> headers)
        {
            IEnumerator<KeyValuePair<string, string>> dem = headers.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                //request.Headers.Add(name+ ":" + value);
                request.Headers.Add(name, value);
            }
        }

    }
}