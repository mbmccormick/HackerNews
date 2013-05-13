using HackerNews.API.Common;
using HackerNews.API.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
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
        public static void GetTopPosts(Action<PostResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://hnwpapi.appspot.com/news") as HttpWebRequest;
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
                    PostResponse data = new JsonSerializer().Deserialize<PostResponse>(tr);

                    tr.Close();
                    sr.Close();

                    foreach (var item in data.items)
                        item.title = CleanText(item.title);

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public static void GetNewPosts(Action<PostResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://hnwpapi.appspot.com/newest") as HttpWebRequest;
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
                    PostResponse data = new JsonSerializer().Deserialize<PostResponse>(tr);

                    tr.Close();
                    sr.Close();

                    foreach (var item in data.items)
                        item.title = CleanText(item.title);

                    // TODO: fix this on the server side
                    foreach (var item in data.items)
                        if (item.time != null)
                            item.time = item.time.Replace("0 minutes ago", "just now");

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public static void GetAskPosts(Action<PostResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://hnwpapi.appspot.com/ask") as HttpWebRequest;
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
                    PostResponse data = new JsonSerializer().Deserialize<PostResponse>(tr);

                    tr.Close();
                    sr.Close();

                    foreach (var item in data.items)
                        item.title = CleanText(item.title);

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public static void GetComments(Action<CommentResponse> callback, string postId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://hnwpapi.appspot.com/nestedcomments/format/json/id/" + postId) as HttpWebRequest;
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
                    CommentResponse data = new JsonSerializer().Deserialize<CommentResponse>(tr);

                    tr.Close();
                    sr.Close();

                    foreach (var item in data.items)
                        item.comment = CleanText(item.comment);

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        private static string CleanText(string input)
        {
            string output = input;

            output = output.Replace("�", "");
            output = output.Replace("&euro;&trade;", "'");
            output = output.Replace("&euro;&oelig;", "\"");
            output = output.Replace("&euro;?", "\"");
            output = output.Replace("&euro;&ldquo;", "-");

            return output;
        }
    }
}
