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
        private string serverAddress = "hnwpapi.herokuapp.com";

        public List<string> PostHistory;
        public int MaxPostHistory = 250;

        public ServiceClient()
        {
            PostHistory = IsolatedStorageHelper.GetObject<List<string>>("PostHistory");

            if (PostHistory == null)
                PostHistory = new List<string>();
        }

        public void GetTopPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/news") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

                tr.Close();
                sr.Close();

                foreach (var item in data)
                {
                    item.title = CleanText(item.title);
                    item.is_read = PostHistory.Contains(item.id);

                    if (item.url.StartsWith("http") == false)
                        item.url = "http://news.ycombinator.com/item?id=" + item.id;
                }

                callback(data);

            }, state);
        }

        public void GetNewPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/newest") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

                tr.Close();
                sr.Close();

                foreach (var item in data)
                {
                    item.title = CleanText(item.title);
                    item.is_read = PostHistory.Contains(item.id);

                    if (item.url.StartsWith("http") == false)
                        item.url = "http://news.ycombinator.com/item?id=" + item.id;
                }

                callback(data);

            }, state);
        }

        public void GetAskPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/ask") as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

                tr.Close();
                sr.Close();

                foreach (var item in data)
                {
                    item.title = CleanText(item.title);
                    item.is_read = PostHistory.Contains(item.id);

                    if (item.url.StartsWith("http") == false)
                        item.url = "http://news.ycombinator.com/item?id=" + item.id;
                }

                callback(data);

            }, state);
        }

        public void GetComments(Action<CommentResponse> callback, string postId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/item/" + postId) as HttpWebRequest;
            request.Accept = "application/json";

            AsyncState state = new AsyncState();
            state.request = request;

            request.BeginGetResponse((result) =>
            {
                var response = request.EndGetResponse(result);

                Stream stream = response.GetResponseStream();
                UTF8Encoding encoding = new UTF8Encoding();
                StreamReader sr = new StreamReader(stream, encoding);

                JsonTextReader tr = new JsonTextReader(sr);
                CommentResponse data = new JsonSerializer().Deserialize<CommentResponse>(tr);

                tr.Close();
                sr.Close();

                if (data.url.StartsWith("http") == false)
                    data.url = "http://news.ycombinator.com/item?id=" + data.id;

                if (data.content != null)
                {
                    Comment child = new Comment();

                    child.comments = null;
                    child.content = data.content;
                    child.id = null;
                    child.level = 0;
                    child.time_ago = data.time_ago;
                    child.title = data.user + " " + data.time_ago;
                    child.user = data.user;

                    data.comments.Insert(0, child);
                }

                int i = data.comments.Count - 1;

                while (i >= 0)
                {
                    data.comments[i] = CleanCommentText(data.comments[i]);
                    i--;
                }

                callback(data);

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
            input = input.Replace("&euro;&tilde;", "'");
            input = input.Replace("&euro;", "...");
            input = input.Replace("__BR__", "\n\n");
            input = input.Replace("\\", "");

            return input;
        }

        private Comment CleanCommentText(Comment input)
        {
            input.content = CleanText(input.content);

            if (input.comments != null)
            {
                foreach (var item in input.comments)
                    CleanText(CleanCommentText(item).content);
            }

            return input;
        }
    }
}
