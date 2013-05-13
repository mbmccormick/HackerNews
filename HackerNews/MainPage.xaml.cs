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

        public static readonly DependencyProperty TopPostsProperty =
               DependencyProperty.Register("TopPosts", typeof(ObservableCollection<Post>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Post>()));

        public ObservableCollection<Post> TopPosts
        {
            get { return (ObservableCollection<Post>)GetValue(TopPostsProperty); }
            set { SetValue(TopPostsProperty, value); }
        }

        public static readonly DependencyProperty NewPostsProperty =
               DependencyProperty.Register("NewPosts", typeof(ObservableCollection<Post>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Post>()));

        public ObservableCollection<Post> NewPosts
        {
            get { return (ObservableCollection<Post>)GetValue(NewPostsProperty); }
            set { SetValue(NewPostsProperty, value); }
        }

        public static readonly DependencyProperty AskPostsProperty =
               DependencyProperty.Register("AskPosts", typeof(ObservableCollection<Post>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Post>()));

        public ObservableCollection<Post> AskPosts
        {
            get { return (ObservableCollection<Post>)GetValue(AskPostsProperty); }
            set { SetValue(AskPostsProperty, value); }
        }

        #endregion

        private bool topLoaded = false;
        private bool newLoaded = false;
        private bool askLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Visible;
            this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Visible;
            this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Visible;

            LoadData();
        }

        private void LoadData()
        {
            topLoaded = false;
            newLoaded = false;
            askLoaded = false;

            GlobalLoading.Instance.IsLoadingText("Loading...");

            SmartDispatcher.BeginInvoke(() =>
            {
                if (this.txtTopPostsEmpty.Visibility == System.Windows.Visibility.Visible)
                    this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Visible;

                if (this.txtNewPostsEmpty.Visibility == System.Windows.Visibility.Visible)
                    this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Visible;

                if (this.txtAskPostsEmpty.Visibility == System.Windows.Visibility.Visible)
                    this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Visible;

                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });

            ServiceClient.GetTopPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Collapsed;

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
                    else
                    {
                        this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    topLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });

            ServiceClient.GetNewPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Collapsed;

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
                    else
                    {
                        this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    newLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });

            ServiceClient.GetAskPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Collapsed;

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
                    else
                    {
                        this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    askLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });
        }

        private void ItemContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            if (item.url.StartsWith("http") == true)
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(item.url);

                browser.Show();
            }
            else
            {
                NavigationService.Navigate(new Uri("/CommentsPage.xaml?id=" + item.item_id, UriKind.Relative));
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}