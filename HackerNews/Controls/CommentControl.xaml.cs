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
            Comment item = this.DataContext as Comment;
            
            if (item.level == 0)
            {
                this.LayoutRoot.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}