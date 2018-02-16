using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace esHelper.Common
{
    public class EsFile
    {
        /// <summary>
        /// 不使用SSH连接测试
        /// </summary>
        /// <param name="connInfo"></param>
        /// <returns></returns>
        public static async Task<bool> ConnectionTest(EsConnectionInfo connInfo)
        {
            try
            {
                string version = await EsFile.GetEsVersion(connInfo.GetLastUrl());
                if (string.IsNullOrEmpty(version) == false)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文件名称合法性检查
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool checkFileName(string fileName)
        {
            //为空或者含有非法字符 \ / : * ? " < > | 等
            if (string.IsNullOrWhiteSpace(fileName) || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool checkFileExists(string fileName)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            if (File.Exists(storageFolder.Path + "\\" + fileName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除EsFile
        /// </summary>
        /// <param name="input"></param>
        public static void DelEsFile(EsConnectionInfo input)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string filePath = storageFolder.Path + "//" + input.connectionName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 把连接信息保存到File
        /// </summary>
        /// <param name="input"></param>
        public static async void SaveEsFile(EsConnectionInfo input)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile esFile = await storageFolder.CreateFileAsync(input.connectionName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(esFile, EsFileEncoding(input));
        }

        /// <summary>
        /// 从文件读取所有可用的连接信息
        /// </summary>
        /// <returns></returns>
        public static List<EsConnectionInfo> GetEsFiles()
        {
            List<EsConnectionInfo> list = new List<EsConnectionInfo>();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            string[] files = Directory.GetFiles(storageFolder.Path);
            foreach (string file in files)
            {
                try
                {
                    string strInput = File.ReadAllText(file);
                    EsConnectionInfo input = EsFileDecoding(strInput);
                    if (input != null)
                    {
                        list.Add(input);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return list;
        }

        private static EsConnectionInfo EsFileDecoding(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<EsConnectionInfo>(str);
            }
            catch
            {
                return null;
            }
        }

        private static string EsFileEncoding(EsConnectionInfo input)
        {
            try
            {
                return JsonConvert.SerializeObject(input);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<String> GetURL(String url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    return await client.GetStringAsync(url);//得到返回字符流
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static String PostURL(String url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.PostAsync(url, null).Result;//得到返回字符流
                return response.Content.ReadAsStringAsync().Result;
            }
        }


        /// <summary>
        /// 获取ES版本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<String> GetEsVersion(String url)
        {
            string json = await GetURL(url);
            try
            {
                JObject jobj = JObject.Parse(json);
                if (jobj != null && jobj.Root["version"] != null && jobj.Root["version"]["number"] != null)
                {
                    return jobj.Root["version"]["number"].ToString();
                }
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// 获取索引列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<String[]> GetIndexList(String url)
        {
            string json = await GetURL(url + "/_cat/indices");
            try
            {
                return json.Split(Environment.NewLine.ToCharArray());
            }
            catch
            {
                return null;
            }
        }


    }
}