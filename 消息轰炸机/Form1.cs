using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace 消息轰炸机 {
    public partial class Form1 : Form {

        //当用translatemessage函数翻译WM_KEYUP消息时发送此消息给拥有焦点的窗口  
        private const int WM_CHAR = 0X102;

        //SendMessageA：ANSI 
        [DllImport("user32.dll")]
        private static extern int SendMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);
        
        //注册热键的api
        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);

        //解除注册热键的api
        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //获取焦点的句柄
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            try {
                MessageBox.Show("启动成功！");
                RegisterHotKey(this.Handle, 123, 0, Keys.F9);
                RegisterHotKey(this.Handle, 456, 0, Keys.F10);
            }
            catch (Exception ee) {
                MessageBox.Show(ee.ToString());
            }
        }

        public static void InputStr(IntPtr myIntPtr, string Input) {
            byte[] ch = (Encoding.Default.GetBytes(Input));
            for (int i = 0; i < ch.Length; i++) {
                SendMessageA(myIntPtr, WM_CHAR, ch[i], 0);
            }
        }

        static void write() {
            while (true) {
                StreamReader sr = new StreamReader("library.txt", Encoding.UTF8);
                String line;
                while ((line = sr.ReadLine()) != null) {
                    InputStr(GetForegroundWindow(), line);
                    SendKeys.SendWait("{enter}");
                }
            }
        }

        Thread t = new Thread(write);

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case 0x0312:  //这个是window消息定义的注册的热键消息  
                    if (m.WParam.ToString() == "123") {
                        //开始
                        try {
                            t.Resume();//继续线程t
                        }
                        catch {
                            try { t.Start();//启动线程t
                            } catch { }
                        }
                    }
                    else if (m.WParam.ToString() == "456") {
                        //停止
                        try { t.Suspend();//挂起线程t
                        } catch { }
                    }
                    break;
            }   
            base.WndProc(ref m);
        }
    }
}
