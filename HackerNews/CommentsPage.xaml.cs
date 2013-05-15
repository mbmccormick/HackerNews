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

        public static ObservableCollection<Comment> Comments { get; set; }

        #endregion

        #region Item Properties

        public static Post CurrentPost { get; set; }

        #endregion

        public CommentsPage()
        {
            InitializeComponent();

            Comments = new ObservableCollection<Comment>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                GlobalLoading.Instance.IsLoading = true;

                if (PostsPage.TopPosts.Where(z => z.item_id == id).Count() > 0)
                    CurrentPost = PostsPage.TopPosts.Single<Post>(z => z.item_id == id);

                if (PostsPage.NewPosts.Where(z => z.item_id == id).Count() > 0)
                    CurrentPost = PostsPage.NewPosts.Single<Post>(z => z.item_id == id);

                if (PostsPage.AskPosts.Where(z => z.item_id == id).Count() > 0)
                    CurrentPost = PostsPage.AskPosts.Single<Post>(z => z.item_id == id);

                this.txtTitle.Text = CurrentPost.title;
                this.txtBody.Text = CurrentPost.longBody;

                App.HackerNewsClient.GetComments((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        if (result != null &&
                            result.items != null)
                        {
                            Comments.Clear();

                            foreach (Comment item in result.items)
                            {
                                Comments.Add(item);
                            }
                        }

                        ToggleLoadingText();
                        ToggleEmptyText();

                        GlobalLoading.Instance.IsLoading = false;
                    });
                }, id);
            }
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
            SmartDispatcher.BeginInvoke(() =>
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

                this.lstComments.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void ToggleEmptyText()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                if (Comments.Count == 0)
                    this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                else
                    this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
            });
        }
    }
}