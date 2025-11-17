using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;

namespace Bongo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Browser.AddressChanged += Browser_AddressChanged;
        }

        private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                AddressBar.Text = Browser.Address;
            });
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                NavigateTo(AddressBar.Text);
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(AddressBar.Text);
        }

        // Navigálás
        private void NavigateTo(string input)
        {
            string url = input.Trim();

            if (!url.StartsWith("http"))
                url = "https://www.google.com/search?q=" + System.Net.WebUtility.UrlEncode(url);

            Browser.Address = url;
        }

        // Alapfunkciók
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.Back();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
                Browser.Forward();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Browser.Stop();
            }
            catch
            {
                // ha valamiért nem működik, lesz a fallback
                Browser.ExecuteScriptAsync("if (window.stop) window.stop();");
            }
        }
    }
}
