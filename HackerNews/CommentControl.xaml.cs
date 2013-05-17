using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HackerNews.API.Models;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace HackerNews
{
    public partial class CommentControl : UserControl
    {
        public CommentControl()
        {
            InitializeComponent();

            this.Loaded += CommentControl_Loaded;
        }

        private void CommentControl_Loaded(object sender, RoutedEventArgs e)
        {
            Comment item = this.DataContext as Comment;

            SetLinkedText(this.txtContent, item.content);
        }

        private static void SetLinkedText(RichTextBox richTextBox, string htmlFragment)
        {
            var regEx = new Regex(@"\<a\s(href\=""|[^\>]+?\shref\="")(?<link>[^""]+)"".*?\>(?<text>.*?)(\<\/a\>|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            richTextBox.Blocks.Clear();

            int nextOffset = 0;

            foreach (Match match in regEx.Matches(htmlFragment))
            {
                if (match.Index >= nextOffset)
                {
                    AppendText(richTextBox, htmlFragment.Substring(nextOffset, match.Index - nextOffset));
                    nextOffset = match.Index + match.Length;
                    AppendLink(richTextBox, match.Groups["text"].Value, new Uri(match.Groups["link"].Value, UriKind.Absolute));
                }
            }

            if (nextOffset < htmlFragment.Length)
            {
                AppendText(richTextBox, htmlFragment.Substring(nextOffset));
            }
        }

        private static void AppendText(RichTextBox richTextBox, string text)
        {
            Paragraph paragraph;

            if (richTextBox.Blocks.Count == 0 ||
                (paragraph = richTextBox.Blocks[richTextBox.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                richTextBox.Blocks.Add(paragraph);
            }

            paragraph.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));

            paragraph.Inlines.Add(new Run { Text = text });
        }

        private static void AppendLink(RichTextBox richTextBox, string text, Uri uri)
        {
            Paragraph paragraph;

            if (richTextBox.Blocks.Count == 0 ||
                (paragraph = richTextBox.Blocks[richTextBox.Blocks.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                richTextBox.Blocks.Add(paragraph);
            }

            paragraph.Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));

            var run = new Run { Text = text };
            var link = new Hyperlink { NavigateUri = uri };

            link.Click += Hyperlink_Click;
            link.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 102, 0));
            link.MouseOverForeground = new SolidColorBrush(Color.FromArgb(255, 255, 102, 0));
            
            link.Inlines.Add(run);
            paragraph.Inlines.Add(link);
        }

        private static void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink item = sender as Hyperlink;

            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(item.NavigateUri.AbsoluteUri, UriKind.Absolute);

            browser.Show();
        }
    }
}
