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
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public ObservableCollection<Post> TopPosts { get; set; }
        public ObservableCollection<Post> NewPosts { get; set; }
        public ObservableCollection<Post> AskPosts { get; set; }

        #endregion

        private bool topLoaded = false;
        private bool newLoaded = false;
        private bool askLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            this.TopPosts = new ObservableCollection<Post>();
            this.NewPosts = new ObservableCollection<Post>();
            this.AskPosts = new ObservableCollection<Post>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                if (topLoaded == false ||
                    newLoaded == false ||
                    askLoaded == false)
                    LoadData();
            }
        }

        private void LoadData()
        {
            topLoaded = false;
            newLoaded = false;
            askLoaded = false;

            

            GlobalLoading.Instance.IsLoading = true;

            App.HackerNewsClient.GetTopPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    if (result != null &&
                        result.items != null &&
                        result.items.Count > 0)
                    {
                        this.TopPosts.Clear();

                        foreach (Post item in result.items)
                        {
                            this.TopPosts.Add(item);
                        }

                        this.TopPosts.RemoveAt(this.TopPosts.Count - 1);
                    }

                    topLoaded = true;
                });
            });

            App.HackerNewsClient.GetNewPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    if (result != null &&
                        result.items != null &&
                        result.items.Count > 0)
                    {
                        this.NewPosts.Clear();

                        foreach (Post item in result.items)
                        {
                            this.NewPosts.Add(item);
                        }

                        this.NewPosts.RemoveAt(this.NewPosts.Count - 1);
                    }

                    newLoaded = true;
                });
            });

            App.HackerNewsClient.GetAskPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    if (result != null &&
                        result.items != null &&
                        result.items.Count > 0)
                    {
                        this.AskPosts.Clear();

                        foreach (Post item in result.items)
                        {
                            this.AskPosts.Add(item);
                        }

                        this.AskPosts.RemoveAt(this.AskPosts.Count - 1);
                    }

                    askLoaded = true;
                });
            });
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Feedback_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.To = "matt@mbmccormick.com";
            emailComposeTask.Subject = "Hacker News Feedback";
            emailComposeTask.Body = "Version " + App.ExtendedVersionNumber + " (" + App.PlatformVersionNumber + ")\n\n";
            emailComposeTask.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {

        }

        private void ToggleLoadingText()
        {
            this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstTopPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstNewPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstAskPosts.Visibility = System.Windows.Visibility.Visible;
        }

        private void ResetEmptyText()
        {
            if (this.txtTopPostsEmpty.Visibility == System.Windows.Visibility.Visible)
            {
                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Visible;
            }

            if (this.txtNewPostsEmpty.Visibility == System.Windows.Visibility.Visible)
            {
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Visible;
            }

            if (this.txtAskPostsEmpty.Visibility == System.Windows.Visibility.Visible)
            {
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ToggleEmptyText()
        {
            if (this.TopPosts.Count == 0)
                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (this.NewPosts.Count == 0)
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (this.AskPosts.Count == 0)
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ListBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (topLoaded &&
                newLoaded &&
                askLoaded)
            {
                ToggleLoadingText();
                ToggleEmptyText();

                GlobalLoading.Instance.IsLoading = false;
            }
        }
    }
}