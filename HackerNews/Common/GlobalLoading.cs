using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace HackerNews.Common
{
    public class GlobalLoading : INotifyPropertyChanged
    {
        private ProgressIndicator _mangoIndicator;
        private bool _statusText = false;

        private GlobalLoading()
        {
        }

        public void Initialize(PhoneApplicationFrame frame)
        {
            _mangoIndicator = new ProgressIndicator();

            frame.Navigated += OnRootFrameNavigated;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            var ee = e.Content;
            var pp = ee as PhoneApplicationPage;
            if (pp != null)
            {
                pp.SetValue(SystemTray.ProgressIndicatorProperty, _mangoIndicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                NotifyValueChanged();
            }
        }

        private static GlobalLoading _in;
        public static GlobalLoading Instance
        {
            get
            {
                if (_in == null)
                {
                    _in = new GlobalLoading();
                }

                return _in;
            }
        }

        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        private bool _loading;

        public bool IsLoading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;

                NotifyValueChanged();
            }
        }

        public void IsLoadingText(string loadingText)
        {
            if (_statusText == true) return;

            _mangoIndicator.Text = loadingText;

            IsLoading = true;
        }

        public void StatusText(string loadingText)
        {
            IsLoading = false;

            _mangoIndicator.Text = loadingText;
            _mangoIndicator.IsVisible = true;

            _statusText = true;
        }

        public void ClearStatusText(string loadingText)
        {
            _mangoIndicator.Text = null;

            _statusText = false;
        }

        private void NotifyValueChanged()
        {
            if (_statusText == true) return;

            if (_mangoIndicator != null)
            {
                // _mangoIndicator.IsIndeterminate = _loading || IsDataManagerLoading;

                if (_loading == false)
                    _mangoIndicator.Text = null;

                // for now, just make sure it's always visible.
                if (_mangoIndicator.IsVisible == false)
                {
                    _mangoIndicator.IsVisible = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
