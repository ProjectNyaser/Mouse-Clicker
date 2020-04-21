using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace 连点器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly Timer timer = new Timer { Enabled = false, Interval = 1 };
        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Tick += (object sender1, EventArgs e1) =>
              {
                  MouseHelper.mouse_event(MouseHelper.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                  MouseHelper.mouse_event(MouseHelper.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
              };
        }
        private const int WM_HOTKEY = 0x312; //窗口消息-热键
        private const int WM_CREATE = 0x1; //窗口消息-创建
        private const int WM_DESTROY = 0x2; //窗口消息-销毁
        private const int Space = 0x3572; //热键ID
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_HOTKEY: //窗口消息-热键ID
                    switch (m.WParam.ToInt32())
                    {
                        case Space: //热键ID
                            timer.Enabled = !timer.Enabled;
                            Text = $"连点器 - F2 - {timer.Enabled}";
                            break;
                        default:
                            break;
                    }
                    break;
                case WM_CREATE: //窗口消息-创建
                    AppHotKey.RegKey(Handle, Space, 0, Keys.F2);
                    break;
                case WM_DESTROY: //窗口消息-销毁
                    AppHotKey.UnRegKey(Handle, Space); //销毁热键
                    break;
                default:
                    break;
            }
        }
        public class AppHotKey
        {
            [DllImport("kernel32.dll")]
            public static extern uint GetLastError();
            //如果函数执行成功，返回值不为0。
            //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(
                IntPtr hWnd,                //要定义热键的窗口的句柄
                int id,                     //定义热键ID（不能与其它ID重复）          
                KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
                Keys vk                     //定义热键的内容
                );

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(
                IntPtr hWnd,                //要取消热键的窗口的句柄
                int id                      //要取消热键的ID
                );

            //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
            [Flags()]
            public enum KeyModifiers
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
            }
            /// <summary>
            /// 注册热键
            /// </summary>
            /// <param name="hwnd">窗口句柄</param>
            /// <param name="hotKey_id">热键ID</param>
            /// <param name="keyModifiers">组合键</param>
            /// <param name="key">热键</param>
            public static void RegKey(IntPtr hwnd, int hotKey_id, KeyModifiers keyModifiers, Keys key)
            {
                try
                {
                    if (!RegisterHotKey(hwnd, hotKey_id, keyModifiers, key))
                    {
                        switch (Marshal.GetLastWin32Error())
                        {
                            case 1409:
                                MessageBox.Show($"热键被占用\n错误码：{Marshal.GetLastWin32Error()}","注册热键失败");
                                break;
                            default:
                                MessageBox.Show($"未知错误\n错误码：{Marshal.GetLastWin32Error()}", "注册热键失败");
                                break;
                        }
                        Application.Exit();
                    }
                }
                catch (Exception) { }
            }
            /// <summary>
            /// 注销热键
            /// </summary>
            /// <param name="hwnd">窗口句柄</param>
            /// <param name="hotKey_id">热键ID</param>
            public static void UnRegKey(IntPtr hwnd, int hotKey_id)
            {
                //注销Id号为hotKey_id的热键设定
                UnregisterHotKey(hwnd, hotKey_id);
            }
        }
    }

    internal class MouseHelper
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        //移动鼠标 
        public const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
    }
}