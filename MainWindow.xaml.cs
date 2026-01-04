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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using Org.BouncyCastle.Asn1.X509;


namespace Bongo
{
    public partial class MainWindow : Window
    {
        int userId;
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





        public MainWindow(int id)
        {
            userId = id;

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

            newTab.Loaded += (s, ev) =>
            {
                var tabItem = (TabItem)s;
                if (tabItem == null) return;

                // Find the actual header content presenter in the TabControl visual tree
                var header = TabControl.ItemContainerGenerator.ContainerFromItem(tabItem);
                if (header == null) return;

                var btn = FindChild<Button>(header, "CloseTabBtn");
                if (btn != null)
                {
                    btn.Click += TabHeader_Close_Click;
                }
            };
        }

        public static T FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T element && element.Name == name)
                    return element;

                var result = FindChild<T>(child, name);
                if (result != null) return result;
            }

            return null;
        }

        public static T FindParent<T>(DependencyObject child) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                var typed = parent as T;
                if (typed != null)
                    return typed;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }


        private void TabHeader_Close_Click(object sender, RoutedEventArgs e)
        {
            var tab = FindParent<TabItem>(sender as DependencyObject);
            CloseTab(tab);
        }


        private void DragBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TabControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var tab = FindParent<TabItem>(e.OriginalSource as DependencyObject);
                if (tab != null && !ReferenceEquals(tab, NewTabBtn))
                {
                    CloseTab(tab);
                    e.Handled = true;
                }
            }
        }

        private void CloseTab(TabItem tab)
        {
            if (tab == null) return;

            int tabIndex = TabControl.Items.IndexOf(tab);

            // Store the width before removing so it does NOT shrink
            double storedWidth = tab.Width;

            TabControl.Items.Remove(tab);

            // Restore width if needed (optional safety)
            if (tabIndex > 0 && tabIndex < TabControl.Items.Count)
            {
                var previousTab = TabControl.Items[tabIndex - 1] as TabItem;
                if (previousTab != null)
                {
                    previousTab.Width = storedWidth;
                    TabControl.SelectedItem = previousTab;
                }
            }
            else
            {
                // Fallback: select last real tab before "+"
                if (TabControl.Items.Count > 1)
                {
                    TabControl.SelectedItem = TabControl.Items[TabControl.Items.Count - 2];
                }
            }
        }

        private void TabHeaderScroll_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sv = sender as ScrollViewer;
            if (sv != null)
            {
                double newOffset = sv.HorizontalOffset - e.Delta;
                sv.ScrollToHorizontalOffset(newOffset);
            }
            e.Handled = true;
        }
        public void OpenChromeMenu(UIElement target)
        {
            ChromeMenuPopup.PlacementTarget = target;
            ChromeMenuPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            ChromeMenuPopup.HorizontalOffset = 0;
            ChromeMenuPopup.VerticalOffset = 4;
            ChromeMenuPopup.IsOpen = true;
        }

    }
}