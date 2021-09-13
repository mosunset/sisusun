using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace Othello
{
    public partial class Form1 : Form
    {
        readonly System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public Image image = new Bitmap(1000, 1000);// 表示用ビットマップ
        public static Graphics g;                   // 表示用グラフィックス
        readonly byte[,] dTab = new byte[8, 8];              // マス目のデータ
        readonly int[,] Eval =                               // 評価用テーブル
            {{200,  6, 70, 30, 30, 70,  6,200},
             {  6,  5,  7,  6,  6,  7,  5,  6},
              { 70,  7, 40, 30, 30, 40,  7, 70},
             { 30,  6, 30,  1,  1, 30,  6, 30},
             { 30,  6, 30,  1,  1, 30,  6, 30},
             { 70,  7, 40, 30, 30, 40,  7, 70},
             {  6,  5,  7,  6,  6,  7,  5,  6},
             {200,  6, 70, 30, 30, 70,  6,200}};
        readonly int[,] procTable                            // チェック方向増分表
            = { { -1, -1 }, { -1,  0 }, { -1, 1 }, { 0, -1 },
                {  0,  1 }, {  1, -1 }, {  1, 0 }, { 1,  1 }};
        Boolean gameStart;                          //  ゲーム開始フラグ
        readonly Brush bD = new SolidBrush(Color.DarkGreen); //　ブラシ  暗い緑色
        readonly Brush bW = new SolidBrush(Color.White);     //          白色
        readonly Brush bB = new SolidBrush(Color.Black);     //          黒色  
        readonly Brush bG = new SolidBrush(Color.Gray);      //          灰色
        readonly Pen pB = new Pen(Color.Black, 1);           //  ペン    黒色
        readonly Pen pW = new Pen(Color.White, 1);           //          白色
        readonly Pen pG = new Pen(Color.DarkGray, 1);        //          暗い灰色
        readonly Pen p1 = new Pen(Color.White, 2);           //          白色（太さ2)
        readonly Pen p2 = new Pen(Color.White, 4);           //          白色（太さ4)
        int TB1, TB2;
        readonly int[] co = new int[2];
        int[] r = new int[2];
        int xx;


        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)   //  描画
        {
            base.OnPaint(e);
            display();
            e.Graphics.DrawImage(image, 10, 50);
        }

        private void Form1_Load(object sender, EventArgs e) //  開始処理
        {
            g = Graphics.FromImage(image);
            g.Clear(BackColor);
            initDtab();
            Refresh();
            label1.Text = "先攻,後攻を選択し，\n開始をクリックして下さい。";
        }

        private void wait()                                 //  待ち処理
        {
            timer1.Enabled = true;
            while (!timer1.Enabled)
            {
                ;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {                                             //  開始ボタン処理

            sw.Start();
            Initialize();
            r = countcoma();
            TB1 = r[0];
            TB2 = r[1];
            Debug.WriteLine("黒 = " + TB1 + ", 白 = " + TB2);
            if (TB1 == TB2)
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  引き分け";
            }
            //  石の数で勝負判定
            else if ((TB1 > TB2 && radioButton1.Checked) || (TB2 > TB1 && radioButton2.Checked))
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  あなたが優勢";
            }
            else
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  コンピュータが優勢";
            }

            if (radioButton2.Checked)
            {
                radioButton2.BackColor = Color.DarkOrange; radioButton1.BackColor = SystemColors.Control;
                label1.Text = "あなたは 白石 です";
                computer();

            }
            else
            {
                radioButton1.BackColor = Color.DarkOrange; radioButton2.BackColor = SystemColors.Control;
                label1.Text = "あなたは 黒石 です";
            }
            gameStart = true;
        }

        public void computer()                              //  コンピュータ側判定処理
        {
            Console.WriteLine(sw.Elapsed);
            if (judgeEnd())
            {
                return;
            }        //  ゲーム終了時なら処理なし
            //label1.Text = ""; 
            gameStart = false;
            int num;
            byte TB = 1;
            if (radioButton1.Checked)
            {
                TB = 2;
                radioButton1.BackColor = Color.DarkOrange; radioButton2.BackColor = SystemColors.Control;
            }
            else
            {
                radioButton2.BackColor = Color.DarkOrange; radioButton1.BackColor = SystemColors.Control;
            }
            int MaxPr = -1, MaxI = -1, MaxJ = -1, MaxNum = 0;

            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    int numTotal = 0;
                    for (int i = 0; i < 8; i++) //  セル(ii, jj)に置いたとき裏返す石の数カウント
                    {
                        if ((num = numberCount(ii, jj, TB, i)) > 0)
                        {
                            numTotal += num;
                        }
                    }
                    if (numTotal > 0 && (MaxPr < 0 || MaxPr < Eval[ii, jj] || (MaxPr == Eval[ii, jj] && MaxNum < numTotal)))    //  同一優先順位のとき
                    {   //  裏返すことができる中で最優先順位，同一優先順位のとき裏返す石が多い方
                        MaxPr = Eval[ii, jj];
                        MaxI = ii;
                        MaxJ = jj;
                        MaxNum = numTotal;
                    }
                }
            }
            if (MaxPr < 0)
            {
                MessageBox.Show("コンピュータの打つ場所がありません。パスします");
            }
            else
            {
                replacePlane(MaxI, MaxJ, TB);
            }    //  石を裏返す
            Refresh();
            gameStart = true;
        }

        public void person(int ip, int jp)                  //  対戦者の処理
        {

            if (dTab[ip, jp] != 0)
            {
                return;
            }  //  既に石が置いてあるところは無視
            gameStart = false;
            byte TB = 2;
            if (radioButton1.Checked)
            {
                TB = 1;
            }//  対戦者の石の種類
            if (canPlace(ip, jp, TB))       //  石を置くことができるとき，
            {                               //  裏返し処理
                if (radioButton1.Checked)
                {
                    radioButton2.BackColor = Color.DarkOrange; radioButton1.BackColor = SystemColors.Control;
                }
                else
                {
                    radioButton1.BackColor = Color.DarkOrange; radioButton2.BackColor = SystemColors.Control;
                }
                replacePlane(ip, jp, TB);
                Refresh();
                wait();
                Thread.Sleep(1000);
                computer();                 //  コンピュータ側処理
            }
            else
            {
                MessageBox.Show("そこには打てません。");
            }
            if (judgeEnd())                 //  ゲーム終了の場合
            {
                sw.Stop();
                label2.Text = "ゲーム経過時間　" + sw.Elapsed.ToString(@"hh\:mm\:ss");
                sw.Reset();
                r = countcoma();
                TB1 = r[0];
                TB2 = r[1];
                if (TB1 == TB2)
                {
                    MessageBox.Show("引き分けです。");
                }
                //  石の数で勝負判定
                else if ((TB1 > TB2 && radioButton1.Checked) || (TB2 > TB1 && radioButton2.Checked))
                {
                    MessageBox.Show("貴方の勝ちです。黒 = " + TB1 + ", 白 = " + TB2);
                }
                else
                {
                    MessageBox.Show("コンピュータの勝ちです。黒 = " + TB1 + ", 白 = " + TB2);
                }
                radioButton1.BackColor = SystemColors.Control;
                radioButton2.BackColor = SystemColors.Control;
                label1.Text = "先攻,後攻を選択し，\n開始をクリックして下さい。";
            }
            gameStart = true;
            Refresh();
        }

        private int[] countcoma()
        {
            TB1 = 0;
            TB2 = 0;   //  両者の石をカウントする
            co[0] = 0;
            co[1] = 0;
            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    if (dTab[ii, jj] == 1)
                    {
                        co[0]++;
                    }
                    else if (dTab[ii, jj] == 2)
                    {
                        co[1]++;
                    }
                }
            }
            return co;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)  //  画面をクリックしたら
        {                                                               //  対戦者の処理
            int x = (e.X - 20) / 50, y = (e.Y - 60) / 50;
            if (x > 7 || y < 0)
            {
                return;
            }
            //Debug.WriteLine(y + " " + x);
            person(y, x);
        }

        private void button2_Click(object sender, EventArgs e)  //  「パス」ボタンが押されたら
        {   // 石を置ける場所があるかどうか判定し、置ける場合はパス解消。ない場合にパス。
            int TB = 2;
            if (radioButton1.Checked)
            {
                TB = 1;
            }
            if (canPlace(TB))
            {
                MessageBox.Show("あなたの置ける場所がありますのでパスできません");
            }
            else
            {
                computer();
            }
        }

        private void Initialize()    //  初期設定
        {
            gameStart = false;
            initDtab();
            Refresh();
        }

        private void initDtab()      //  石の配置初期設定
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dTab[i, j] = 0;
                }
            }
            dTab[3, 3] = dTab[4, 4] = 1;
            dTab[4, 3] = dTab[3, 4] = 2;
        }

        public int numberCount(int ii, int jj, int TB, int i)
        {    //  石を置いたとき裏返す石をカウント。なければ -1 を返却する。
            if (dTab[ii, jj] != 0)
            {
                return -1;
            } //  既に石が配置されていれば置けない。
            int RB = 3 - TB, ip = procTable[i, 0], jp = procTable[i, 1]; //  チェック方向設定
            int IIP = ii + ip, JJP = jj + jp;                       // 1 個先の配列添字設定。
            if (IIP < 0 || IIP > 7 || JJP < 0 || JJP > 7)
            {
                return -1;
            }// 1 個先が範囲外なら置けない。
            if (dTab[IIP, JJP] != RB)
            {
                return -1;
            }                    // 1 個先が対戦者の石でないとき
            int numStone = 1;                                       // 置けない(自分の石/石がない)。
            while (dTab[IIP, JJP] == RB)                            // 対戦者の石が続くときカウント。
            {
                IIP = IIP + ip;
                JJP = JJP + jp;
                if (IIP < 0 || IIP > 7 || JJP < 0 || JJP > 7)
                {
                    return -1;
                }
                numStone++;
            }

            if (dTab[IIP, JJP] == TB)
            {
                return numStone--;
            }           //　対戦者の石が途切れ箇所が
            else
            {
                return -1;
            }                                         //  自分の石のとき石を置ける。
        }

        public Boolean canPlace(int ii, int jj, int TB)             // セル(ii, jj)に石を置くこと
        {   　　　　　　　　　　　// ができるかを判定。
            for (int i = 0; i < 8; i++)
            {
                if (numberCount(ii, jj, TB, i) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean canPlace(int TB)　                            // 石を置く場所があるかを判定。
        {
            label2.Text = "";
            int c = 0;
            for (int ii = 0; ii < 8; ii++)
            {
                for (int jj = 0; jj < 8; jj++)
                {
                    if (canPlace(ii, jj, TB))
                    {
                        c++;
                        if (TB == 1)
                        {
                            label2.Text += "X" + jj + " Y" + ii + "\n";
                        }
                        dspStone(ii, jj, bB, pB, 10);
                    }
                }
            }

            if (c >= 1)
            {
                Refresh();
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean judgeEnd()                                    // 終了判定。
        {
            for (int TB = 1; TB < 3; TB++)
            {
                if (canPlace(TB))
                {
                    return false;
                }
            }
            return true;
        }

        public void replacePlane(int ii, int jj, byte TB)     // 石の裏返し。
        {
            byte[,] Temp = new byte[8, 8];// 石の状態を作業領域に移して
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Temp[i, j] = dTab[i, j];
                }
            }
            for (int i = 0; i < 8; i++)  // 作業領域を判定して
            {
                int num = numberCount(ii, jj, TB, i), ip = ii, jp = jj;
                int di = procTable[i, 0], dj = procTable[i, 1];
                if (num > 0)             // 石を裏返す
                {
                    for (int j = 0; j <= num; j++, ip += di, jp += dj)
                    {
                        Temp[ip, jp] = TB;
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dTab[i, j] = Temp[i, j];
                }
            }
            r = countcoma();
            TB1 = r[0];
            TB2 = r[1];
            Debug.WriteLine("黒 = " + TB1 + ", 白 = " + TB2);
            if (TB1 == TB2)
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  引き分け";
            }
            //  石の数で勝負判定
            else if ((TB1 > TB2 && radioButton1.Checked) || (TB2 > TB1 && radioButton2.Checked))
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  あなたが優勢";
            }
            else
            {
                label3.Text = "黒 = " + TB1 + ",  白 = " + TB2 + ",  コンピュータが優勢";
            }
        }

        private void dspStone(int i, int j, Brush b, Pen p, float R)// 石の表示
        {
            float X = j * 50 + 20, X1 = X + 1;
            float Y = i * 50 + 20, Y1 = Y + 1;
            g.FillEllipse(bG, X1, Y1, R, R); //  石に厚さがあるような形で表示
            g.FillEllipse(b, X, Y, R, R);
            g.DrawEllipse(p, X, Y, R, R);
        }

        public void display()
        {
            g.FillRectangle(bD, 10, 10, 400, 400); // マス目の表示
            for (float X = 60; X < 390; X += 50)
            {
                g.DrawLine(p1, X, 10, X, 410);
            }
            for (float Y = 60; Y < 390; Y += 50)
            {
                g.DrawLine(p1, 10, Y, 410, Y);
            }
            g.DrawRectangle(p2, 10, 10, 400, 400);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {// 白い石は大きく見えるので若干小さく表示
                    if (dTab[i, j] == 1)
                    {
                        dspStone(i, j, bB, pG, 30);
                    }
                    else if (dTab[i, j] == 2)
                    {
                        dspStone(i, j, bW, pB, 29);
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }
    }
}
