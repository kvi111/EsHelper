using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace esHelper.Common
{
    public class HttpUtil
    {
        public static async Task<String> GetURL(String url, string userName = "", string password = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    AddAuthorization(userName, password, client);
                    return await client.GetStringAsync(url);//得到返回字符流
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void AddAuthorization(string userName, string password, HttpClient client)
        {
            if (string.IsNullOrEmpty(userName) == false && string.IsNullOrEmpty(password) == false)
            {
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + authorization);
            }
        }

        public static async Task<String> PostURL(String url, String data = "", string userName = "", string password = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //ByteArrayContent bac = new ByteArrayContent(new byte[] { });
                    //if (string.IsNullOrEmpty(data) == false)
                    //{
                    //    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    //    bac = new ByteArrayContent(bytes);
                    //}
                    AddAuthorization(userName, password, client);
                    StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await client.PostAsync(url, stringContent);//得到返回字符流
                    return await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static async Task<String> PutURL0(String url, String data = "")
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())  //handler
        //        {
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        //            ByteArrayContent bac = new ByteArrayContent(new byte[] { });
        //            bac.Headers.Add("Content-Type", "application/json");
        //            bac.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        //            if (string.IsNullOrEmpty(data) == false)
        //            {
        //                byte[] bytes = Encoding.UTF8.GetBytes(data);
        //                bac = new ByteArrayContent(bytes);
        //            }
        //            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        //            HttpResponseMessage res = await client.PutAsync(url, bac);//得到返回字符流
        //            return await res.Content.ReadAsStringAsync();
        //        }

        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static async Task<String> PutURL(String url, String data = "", string userName = "", string password = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    AddAuthorization(userName, password, client);
                    StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage res = await client.PutAsync(url, stringContent);//得到返回字符流
                    return await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public static async Task<String> PutURL_CreateIndex1(String url, String data = "")
        //{
        //    try
        //    {
        //        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

        //        request.Method = "PUT";
        //        request.Accept = "*/*";
        //        request.ContentType = "application/json";
        //        //request.Headers["Host"] = "localhost:9200";
        //        //request.Headers["Accept-Language"] = "zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2";
        //        //request.Headers["Accept-Encoding"] = "gzip,deflate";
        //        //request.Headers["User-Agent"] = "Mozilla /5.0 (Windows NT 10.0; WOW64; rv:58.0) Gecko/20100101 Firefox/58.0";

        //        byte[] buffer = Encoding.UTF8.GetBytes(data);
        //        Stream reqstr = await request.GetRequestStreamAsync();
        //        reqstr.Write(buffer, 0, buffer.Length);

        //        WebResponse response = await request.GetResponseAsync();
        //        StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //        return readStream.ReadToEnd();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public static async Task<String> DeleteURL(String url, string userName = "", string password = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    AddAuthorization(userName, password, client);
                    HttpResponseMessage res = await client.DeleteAsync(url);//得到返回字符流
                    return await res.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //public static String PostURL(String url)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        HttpResponseMessage response = client.PostAsync(url, null).Result;//得到返回字符流
        //        return response.Content.ReadAsStringAsync().Result;
        //    }
        //}
    }
}
