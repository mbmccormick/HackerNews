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
using Windows.ApplicationModel.Store;

namespace HackerNews
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Post> TopPosts { get; set; }
        public static ObservableCollection<Post> NewPosts { get; set; }
        public static ObservableCollection<Post> AskPosts { get; set; }

        #endregion

        private bool isTopLoaded = false;
        private bool isNewLoaded = false;
        private bool isAskLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            App.UnhandledExceptionHandled += new EventHandler<ApplicationUnhandledExceptionEventArgs>(App_UnhandledExceptionHandled);

            TopPosts = new ObservableCollection<Post>();
            NewPosts = new ObservableCollection<Post>();
            AskPosts = new ObservableCollection<Post>();
        }

        private void App_UnhandledExceptionHandled(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ToggleLoadingText();
                ToggleEmptyText();

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                LittleWatson.CheckForPreviousException(true);

                if (isTopLoaded == false ||
                    isNewLoaded == false ||
                    isAskLoaded == false)
                {
                    LoadData();
                }

                FeedbackHelper.PromptForRating();
            }
        }

        private async void LoadData()
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            await App.HackerNewsClient.GetTopPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    TopPosts.Clear();

                    foreach (Post item in result)
                    {
                        TopPosts.Add(item);
                    }

                    isTopLoaded = true;

                    if (isTopLoaded &&
                        isNewLoaded &&
                        isAskLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });

            await App.HackerNewsClient.GetNewPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    NewPosts.Clear();

                    foreach (Post item in result)
                    {
                        NewPosts.Add(item);
                    }

                    isNewLoaded = true;

                    if (isTopLoaded &&
                        isNewLoaded &&
                        isAskLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });

            await App.HackerNewsClient.GetAskPosts((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    AskPosts.Clear();

                    foreach (Post item in result)
                    {
                        AskPosts.Add(item);
                    }

                    isAskLoaded = true;

                    if (isTopLoaded &&
                        isNewLoaded &&
                        isAskLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });
        }

        private void ToggleLoadingText()
        {
            this.txtTopPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtNewPostsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtAskPostsLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstTopPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstNewPosts.Visibility = System.Windows.Visibility.Visible;
            this.lstAskPosts.Visibility = System.Windows.Visibility.Visible;
        }

        private void ToggleEmptyText()
        {
            if (TopPosts.Count == 0)
                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtTopPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (NewPosts.Count == 0)
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtNewPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (AskPosts.Count == 0)
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtAskPostsEmpty.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            isTopLoaded = false;
            isNewLoaded = false;
            isAskLoaded = false;

            LoadData();
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

        private int _offsetKnob = 5;

        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            LongListSelector target = (LongListSelector)sender;

            if (target.ItemsSource != null &&
                target.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as Post).Equals(target.ItemsSource[target.ItemsSource.Count - _offsetKnob]))
                    {
                        this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                        if (target == this.lstTopPosts)
                        {
                            App.HackerNewsClient.GetNextTopPosts((result) =>
                            {
                                SmartDispatcher.BeginInvoke(() =>
                                {
                                    foreach (Post item in result)
                                    {
                                        TopPosts.Add(item);
                                    }

                                    this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                                });
                            });
                        }
                        else if (target == this.lstNewPosts)
                        {
                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else if (target == this.lstAskPosts)
                        {
                            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void LongListSelector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
        }
    }
}