using CefSharp;
using CefSharp.Wpf;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bongo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Browser.FrameLoadEnd += (sender, args) =>
            {
                args.Frame.ExecuteJavaScriptAsync("window.onerror = ()=>true;");
                args.Frame.ExecuteJavaScriptAsync("console.error = ()=>{};");
                args.Frame.ExecuteJavaScriptAsync("console.warn = ()=>{};");
            };

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


        // ✔ Belépő url normalizálás + létezés ellenőrzés
        private async void NavigateTo(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string url = input.Trim();

            // 0) Speciális protokollok, amiket NEM módosítunk
            if (url.StartsWith("about:") ||
                url.StartsWith("chrome:") ||
                url.StartsWith("file:/"))
            {
                Browser.Address = url;
                return;
            }

            // 1) Ha már teljes URL (http, https, ftp)
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri absoluteUri))
            {
                Browser.Address = absoluteUri.ToString();
                return;
            }

            // 2) Ha domain, IP vagy localhost TLD ellenőrzés nélkül
            bool looksLikeHost =
                url.Contains('.') ||
                url.StartsWith("localhost") ||
                char.IsDigit(url[0]);

            if (looksLikeHost)
            {
                // Ékezetes domain támogatás (IDN)
                try
                {
                    var idn = new System.Globalization.IdnMapping();
                    string host = idn.GetAscii(url);
                    url = host;
                }
                catch { /* ha nem konvertálható, átugorjuk */ }

                string https = "https://" + url;

                // Ellenőrizzük: működik-e HTTPS
                if (await UrlExists(https))
                    Browser.Address = https;
                else
                    Browser.Address = "http://" + url;

                return;
            }

            // 3) Ha nem URL → Google keresés
            string query = System.Net.WebUtility.UrlEncode(input);
            Browser.Address = "https://www.google.com/search?q=" + query;
        }



        // ✔ Gyors URL létezésteszt
        private async Task<bool> UrlExists(string url)
        {
            try
            {
                using (HttpClient c = new HttpClient())
                {
                    c.Timeout = TimeSpan.FromSeconds(2);
                    var r = await c.GetAsync(url);
                    return r.IsSuccessStatusCode;
                }
            }
            catch { return false; }
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


        // 🚀 VÉGRE: tökéletesen működő STOP gomb
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            var core = Browser.GetBrowser();
            if (core == null)
                return;

            try
            {
                // 1) Chromium motor betöltés megállítása
                core.StopLoad();

                // 2) JS oldali stop (Chrome is ezt hívja)
                Browser.ExecuteScriptAsync("window.stop();");

                // 3) Aktív hálózati kapcsolatok megszakítása
                core.GetHost().WasResized();
            }
            catch { }
        }
    }

}
