/**
 * Copyright (c) 2013 Nokia Corporation. All rights reserved.
 *
 * Nokia, Nokia Connecting People, Nokia Developer, and HERE are trademarks
 * and/or registered trademarks of Nokia Corporation. Other product and company
 * names mentioned herein may be trademarks or trade names of their respective
 * owners.
 *
 * See the license text file delivered with this project for more information.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;

namespace HackerNews.Common
{
    public enum FeedbackState
    {
        Inactive = 0,
        Active,
        FirstReview,
        SecondReview,
        Feedback
    }

    /// <summary>
    /// This helper class controls the behaviour of the FeedbackOverlay control.
    /// When the app has been launched FirstCount times the initial prompt is shown.
    /// If the user reviews no more prompts are shown. When the app has been
    /// launched SecondCount times and not been reviewed, the prompt is shown.
    /// </summary>
    public class FeedbackHelper : INotifyPropertyChanged
    {
        // Constants
        private const string LaunchCountKey = "RATE_MY_APP_LAUNCH_COUNT";
        private const string ReviewedKey = "RATE_MY_APP_REVIEWED";
        private const string LastLaunchDateKey = "RATE_MY_APP_LAST_LAUNCH_DATE";

        // Members
        private int firstCount = 5;
        private int secondCount = 10;
        private FeedbackState state;
        private int launchCount = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        public static readonly FeedbackHelper Default = new FeedbackHelper();
        private bool reviewed = false;
        private DateTime lastLaunchDate = new DateTime();

        public DateTime LastLaunchDate
        {
            get { return lastLaunchDate; }
            internal set
            {
                lastLaunchDate = value;
                OnPropertyChanged("LastLaunchDate");
            }
        }

        public bool IsReviewed
        {
            get { return reviewed; }
            internal set
            {
                reviewed = value;
                OnPropertyChanged("IsReviewed");
            }
        }

        public FeedbackState State
        {
            get { return state; }
            internal set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        public int LaunchCount
        {
            get { return launchCount; }
            internal set
            {
                launchCount = value;
                OnPropertyChanged("LaunchCount");
            }
        }

        public int FirstCount
        {
            get { return firstCount; }
            internal set
            {
                firstCount = value;
                OnPropertyChanged("FirstCount");
            }
        }

        public int SecondCount
        {
            get { return secondCount; }
            internal set
            {
                secondCount = value;
                OnPropertyChanged("SecondCount");
            }
        }

        public bool CountDays
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private FeedbackHelper()
        {
            State = FeedbackState.Active;
        }

        /// <summary>
        /// Called when FeedbackLayout control is instantiated, which is
        /// supposed to happen when application's main page is instantiated.
        /// </summary>
        public void Launching()
        {
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();

            // Only load state if app is not trial, app is not activated after
            // being tombstoned, and state has not been loaded before.
            if (!license.IsTrial() &&
                PhoneApplicationService.Current.StartupMode == StartupMode.Launch &&
                State == FeedbackState.Active)
            {
                LoadData();
            }

            // Uncomment for testing
            // State = FeedbackState.FirstReview;
            // State = FeedbackState.SecondReview;
        }

        /// <summary>
        /// Call when user has reviewed.
        /// </summary>
        public void Reviewed()
        {
            IsReviewed = true;

            SaveData();
        }

        /// <summary>
        /// Reset review and feedback launch counter and review state.
        /// </summary>
        public void Reset()
        {
            LaunchCount = 0;
            IsReviewed = false;
            LastLaunchDate = DateTime.Now;

            SaveData();
        }

        /// <summary>
        /// Loads last state from storage and works out the new state.
        /// </summary>
        private void LoadData()
        {
            try
            {
                LaunchCount = IsolatedStorageHelper.GetObject<int>(LaunchCountKey);
                IsReviewed = IsolatedStorageHelper.GetObject<bool>(ReviewedKey);
                LastLaunchDate = IsolatedStorageHelper.GetObject<DateTime>(LastLaunchDateKey);

                if (!reviewed)
                {
                    if (!CountDays || lastLaunchDate.Date < DateTime.Now.Date)
                    {
                        LaunchCount++;
                        LastLaunchDate = DateTime.Now;
                    }

                    if (LaunchCount == FirstCount)
                    {
                        State = FeedbackState.FirstReview;
                    }
                    else if (LaunchCount == SecondCount)
                    {
                        State = FeedbackState.SecondReview;
                    }

                    SaveData();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.LoadState - Failed to load state, Exception: {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Stores current state.
        /// </summary>
        private void SaveData()
        {
            try
            {
                IsolatedStorageHelper.SaveObject(LaunchCountKey, LaunchCount);
                IsolatedStorageHelper.SaveObject(ReviewedKey, reviewed);
                IsolatedStorageHelper.SaveObject(LastLaunchDateKey, lastLaunchDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("FeedbackHelper.StoreState - Failed to store state, Exception: {0}", ex.ToString()));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Review()
        {
            Reviewed();

            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
        }

        public void Feedback()
        {
            Feedback("[Your feedback here]", false);
        }

        public void Feedback(string contents, bool error)
        {
            string version = string.Empty;

            var appManifestResourceInfo = Application.GetResourceStream(new Uri("WMAppManifest.xml", UriKind.Relative));

            using (var appManifestStream = appManifestResourceInfo.Stream)
            {
                using (var reader = XmlReader.Create(appManifestStream, new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true }))
                {
                    var doc = XDocument.Load(reader);
                    var app = doc.Descendants("App").FirstOrDefault();
                    if (app != null)
                    {
                        var versionAttribute = app.Attribute("Version");
                        if (versionAttribute != null)
                        {
                            version = versionAttribute.Value;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(version))
            {
                // Application version
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var parts = asm.FullName.Split(',');
                version = parts[1].Split('=')[1];
            }

            // Body text including hardware, firmware and software info
            string body = string.Format(contents + "\n\n---------------------------------\nDevice Name: {0}\nDevice Manufacturer: {1}\nDevice Firmware Version: {2}\nDevice Hardware Version: {3}\nApplication Version: {4}\n---------------------------------\n\nNote: This e-mail exchange is governed by {5}’s privacy policy. You can find more details on the About page in the application.",
                 DeviceStatus.DeviceName,
                 DeviceStatus.DeviceManufacturer,
                 DeviceStatus.DeviceFirmwareVersion,
                 DeviceStatus.DeviceHardwareVersion,
                 version,
                 "Hacker News");

            // Email task
            var email = new EmailComposeTask();
            email.To = App.FeedbackEmailAddress;
            email.Subject = error ? "Hacker News Error Report" : "Hacker News Feedback";
            email.Body = body;

            email.Show();
        }

        public static void PromptForRating()
        {
            FeedbackHelper.Default.Launching();

            // Check if review/feedback notification should be shown.
            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                FeedbackHelper.ShowRating();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.SecondReview)
            {
                FeedbackHelper.ShowRating();
            }
            else
            {
                FeedbackHelper.Default.State = FeedbackState.Inactive;
            }
        }

        public static void ShowRating()
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Rate Hacker News",
                Message = "It looks like you are enjoying Hacker News. Would you like to give it a rating on the Windows Phone Store?",
                LeftButtonContent = "give rating",
                RightButtonContent = "no thanks",
                IsFullScreen = false
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        FeedbackHelper.Default.Review();
                        FeedbackHelper.Default.State = FeedbackState.Inactive;

                        break;
                    default:
                        FeedbackHelper.ShowFeedback();

                        break;
                }
            };

            messageBox.Show();
        }

        public static void ShowFeedback()
        {
            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                FeedbackHelper.Default.State = FeedbackState.Feedback;

                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "Feedback",
                    Message = "Sorry to hear you didn't want to give Hacker News a rating on the Store. Would you like to send some feedback on how things could be better?",
                    LeftButtonContent = "give feedback",
                    RightButtonContent = "no thanks",
                    IsFullScreen = false
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            FeedbackHelper.Default.Feedback();
                            FeedbackHelper.Default.State = FeedbackState.Inactive;

                            break;
                        default:
                            FeedbackHelper.Default.State = FeedbackState.Inactive;

                            break;
                    }
                };

                messageBox.Show();
            }
        }
    }
}