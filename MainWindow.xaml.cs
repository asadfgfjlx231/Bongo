using CefSharp;
using CefSharp.Wpf;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;


namespace Bongo
{
    public partial class MainWindow : Window
    {
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);

                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;

                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;

            /// <summary>y coordinate of point.</summary>
            public int y;

            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();

            public int Width { get { return Math.Abs(right - left); } }

            public int Height { get { return bottom - top; } }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty { get { return left >= right || top >= bottom; } }

            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not guaranteed to be unique)</summary>
            public override int GetHashCode() =>
                left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();

            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return rect1.left == rect2.left &&
                       rect1.top == rect2.top &&
                       rect1.right == rect2.right &&
                       rect1.bottom == rect2.bottom;
            }

            /// <summary> Determine if 2 RECT are different (deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);





        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            MinimizeBtn.Click += (s, e) => WindowState = WindowState.Minimized;
            MaximizeBtn.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            
            CloseBtn.Click += (s, e) => this.Close();
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));


            Browser.FrameLoadEnd += (sender, args) =>
            {
                args.Frame.ExecuteJavaScriptAsync("window.onerror = ()=>true;");
                args.Frame.ExecuteJavaScriptAsync("console.error = ()=>{};");
                args.Frame.ExecuteJavaScriptAsync("console.warn = ()=>{};");
            };

            Browser.AddressChanged += Browser_AddressChanged;
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern long DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute,
                                                        ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);


        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3,
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

        private void Window_StateChanged(object sender, EventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            MainWindow Current = this;

            if (Current.WindowState != WindowState.Maximized)
            {
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }
            else
            {
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }

        }

        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var newTab = new TabItem
            {
                Header = "Új lap",
                HeaderTemplate = (DataTemplate)Application.Current.Resources["TabHeaderTemplate"],
                Content = new TabContent()
            };

            TabControl.Items.Insert(TabControl.Items.Count - 1, newTab);
            TabControl.SelectedItem = newTab;
        }

        private void DragBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }

}
