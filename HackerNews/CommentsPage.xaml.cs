using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HackerNews.API;
using HackerNews.API.Models;
using System.Collections.ObjectModel;
using HackerNews.Common;
using Microsoft.Phone.Tasks;
using System.Windows.Media;

namespace HackerNews
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        #region List Properties

        public static CommentResponse CurrentPost { get; set; }

        public static ObservableCollection<Comment> Comments { get; set; }

        #endregion

        private bool isLoaded = false;

        public CommentsPage()
        {
            InitializeComponent();

            Comments = new ObservableCollection<Comment>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isLoaded == false)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                App.HackerNewsClient.GetComments((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentPost = result;

                        if (CurrentPost != null &&
                            CurrentPost.comments != null)
                        {
                            this.txtTitle.Text = CurrentPost.title;
                            this.txtBody.Text = CurrentPost.description;

                            Comments.Clear();

                            foreach (Comment item in CurrentPost.comments)
                            {
                                Comments.Add(item);
                            }

                            if (CurrentPost.content != null)
                            {
                                Comment data = new Comment();

                                data.comments = null;
                                data.content = CurrentPost.content;
                                data.id = null;
                                data.level = 0;
                                data.time_ago = CurrentPost.time_ago;
                                data.title = CurrentPost.user + " " + CurrentPost.time_ago;
                                data.user = CurrentPost.user;

                                Comments.Insert(0, data);
                            }
                        }

                        isLoaded = true;

                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    });
                }, id);
            }
        }

        private void Share_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = CurrentPost.title;
            shareLinkTask.LinkUri = new Uri(CurrentPost.url);
            shareLinkTask.Show();
        }

        private void Navigate_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("http://news.ycombinator.com/item?id=" + CurrentPost.id);
            webBrowserTask.Show();
        }

        private void Feedback_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.To = App.FeedbackEmailAddress;
            emailComposeTask.Subject = "Hacker News Feedback";
            emailComposeTask.Body = "Version " + App.ExtendedVersionNumber + " (" + App.PlatformVersionNumber + ")\n\n";
            emailComposeTask.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void ToggleLoadingText()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

                this.lstComments.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void ToggleEmptyText()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                if (Comments.Count == 0)
                    this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                else
                    this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });
        }
    }
}