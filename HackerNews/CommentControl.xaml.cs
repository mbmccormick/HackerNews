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
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using HackerNews.Models;

namespace HackerNews
{
    public partial class CommentControl : UserControl
    {
        public CommentControl()
        {
            InitializeComponent();

            this.Loaded += CommentControl_Loaded;
        }

        private void CommentControl_Loaded(object sender, RoutedEventArgs e)
        {
            CommentItem item = this.DataContext as CommentItem;

            this.LayoutRoot.Margin = new Thickness(12 * item.level, 0, 0, 0);

            // SetLinkedText(this.txtRichContent, item.content);

            // this.txtPlainContent.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Hyperlink_Click(object sender, NavigationEventArgs e)
        {
            Hyperlink item = sender as Hyperlink;

            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(item.NavigateUri.AbsoluteUri, UriKind.Absolute);

            webBrowserTask.Show();
        }
    }
}