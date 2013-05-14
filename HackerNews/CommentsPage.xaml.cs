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

        public ObservableCollection<Comment> Comments { get; set; }

        #endregion

        public CommentsPage()
        {
            InitializeComponent();

            this.Comments = new ObservableCollection<Comment>();

            this.Loaded += CommentsPage_Loaded;
        }

        private void CommentsPage_Loaded(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                GlobalLoading.Instance.IsLoading = true;

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
                        }
                        else
                        {
                            this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                        }

                        this.lstComments.Visibility = System.Windows.Visibility.Visible;

                        GlobalLoading.Instance.IsLoading = false;
                    });
                }, id);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}