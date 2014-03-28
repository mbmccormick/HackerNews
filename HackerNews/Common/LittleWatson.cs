using System;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

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
                                FeedbackHelper.Default.Feedback(contents, true);

                                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());

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