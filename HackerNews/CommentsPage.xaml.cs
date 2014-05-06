using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using HackerNews.API.Models;
using HackerNews.Common;
using HackerNews.Models;
using Microsoft.Phone.Controls;
using Windows.ApplicationModel.Store;

namespace HackerNews
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        #region List Properties

        public static CommentResponse CurrentPost { get; set; }

        private static List<CommentItem> CommentsDataSource { get; set; }

        public static ObservableCollection<CommentItem> Comments { get; set; }

        #endregion

        private int maxResults = 25;
        private int currentOffset = 0;

        private bool isLoaded = false;

        public CommentsPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            CommentsDataSource = new List<CommentItem>();
            Comments = new ObservableCollection<CommentItem>();
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.txtTitle.Visibility = System.Windows.Visibility.Collapsed;
                this.txtDescription.Visibility = System.Windows.Visibility.Collapsed;

                ToggleLoadingText();

                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
                this.txtEmpty.Text = "Sorry, could not download comments right now.";

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isLoaded == false)
            {
                LoadData();
            }
        }

        private async void LoadData()
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                ResetDefaultLayout();

                await App.HackerNewsClient.GetComments((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentPost = result;

                        this.txtTitle.Text = CurrentPost.title;
                        this.txtDescription.Text = CurrentPost.description;

                        Comments.Clear();
                        CommentsDataSource.Clear();

                        Flatten(CurrentPost.comments);

                        foreach (CommentItem item in CommentsDataSource.Take(maxResults))
                        {
                            Comments.Add(item);
                            currentOffset++;
                        }

                        isLoaded = true;

                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    });
                }, id);
            }
        }

        private void ResetDefaultLayout()
        {
            this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (Comments.Count == 0)
            {
                this.txtLoading.Visibility = System.Windows.Visibility.Visible;

                this.lstComments.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ToggleLoadingText()
        {
            this.txtLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstComments.Visibility = System.Windows.Visibility.Visible;
        }

        private void ToggleEmptyText()
        {
            if (Comments.Count == 0)
                this.txtEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private static void Flatten(List<Comment> enumerable)
        {
            foreach (Comment item in enumerable)
            {
                CommentsDataSource.Add(new CommentItem(item));

                Flatten(item.comments);
            }
        }

        private void Feedback_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            FeedbackHelper.Default.Feedback();
        }
        private async void Donate_Click(object sender, EventArgs e)
        {
            try
            {
                var productList = await CurrentApp.LoadListingInformationAsync();
                var product = productList.ProductListings.FirstOrDefault(p => p.Value.ProductType == ProductType.Consumable);
                var receipt = await CurrentApp.RequestProductPurchaseAsync(product.Value.ProductId, true);

                if (CurrentApp.LicenseInformation.ProductLicenses[product.Value.ProductId].IsActive)
                {
                    CurrentApp.ReportProductFulfillment(product.Value.ProductId);

                    MessageBox.Show("Thank you for your donation! Your support motivates me to keep developing for Hacker News, the best Hacker News client for Windows Phone.", "Thank You", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                // do nothing
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void lstComments_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (Comments.Count < CommentsDataSource.Count)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    Comments.Add(CommentsDataSource[currentOffset]);
                    currentOffset++;
                }
            }
        }
    }
}