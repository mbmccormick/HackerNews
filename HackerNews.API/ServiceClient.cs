using HackerNews.API.Common;
using HackerNews.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public List<string> PostHistory;
        public int MaxPostHistory = 200;

        public ServiceClient()
        {
            PostHistory = IsolatedStorageHelper.GetObject<List<string>>("PostHistory");

            if (PostHistory == null)
                PostHistory = new List<string>();
        }

        public void GetTopPosts(Action<PostResponse> callback)
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

        public void GetNewPosts(Action<PostResponse> callback)
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

        public void GetAskPosts(Action<PostResponse> callback)
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

        public void GetComments(Action<CommentResponse> callback, string postId)
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

                    int i = data.items.Count - 1;

                    while (i >= 0)
                    {
                        data.items[i] = CleanCommentText(data.items[i]);
                        i--;
                    }

                    callback(data);
                }
                catch (WebException ex)
                {
                    callback(null);
                }

            }, state);
        }

        public void MarkPostAsRead(string postId)
        {
            while (PostHistory.Count >= MaxPostHistory)
            {
                PostHistory.RemoveAt(MaxPostHistory - 1);
            }

            PostHistory.Insert(0, postId);
        }

        public void SaveData()
        {
            IsolatedStorageHelper.SaveObject<List<string>>("PostHistory", PostHistory);
        }

        private string CleanText(string input)
        {
            input = input.Replace("�", "");
            input = input.Replace("&amp;", "");
            input = input.Replace("&euro;&trade;", "'");
            input = input.Replace("&euro;&oelig;", "\"");
            input = input.Replace("&euro;?", "\"");
            input = input.Replace("&euro;&ldquo;", "-");
            input = input.Replace("&euro;", "...");
            input = input.Replace("__BR__", "\n\n");
            input = input.Replace("\\", "");

            return input;
        }

        private Comment CleanCommentText(Comment input)
        {
            input.comment = CleanText(input.comment);

            foreach (var item in input.children)
                CleanText(CleanCommentText(item).comment);

            return input;
        }
    }
}
