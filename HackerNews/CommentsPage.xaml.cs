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

        public static readonly DependencyProperty CommentsProperty =
               DependencyProperty.Register("Comments", typeof(ObservableCollection<Comment>), typeof(CommentsPage), new PropertyMetadata(new ObservableCollection<Comment>()));

        public ObservableCollection<Comment> Comments
        {
            get { return (ObservableCollection<Comment>)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        #endregion

        public CommentsPage()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            GlobalLoading.Instance.IsLoadingText("Loading...");

            SmartDispatcher.BeginInvoke(() =>
            {
                this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });

            ServiceClient.GetComments((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

                    if (result != null &&
                        result.items != null)
                    {
                        this.Comments.Clear();

                        foreach (Comment item in result.items)
                        {
                            this.Comments.Add(item);
                        }

                        this.Comments.RemoveAt(this.Comments.Count - 1);
                    }
                    else
                    {
                        this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    GlobalLoading.Instance.IsLoading = false;
                });
            }, "123456");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}