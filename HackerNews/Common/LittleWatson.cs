using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;

namespace HackerNews.Common
{
    public class LittleWatson
    {
        const string filename = "LittleWatson.txt";

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    SafeDeleteFile(store);
                    using (TextWriter output = new StreamWriter(store.CreateFile(filename)))
                    {
                        output.WriteLine(extra);
                        output.WriteLine();
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal static void CheckForPreviousException(bool isFirstRun)
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(filename))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(filename, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }
                        SafeDeleteFile(store);
                    }
                }

                if (contents != null)
                {
                    string messageBoxText = null;
                    if (isFirstRun)
                        messageBoxText = "An unhandled error occurred the last time you ran this application. Would you like to send an email to report it?";
                    else
                        messageBoxText = "An unhandled error has just occurred. Would you like to send an email to report it?";

                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = "Error Report",
                        Message = messageBoxText,
                        LeftButtonContent = "yes",
                        RightButtonContent = "no",
                        IsFullScreen = false
                    };

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                EmailComposeTask email = new EmailComposeTask();
                                email.To = App.FeedbackEmailAddress;
                                email.Subject = "Hacker News Error Report";
                                email.Body = "Version " + App.ExtendedVersionNumber + " (" + App.PlatformVersionNumber + ")\n" + contents;

                                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());

                                email.Show();

                                break;
                            default:
                                break;
                        }
                    };

                    messageBox.Show();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(filename);
            }
            catch (Exception)
            {
            }
        }
    }
}