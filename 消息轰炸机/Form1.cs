using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace 消息轰炸机 {
    public partial class Form1 : Form {
        [DllImport("user32.dll")]
        private static extern int SendMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_CHAR = 0X102;
        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);
        //解除注册热键的api
        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32", EntryPoint = "PaintDesktop")]
        public static extern int PaintDesktop(int hdc);
        [DllImport("user32.dll", EntryPoint = "GetCursor")]
        public static extern IntPtr GetCursor();
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

                //string[] lib = { "消息1", "消息2" };
                //for (int i = 0; i < 2; i++) {
                //    InputStr(GetForegroundWindow(), lib[i]);
                //    SendKeys.SendWait("{enter}");
                //}
            }

        }
        Thread t = new Thread(write);
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case 0x0312:  //这个是window消息定义的注册的热键消息  
                    if (m.WParam.ToString() == "123") {
                        //开始
                        try {
                            t.Resume();
                        }
                        catch {
                            try { t.Start(); }
                            catch { }
                        }
                    }
                    else if (m.WParam.ToString() == "456") {
                        //停止
                        try {
                            t.Suspend();
                        }
                        catch { }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
