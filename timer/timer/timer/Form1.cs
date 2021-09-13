using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace timer
{
    public partial class Form1 : Form
    {
        public DateTime time = new DateTime();
        DateTime stert;
        DateTime now;
        TimeSpan ts;
        readonly Timer timer;
        int rap = 0;
        int frg = 0;
        public Form1()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Enabled = false;
            timer.Tick += new EventHandler(MyClock);
            timer.Interval = 100;
        }

        public void MyClock(object sender, EventArgs e)
        {
            now = DateTime.Now;
            ts = now - stert;
            label1.Text = ts.ToString(@"hh\:mm\:ss\.ff");
        }

        //開始　停止　ボタン
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer.Enabled == false)//タイマーが停止中なら
            {
                //開始
                if (frg >= 1)
                {
                    TimeSpan stt = DateTime.Now - DateTime.Parse(label1.Text);
                    Debug.WriteLine(stt + "  ++0");
                }
                else
                {
                    stert = DateTime.Now;
                    Debug.WriteLine(stert + "  ++1");
                    frg = 1;
                }
                timer.Enabled = true;
                button1.Text = "停止";
                button1.BackColor = Color.LightCoral;
                button2.Text = "ラップ";
            }
            else
            {
                //停止
                timer.Enabled = false;
                button1.Text = "開始";
                button1.BackColor = Color.LightGreen;
                button2.Text = "リセット";
            }
        }

        //ラップ　リセット　ボタン
        private void button2_Click(object sender, EventArgs e)
        {
            if (timer.Enabled == false)//タイマーが停止中なら
            {
                //リセットが押されたなら                
                listBox1.Items.Clear();
                rap = 0;
                frg = 0;
                label1.Text = "00:00:00.00";
            }
            else
            {
                //ラップが押されたなら
                rap += 1;
                //debug.WriteLine("ラップ押されました" + rap);
                listBox1.Items.Insert(0, "ラップ" + rap + ". " + ts.ToString(@"hh\:mm\:ss\.ff"));
            }
        }
    }
}
