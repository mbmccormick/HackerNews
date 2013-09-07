using HackerNews.API.Common;
using HackerNews.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private string serverAddress = "node-hnapi.herokuapp.com";

        public List<string> PostHistory;
        public int MaxPostHistory = 250;

        public ServiceClient()
        {
            PostHistory = IsolatedStorageHelper.GetObject<List<string>>("PostHistory");

            if (PostHistory == null)
                PostHistory = new List<string>();
        }

        public async Task GetTopPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/news") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            foreach (var item in data)
            {
                item.title = CleanText(item.title);
                item.is_read = PostHistory.Contains(item.id);

                if (item.url.StartsWith("http") == false)
                    item.url = "http://news.ycombinator.com/item?id=" + item.id;
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public async Task GetNewPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/newest") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            foreach (var item in data)
            {
                item.title = CleanText(item.title);
                item.is_read = PostHistory.Contains(item.id);

                if (item.url.StartsWith("http") == false)
                    item.url = "http://news.ycombinator.com/item?id=" + item.id;
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public async Task etAskPosts(Action<List<Post>> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/ask") as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            List<Post> data = new JsonSerializer().Deserialize<List<Post>>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            foreach (var item in data)
            {
                item.title = CleanText(item.title);
                item.is_read = PostHistory.Contains(item.id);

                if (item.url.StartsWith("http") == false)
                    item.url = "http://news.ycombinator.com/item?id=" + item.id;
            }

            for (int i = 0; i < data.Count; i++)
            {
                data[i] = FormatPost(data[i]);
            }

            callback(data);
        }

        public async Task GetComments(Action<CommentResponse> callback, string postId)
        {
            HttpWebRequest request = HttpWebRequest.Create("http://" + serverAddress + "/item/" + postId) as HttpWebRequest;
            request.Accept = "application/json";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            CommentResponse data = new JsonSerializer().Deserialize<CommentResponse>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            if (data.url.StartsWith("http") == false)
                data.url = "http://news.ycombinator.com/item?id=" + data.id;

            data = FormatCommentPost(data);

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
                data.comments[i] = FormatComment(data.comments[i]);

                i--;
            }

            callback(data);
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

        private Post FormatPost(Post data)
        {
            if (data.type == "job")
            {
                data.points = 0;
                data.user = "N/A";
            }

            data.time_ago = data.time_ago.Replace("0 minutes ago", "just now");
            data.description = data.points == 1 ? data.points + " point, posted " + data.time_ago + " by " + data.user : data.points + " points, posted " + data.time_ago + " by " + data.user;
            data.description = data.type == "job" ? "Posted " + data.time_ago : data.description;

            return data;
        }

        private CommentResponse FormatCommentPost(CommentResponse data)
        {
            if (data.type == "job")
            {
                data.points = 0;
                data.user = "N/A";
            }

            data.time_ago = data.time_ago.Replace("0 minutes ago", "just now");
            data.description = data.points == 1 ? data.points + " point, posted " + data.time_ago + " by " + data.user : data.points + " points, posted " + data.time_ago + " by " + data.user;
            data.description = data.type == "job" ? "Posted " + data.time_ago : data.description;

            return data;
        }

        private Comment FormatComment(Comment data)
        {
            data.time_ago = data.time_ago.Replace("0 minutes ago", "just now");
            data.title = data.user + " " + data.time_ago;

            for (int i = 0; i < data.comments.Count; i++)
            {
                data.comments[i] = FormatComment(data.comments[i]);
            }

            return data;
        }

        private string CleanText(string data)
        {
            data = data.Replace("�", "");
            data = data.Replace("&amp;", "");
            data = data.Replace("&euro;&trade;", "'");
            data = data.Replace("&euro;&oelig;", "\"");
            data = data.Replace("&euro;?", "\"");
            data = data.Replace("&euro;&ldquo;", "-");
            data = data.Replace("&euro;&tilde;", "'");
            data = data.Replace("&euro;", "...");
            data = data.Replace("__BR__", "\n\n");
            data = data.Replace("\\", "");

            if (data.StartsWith("<p>"))
                data = data.Substring(3);

            if (data.EndsWith("<p>"))
                data = data.Substring(0, data.Length - 3);

            data = data.Trim();

            return data;
        }

        private Comment CleanCommentText(Comment data)
        {
            data.content = CleanText(data.content);

            foreach (var item in data.comments)
                CleanText(CleanCommentText(item).content);

            return data;
        }
    }
}
