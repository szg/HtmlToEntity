//-----------------------------------------------------------------------
// <copyright file="HttpClient.cs" company="STO, Ltd.">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// HttpClient
/// HTTP GET、POST
/// 
/// 修改纪录
/// 
/// 2019-12-10 版本：1.1 孙志广 增加返回内容编码判断，发送请求增加CookieContainer
/// 2017-09-29 版本：1.0 孙志广 创建文件。     
/// 
/// <author>
///     <name>孙志广</name>
///     <date>2017-09-29</date>
/// </author>
/// </summary>
public class HttpClient
{
    /// <summary>
    /// POST请求与获取结果
    /// <param name="url">网址</param>
    /// <param name="postDataStr">发送的数据</param>
    /// <param name="isSuccess">请求是否成功</param>
    /// <param name="timeout">获取超时时间，默认为10秒</param>
    /// </summary>
    public static string Post(string url, string postDataStr, out bool isSuccess, int timeout = 10)
    {
        HttpWebRequest request = null;
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url 不能为空");
            if (timeout < 1)
                throw new ArgumentNullException("timeout 超时时间不能小于1秒");

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf8";
            request.Timeout = timeout * 1000;
            request.KeepAlive = false;
            request.CookieContainer = new CookieContainer();
            byte[] data = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = data.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding encoding = GetEncodingByContentType(response.ContentType);
            StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
            string retString = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();

            isSuccess = true;
            return retString;
        }
        catch (Exception exc)
        {
            isSuccess = false;
            return exc.Message;
        }
        finally
        {
            if (request != null)
            {
                request.Abort();
            }
        }
    }

    /// <summary>
    /// POST请求与获取结果
    /// <param name="url">网址</param>
    /// <param name="paramDic">发送的数据字典</param>
    /// <param name="isSuccess">请求是否成功</param>
    /// <param name="timeout">获取超时时间，默认为10秒</param>
    /// </summary>
    public static string Post(string url, Dictionary<string, string> paramDic, out bool isSuccess, int timeout = 10)
    {
        HttpWebRequest request = null;
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url 不能为空");
            if (timeout < 1)
                throw new ArgumentNullException("timeout 超时时间不能小于1秒");

            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (KeyValuePair<string, string> kvp in paramDic)
            {
                sb.AppendFormat("{2}{0}={1}", kvp.Key, kvp.Value, count == 1 ? "" : "&");
                count++;
            }

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf8";
            request.KeepAlive = false;
            request.Timeout = timeout * 1000;
            request.CookieContainer = new CookieContainer();
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            request.ContentLength = data.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding encoding = GetEncodingByContentType(response.ContentType);
            StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
            string retString = reader.ReadToEnd();
            reader.Dispose();
            stream.Dispose();

            isSuccess = true;
            return retString;
        }
        catch (Exception exc)
        {
            isSuccess = false;
            return exc.Message;
        }
    }

    /// <summary>
    /// GET请求与获取结果
    /// <param name="url">网址</param>
    /// <param name="getDataStr">发送的数据</param>
    /// <param name="isSuccess">请求是否成功</param>
    /// <param name="timeout">获取超时时间，默认为10秒</param>
    /// </summary>
    public static string Get(string url, string getDataStr, CookieCollection cookie, out bool isSuccess, out CookieCollection cookies, string referer = null, int timeout = 10)
    {
        HttpWebRequest request = null;
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url 不能为空");
            if (timeout < 1)
                throw new ArgumentNullException("timeout 超时时间不能小于1秒");

            request = (HttpWebRequest)WebRequest.Create(url + (getDataStr == "" ? "" : "?") + getDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.KeepAlive = false;
            request.Timeout = timeout * 1000;
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookie);
            request.Referer = referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            cookies = response.Cookies;
            Encoding encoding = GetEncodingByContentType(response.ContentType);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            myStreamReader.Dispose();
            myResponseStream.Dispose();
            isSuccess = true;

            return retString;
        }
        catch (Exception exc)
        {
            if (cookie != null)
                cookies = cookie;
            else
                cookies = new CookieCollection();
            isSuccess = false;
            return exc.Message;
        }
    }

    /// <summary>
    /// GET请求与获取结果
    /// <param name="url">网址</param>
    /// <param name="paramDic">发送的数据字典</param>
    /// <param name="isSuccess">请求是否成功</param>
    /// <param name="timeout">获取超时时间，默认为10秒</param>
    /// </summary>
    public static string Get(string url, Dictionary<string, string> paramDic, out bool isSuccess, int timeout = 10)
    {
        HttpWebRequest request = null;
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException("url 不能为空");
            if (timeout < 1)
                throw new ArgumentNullException("timeout 超时时间不能小于1秒");

            StringBuilder sb = new StringBuilder(url);
            if (paramDic != null && paramDic.Count > 0 && !url.EndsWith("?"))
            {
                // 参数存在 url 不是以 ? 结尾的 ，加上 ?
                sb.Append("?");
            }
            if (paramDic != null && paramDic.Count > 0)
            {
                int count = 1;
                foreach (KeyValuePair<string, string> kvp in paramDic)
                {
                    sb.AppendFormat("{2}{0}={1}", kvp.Key, kvp.Value, count == 1 ? "" : "&");
                    count++;
                }
            }

            request = (HttpWebRequest)WebRequest.Create(sb.ToString());
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.KeepAlive = false;
            request.Timeout = timeout * 1000;
            request.CookieContainer = new CookieContainer();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Encoding encoding = GetEncodingByContentType(response.ContentType);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            myStreamReader.Dispose();
            myResponseStream.Dispose();
            isSuccess = true;

            return retString;
        }
        catch (TimeoutException te)
        {
            isSuccess = false;
            return te.Message;
        }
        catch (Exception exc)
        {
            isSuccess = false;
            return exc.Message;
        }
        finally
        {
            if (request != null)
            {
                request.Abort();
            }
        }
    }

    static string regexStr = "charset=.*?(?=$|\\s|\\;|\")";
    static Regex regex = new Regex(regexStr, RegexOptions.IgnoreCase);
    private static Encoding GetEncodingByContentType(string encodingStr)
    {
        Encoding encoding = Encoding.UTF8;
        if (string.IsNullOrEmpty(encodingStr) || encodingStr.Trim().Equals("text/html"))
            return encoding;
        string val = encodingStr;
        var isMatch = regex.IsMatch(encodingStr);
        if (isMatch)
        {
            var match = regex.Match(encodingStr);
            val = match.Value;
            val = val.Replace("charset=", "");
        }
        if (string.IsNullOrEmpty(val)) return encoding;
        val = val.Trim().ToUpper();
        switch (val)
        {
            case "UTF-8":
            case "UTF-7":
            case "UTF-32":
            case "UTF-32BE":
            case "UTF-16":
            case "UTF-16BE":
            case "ASCII":
            case "US-ASCII":
            case "GB2312":
            case "GBK":
            case "UNICODE":
            case "ISO-8859-1":
                encoding = Encoding.GetEncoding(val);
                break;
            default:
                try
                {
                    encoding = Encoding.GetEncoding(val);
                }
                catch (Exception e)
                {
                }
                break;
        }
        return encoding;
    }
}