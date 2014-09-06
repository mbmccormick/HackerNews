using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HackerNews.API.Models;

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
        }
    }
}