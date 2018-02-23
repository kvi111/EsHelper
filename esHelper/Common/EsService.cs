﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace esHelper.Common
{
    public class EsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sshIp">ssh主机ip</param>
        /// <param name="sshPort">ssh主机端口</param>
        /// <param name="username">ssh主机用户名</param>
        /// <param name="password">ssh主机密码</param>
        /// <param name="lanIp">本地或者远程主机ip</param>
        /// <param name="lanPort">本地或者远程主机端口</param>
        /// <returns></returns>
        public static SshClient GetSshClient(EsConnectionInfo connInfo)
        {
            SshClient client;
            //input.localPort = 8001;//todo：需要去获取本机未用端口
            try
            {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(connInfo.sshIp, connInfo.sshPort, connInfo.username, connInfo.password);
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);
                client = new SshClient(connectionInfo);
                //client.ErrorOccurred += Client_ErrorOccurred;
                client.Connect();
                if (!client.IsConnected)
                {
                    throw new Exception("SSH connect failed");
                }
                var portFwdL = new ForwardedPortLocal(connInfo.localIp, (uint)connInfo.localPort, connInfo.esIp, (uint)connInfo.esPort); //映射到本地端口
                client.AddForwardedPort(portFwdL);
                portFwdL.Start();

                if (!client.IsConnected)
                {
                    throw new Exception("Port forwarding failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return client;
        }

        /// <summary>
        /// 不使用SSH连接测试
        /// </summary>
        /// <param name="connInfo"></param>
        /// <returns></returns>
        public static async Task<bool> ConnectionTest(EsConnectionInfo connInfo)
        {
            try
            {
                string version = await EsService.GetEsVersion(connInfo);
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


        /// <summary>
        /// 获取ES版本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<String> GetEsVersion(EsConnectionInfo connInfo) //String url,string userName, string password
        {
            string json = await HttpUtil.GetURL(connInfo.GetLastUrl(), connInfo.esUsername, connInfo.esPassword);
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
        /// <param name="connInfo"></param>
        /// <returns></returns>
        public static async Task<String[]> GetIndexList(EsConnectionInfo connInfo)
        {
            string json = await HttpUtil.GetURL(connInfo.GetLastUrl() + "/_cat/indices", connInfo.esUsername, connInfo.esPassword);
            try
            {
                return json.Split(Environment.NewLine.ToCharArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取索引列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<FuncResult> CreateIndex(EsConnectionInfo connInfo, String indexName, String json)
        {
            try
            {
                string result = await HttpUtil.PutURL(connInfo.GetLastUrl() + "/" + indexName, json, connInfo.esUsername, connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return new FuncResult() { Success = false, Message = "put error" };
                }
                JObject jobj = JObject.Parse(result);
                if (jobj != null)
                {
                    if (jobj.Root["acknowledged"] != null)
                    {
                        return new FuncResult() { Success = true };
                    }
                    else if (jobj.Root["error"] != null && jobj.Root["error"]["reason"] != null)
                    {
                        return new FuncResult() { Success = false, Message = jobj.Root["error"]["reason"].ToString() };
                    }
                }
                return new FuncResult() { Success = false, Message = "error" };
            }
            catch
            {
                return new FuncResult() { Success = false, Message = "create index error!" };
            }
        }

        /// <summary>
        /// 获取索引数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="indexName">索引名称</param>
        /// <param name="json"></param>
        /// <param name="pageIndex">开始位置</param>
        /// <param name="pageCount">每页数量</param>
        /// <returns></returns>
        public static async Task<PerPageData> GetIndexData(EsConnectionInfo connInfo, String indexName, int pageIndex = 0, int pageSize = 20)
        {
            PerPageData pData;
            try
            {
                String json = "{\"query\": {\"match_all\": { }},\"from\": " + (pageIndex * pageSize).ToString() + ", \"size\": " + pageSize.ToString() + "}";
                string result = await HttpUtil.PostURL(connInfo.GetLastUrl() + "/" + indexName + "/_search", json, connInfo.esUsername, connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }
                JObject jObj = JObject.Parse(result);
                if (jObj != null)
                {
                    if (jObj.Root["hits"] != null && jObj.Root["hits"]["total"] != null)
                    {
                        int total = int.Parse(jObj.Root["hits"]["total"].ToString());
                        pData = new PerPageData(pageIndex, total, pageSize);
                        pData.pageData = jObj;
                        return pData;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取索引Mapping
        /// </summary>
        /// <param name="url"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static async Task<JObject> GetIndexMapping(EsConnectionInfo connInfo, String indexName)
        {
            try
            {
                string result = await HttpUtil.GetURL(connInfo.GetLastUrl() + "/" + indexName, connInfo.esUsername, connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }
                JObject jObj = JObject.Parse(result);
                if (jObj != null)
                {
                    if (jObj.Root[indexName] != null && jObj.Root[indexName]["mappings"] != null)
                    {
                        return jObj;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 开启已经关闭的索引
        /// </summary>
        /// <param name="url"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static async Task<bool> OpenIndex(EsConnectionInfo connInfo, String indexName)
        {
            try
            {
                string result = await HttpUtil.PostURL(connInfo.GetLastUrl() + "/" + indexName + "/_open", userName: connInfo.esUsername, password: connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                JObject jObj = JObject.Parse(result);
                if (jObj != null)
                {
                    if (jObj.Root["acknowledged"] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 关闭索引(不存在内存中，仍然在磁盘中)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static async Task<bool> CloseIndex(EsConnectionInfo connInfo, String indexName)
        {
            try
            {
                string result = await HttpUtil.PostURL(connInfo.GetLastUrl() + "/" + indexName + "/_close", userName: connInfo.esUsername, password: connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                JObject jObj = JObject.Parse(result);
                if (jObj != null)
                {
                    if (jObj.Root["acknowledged"] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="url"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteIndex(EsConnectionInfo connInfo, String indexName)
        {
            try
            {
                string result = await HttpUtil.DeleteURL(connInfo.GetLastUrl() + "/" + indexName, connInfo.esUsername, connInfo.esPassword);
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                JObject jObj = JObject.Parse(result);
                if (jObj != null)
                {
                    if (jObj.Root["acknowledged"] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}