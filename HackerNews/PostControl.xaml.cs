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

            if (App.HackerNewsClient.PostHistory.Contains(item.item_id) == true)
            {
                this.txtTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
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
                App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.item_id, UriKind.Relative));
            }

            App.HackerNewsClient.MarkPostAsRead(item.item_id);

            this.txtTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
        }

        private void CommentControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post item = ((FrameworkElement)sender).DataContext as Post;

            App.RootFrame.Navigate(new Uri("/CommentsPage.xaml?id=" + item.item_id, UriKind.Relative));
        }
    }
}
