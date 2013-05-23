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

            if (item.type == "job")
                this.stkCommentCount.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PostControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            if (item.type == "link" ||
                item.type == "job")
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(item.url);

                webBrowserTask.Show();
            }
            else
            {
                App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.id, UriKind.Relative));
            }

            App.HackerNewsClient.MarkPostAsRead(item.id);
            item.is_read = true;
        }

        private void CommentControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            if (item.type == "job") return;

            if (item.type == "ask")
            {
                App.HackerNewsClient.MarkPostAsRead(item.id);
                item.is_read = true;
            }

            App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.id, UriKind.Relative));
        }
    }
}
