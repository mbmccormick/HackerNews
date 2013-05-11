using HackerNews.API.Common;
using HackerNews.API.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace HackerNews.API
{
    public class ServiceClient
    {
        public static void GetTopItems(Action<Response> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://api.ihackernews.com/page") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                try
                {
                    var response = request.EndGetResponse(result);

                    Stream stream = response.GetResponseStream();
                    UTF8Encoding encoding = new UTF8Encoding();
                    StreamReader sr = new StreamReader(stream, encoding);

                    JsonTextReader tr = new JsonTextReader(sr);
                    Response data = new JsonSerializer().Deserialize<Response>(tr);

                    tr.Close();
                    sr.Close();

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public static void GetNewItems(Action<Response> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://api.ihackernews.com/new") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                try
                {
                    var response = request.EndGetResponse(result);

                    Stream stream = response.GetResponseStream();
                    UTF8Encoding encoding = new UTF8Encoding();
                    StreamReader sr = new StreamReader(stream, encoding);

                    JsonTextReader tr = new JsonTextReader(sr);
                    Response data = new JsonSerializer().Deserialize<Response>(tr);

                    tr.Close();
                    sr.Close();

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public static void GetAskItems(Action<Response> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://api.ihackernews.com/ask") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                try
                {
                    var response = request.EndGetResponse(result);

                    Stream stream = response.GetResponseStream();
                    UTF8Encoding encoding = new UTF8Encoding();
                    StreamReader sr = new StreamReader(stream, encoding);

                    JsonTextReader tr = new JsonTextReader(sr);
                    Response data = new JsonSerializer().Deserialize<Response>(tr);

                    tr.Close();
                    sr.Close();

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }
    }
}
