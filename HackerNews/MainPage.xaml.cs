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
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static readonly DependencyProperty TopItemsProperty =
               DependencyProperty.Register("TopItems", typeof(ObservableCollection<Item>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Item>()));

        public ObservableCollection<Item> TopItems
        {
            get { return (ObservableCollection<Item>)GetValue(TopItemsProperty); }
            set { SetValue(TopItemsProperty, value); }
        }

        public static readonly DependencyProperty NewItemsProperty =
               DependencyProperty.Register("NewItems", typeof(ObservableCollection<Item>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Item>()));

        public ObservableCollection<Item> NewItems
        {
            get { return (ObservableCollection<Item>)GetValue(NewItemsProperty); }
            set { SetValue(NewItemsProperty, value); }
        }

        public static readonly DependencyProperty AskItemsProperty =
               DependencyProperty.Register("AskItems", typeof(ObservableCollection<Item>), typeof(MainPage), new PropertyMetadata(new ObservableCollection<Item>()));

        public ObservableCollection<Item> AskItems
        {
            get { return (ObservableCollection<Item>)GetValue(AskItemsProperty); }
            set { SetValue(AskItemsProperty, value); }
        }

        #endregion

        private bool topLoaded = false;
        private bool newLoaded = false;
        private bool askLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            topLoaded = false;
            newLoaded = false;
            askLoaded = false;

            GlobalLoading.Instance.IsLoading = true;

            SmartDispatcher.BeginInvoke(() =>
            {
                TopItems.Clear();
                NewItems.Clear();
                AskItems.Clear();

                this.txtTopItemsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtNewItemsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.txtAskItemsEmpty.Visibility = System.Windows.Visibility.Collapsed;

                this.txtTopItemsLoading.Visibility = System.Windows.Visibility.Visible;
                this.txtNewItemsLoading.Visibility = System.Windows.Visibility.Visible;
                this.txtAskItemsLoading.Visibility = System.Windows.Visibility.Visible;
            });

            ServiceClient.GetTopItems((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtTopItemsLoading.Visibility = System.Windows.Visibility.Collapsed;

                    if (result != null)
                    {
                        this.TopItems.Clear();

                        foreach (Item item in result.items)
                        {
                            this.TopItems.Add(item);
                        }
                    }
                    else
                    {
                        this.txtTopItemsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    topLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });

            ServiceClient.GetNewItems((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtNewItemsLoading.Visibility = System.Windows.Visibility.Collapsed;

                    if (result != null)
                    {
                        this.NewItems.Clear();

                        foreach (Item item in result.items)
                        {
                            this.NewItems.Add(item);
                        }
                    }
                    else
                    {
                        this.txtNewItemsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    newLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });

            ServiceClient.GetAskItems((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.txtAskItemsLoading.Visibility = System.Windows.Visibility.Collapsed;

                    if (result != null)
                    {
                        this.AskItems.Clear();

                        foreach (Item item in result.items)
                        {
                            this.AskItems.Add(item);
                        }
                    }
                    else
                    {
                        this.txtAskItemsEmpty.Visibility = System.Windows.Visibility.Visible;
                    }

                    askLoaded = true;

                    if (topLoaded && newLoaded && askLoaded)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });
        }

        private void ItemContent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Item item = ((FrameworkElement)sender).DataContext as Item;

            if (item.url.StartsWith("http") == true)
            {
                WebBrowserTask browser = new WebBrowserTask();
                browser.Uri = new Uri(item.url);

                browser.Show();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}