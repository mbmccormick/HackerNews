using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HackerNews.API.Models;
using Microsoft.Phone.Tasks;
using System.Windows.Media;

namespace HackerNews
{
    public partial class PostControl : UserControl
    {
        public PostControl()
        {
            InitializeComponent();

            this.Loaded += PostControl_Loaded;
        }

        private void PostControl_Loaded(object sender, RoutedEventArgs e)
        {
            Post item = this.DataContext as Post;

            if (App.HackerNewsClient.PostHistory.Contains(item.id) == true)
            {
                this.txtTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
                this.txtDescription.Foreground = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
                this.pthCommentCount.Fill = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }
        }

        private void PostControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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
                App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.id, UriKind.Relative));
            }

            App.HackerNewsClient.MarkPostAsRead(item.id);

            this.txtTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
            this.txtDescription.Foreground = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
            this.pthCommentCount.Fill = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
        }

        private void CommentControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            if (item.type == "job") return;

            if (item.type == "ask")
            {
                App.HackerNewsClient.MarkPostAsRead(item.id);

                this.txtTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
                this.txtDescription.Foreground = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
                this.pthCommentCount.Fill = new SolidColorBrush(Color.FromArgb(255, 222, 222, 222));
            }

            App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.id, UriKind.Relative));
        }
    }
}
