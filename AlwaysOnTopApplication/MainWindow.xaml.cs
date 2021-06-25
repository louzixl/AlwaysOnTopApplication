using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace AlwaysOnTopApplication
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // 使用方案一
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null)
                source.AddHook(WndProc);

            // 使用方案二
            // _ = SetTopMostLoop();

            // 使用方案三
            //_mouseHook = new MouseHook();
            //_mouseHook.OnMouseActivity += MouseHook_OnMouseActivity;
            //_mouseHook.Start();
        }

        /// <summary>
        /// 方案一：捕获WM_WINDOWPOSCHANGING消息，若无SWP_NOZORDER标志，则置顶
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32Api.WM_WINDOWPOSCHANGING:
                    Win32Api.WINDOWPOS wp = (Win32Api.WINDOWPOS)Marshal.PtrToStructure(
                        lParam, typeof(Win32Api.WINDOWPOS));
                    if ((wp.flags & Win32Api.SWP_NOZORDER) == 0)
                        _ = SetTopMostLater();
                    break;
            }

            return IntPtr.Zero;
        }

        private async Task SetTopMostLater()
        {
            await Task.Delay(300);
            var interopHelper = new WindowInteropHelper(this);
            Win32Api.SetWindowPos(interopHelper.Handle, Win32Api.HWND_TOPMOST, 0, 0, 0, 0, Win32Api.TOPMOST_FLAGS);
        }

        /// <summary>
        /// 方案二：循环置顶
        /// </summary>
        /// <returns></returns>
        private async Task SetTopMostLoop()
        {
            while (true)
            {
                await Task.Delay(2000);
                var interopHelper = new WindowInteropHelper(this);
                Win32Api.SetWindowPos(interopHelper.Handle, Win32Api.HWND_TOPMOST, 0, 0, 0, 0, Win32Api.TOPMOST_FLAGS);
            }
        }

        // 方案三：当鼠标按下时置顶
        private void MouseHook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("mouse down......");
            _ = SetTopMostLater();
        }

        private MouseHook _mouseHook;
    }
}
