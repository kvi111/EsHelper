using System;
using System.IO;
using System.Net;
using System.Text;
using Windows.Web.Http;

namespace esHelper
{
    //public class RestClient
    //{
    //    private static string BaseUri;
    //    public RestClient(string baseUri)
    //    {
    //        BaseUri = baseUri;
    //    }


    //    #region Delete方式
    //    private static string Delete(string uri, string data = "")
    //    {
    //        string serviceUrl = "";
    //        if (BaseUri == "" || BaseUri == null)
    //        {
    //            serviceUrl = uri;
    //        }
    //        else
    //        {
    //            serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
    //        }
    //        return CommonHttpRequest(serviceUrl, "DELETE", data);
    //    }
    //    #endregion
    //    #region Put方式
    //    private static string Put(string uri, string data)
    //    {
    //        string serviceUrl = "";
    //        if (BaseUri == "" || BaseUri == null)
    //        {
    //            serviceUrl = uri;
    //        }
    //        else
    //        {
    //            serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
    //        }
    //        return CommonHttpRequest(serviceUrl, "PUT", data);
    //    }
    //    #endregion
    //    #region POST方式实现
    //    private static string Post(string uri, string data)
    //    {
    //        string serviceUrl = "";
    //        if (BaseUri == "" || BaseUri == null)
    //        {
    //            serviceUrl = uri;
    //        }
    //        else
    //        {
    //            serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
    //        }
    //        return CommonHttpRequest(serviceUrl, "Post", data);
    //    }
    //    #endregion
    //    #region GET方式实现
    //    private static string Get(string uri)
    //    {
    //        string serviceUrl = "";
    //        if (BaseUri == "" || BaseUri == null)
    //        {
    //            serviceUrl = uri;
    //        }
    //        else
    //        {
    //            serviceUrl = string.Format("{0}/{1}", BaseUri, uri);
    //        }


    //        return CommonHttpRequest(serviceUrl, "GET");
    //    }
    //    #endregion
    //    #region  私有方法
    //    private static string CommonHttpRequest(string url, string type, string data = "")
    //    {
    //        HttpWebRequest myRequest = null;
    //        Stream outstream = null;
    //        HttpWebResponse myResponse = null;
    //        StreamReader reader = null;
    //        try
    //        {
    //            //构造http请求的对象
    //            myRequest = (HttpWebRequest)WebRequest.Create(url);


    //            //设置
    //            //myRequest.ProtocolVersion = HttpVersion.Http10;
    //            myRequest.Method = type;

    //            if (data.Trim() != "")
    //            {
    //                myRequest.ContentType = "text/xml";
    //                //myRequest.ContentLength = data.Length;
    //               // myRequest.Headers.Add("data", data);

    //                //转成网络流
    //                byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

    //                outstream = myRequest.GetRequestStream();
    //                outstream.Flush();
    //                outstream.Write(buf, 0, buf.Length);
    //                outstream.Flush();
    //                outstream.Close();
    //            }
    //            // 获得接口返回值
    //            myResponse = (HttpWebResponse)myRequest.GetResponse();
    //            reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
    //            string ReturnXml = reader.ReadToEnd();
    //            reader.Close();
    //            myResponse.Close();
    //            myRequest.Abort();
    //            return ReturnXml;
    //        }
    //        catch (Exception)
    //        {
    //            // throw new Exception();
    //            if (outstream != null) outstream.Close();
    //            if (reader != null) reader.Close();
    //            if (myResponse != null) myResponse.Close();
    //            if (myRequest != null) myRequest.Abort();
    //            return "";
    //        }
    //    }
    //    #endregion


    //    #region 通用请求
    //    /// <summary>
    //    /// Http通用请求
    //    /// </summary>
    //    /// <param name="url"></param>
    //    /// <param name="type"></param>
    //    /// <param name="inputData"></param>
    //    /// <returns></returns>
    //    public static string HttpRequest(string url, HttpType type, string inputData = "")
    //    {
    //        switch (type)
    //        {
    //            case HttpType.PUT:
    //                return Put(url, inputData);
    //            case HttpType.GET:
    //                return Get(url);
    //            case HttpType.POST:
    //                return Post(url, inputData);
    //            case HttpType.DELETE:
    //                return Delete(url, inputData);
    //            default:
    //                break;
    //        }
    //        return "";
    //    }


    //    /// <summary>
    //    /// Http通用请求
    //    /// </summary>
    //    /// <param name="ip"></param>
    //    /// <param name="port"></param>
    //    /// <param name="uri"></param>
    //    /// <param name="type"></param>
    //    /// <param name="inputData"></param>
    //    /// <returns></returns>
    //    public static string HttpRequest(string ip, string port, string uri, HttpType type, string inputData = "")
    //    {
    //        string url = "http://" + ip + ":" + port + uri;
    //        return HttpRequest(url, type, inputData);
    //    }


    //    #endregion


    //    public enum HttpType
    //    {
    //        PUT = 0,
    //        GET = 1,
    //        POST = 2,
    //        DELETE = 3


    //    }

    //    //socket实现方式也记录下吧

    //    //public static string Post(string url, string xmlString)
    //    //{
    //    //    string body = HeadlerInit();
    //    //    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(body);
    //    //    int bodylength = byteArray.Length;
    //    //    string resc = HttpHelper.SocketSendReceive(body, "192.168.1.222", 8000);
    //    //    return resc;

    //    //}

    //    public static string HeadlerInit()
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.AppendLine("POST /Test HTTP/1.1");


    //        sb.AppendLine("Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, */*");


    //        sb.AppendLine("User-Agent: Mozilla/4.0 (compatible; MSIE 5.5; Windows 98)");


    //        sb.AppendLine("Content-Type: application/x-www-form-urlencoded");


    //        sb.AppendLine("Content-Length: 195");


    //        sb.AppendLine("Host: 192.168.1.222:8000");


    //        sb.AppendLine("Connection: Keep-Alive");


    //        sb.AppendLine("Cache-Control: no-cache");




    //        sb.Append("\r\n"); //死在这
    //        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?> <DataServer version=\"1.0\" ><id>7</id><ip>192.168.1.250</ip></DataServer>");
    //        return sb.ToString();
    //    }

    //    public static string SocketSendReceive(string request, string server, int port)
    //    {
    //        try
    //        {
    //            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
    //            Byte[] bytesReceived = new Byte[655350];
    //            // 创建连接
    //            Socket s = ConnectSocket(server, port);
    //            if (s == null)
    //                return ("Connection failed");
    //            // 发送内容.
    //            s.Send(bytesSent, bytesSent.Length, 0);
    //            // Receive the server home page content.
    //            int bytes = 0;
    //            string page = "Default HTML page on " + server + ":\r\n";
    //            //接受返回的内容.
    //            do
    //            {
    //                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
    //                page = page + Encoding.UTF8.GetString(bytesReceived, 0, bytes);
    //            }
    //            while (bytes > 0);


    //            return page;
    //        }
    //        catch (Exception ex)
    //        {
    //            return ex.Message;
    //        }
    //    }
    //}
}
