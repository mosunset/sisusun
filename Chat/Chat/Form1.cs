using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MySqlConnector;
using System.Threading;
using System.Text.RegularExpressions;

namespace Chat
{
    public partial class Form1 : Form
    {
        MySqlConnection connection = new MySqlConnection("server=127.0.0.1;port=3306;user=root;database=chat;password=");

        public Form1()
        {
            InitializeComponent();
            readsql();
            Random r1 = new System.Random();
            textBox1.Text = GeneratePassword(10); textBox2.Text = r1.Next(1, 1000).ToString();
        }
        private void readsql()
        {
            listBox1.Items.Clear();
            connection.Open();

            Debug.WriteLine(connection.State);

            string sql = "SELECT * FROM honbun order by time desc";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            Thread.Sleep(3);
            while (rdr.Read())
            {
                Console.WriteLine(rdr[0] + " " + rdr[1] + " " + rdr[2]);

                listBox1.Items.Add(rdr[0] + "     -     " + rdr[1] + "     -     " + rdr[2]);
            }
            cmd.Dispose();
            rdr.Close();
            connection.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private static readonly string passwordChars = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string GeneratePassword(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            Random r = new Random();

            for (int i = 0; i < length; i++)
            {
                //文字の位置をランダムに選択
                int pos = r.Next(passwordChars.Length);
                //選択された位置の文字を取得
                char c = passwordChars[pos];
                //パスワードに追加
                sb.Append(c);
            }

            return sb.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            readsql();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection.Open();


            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = "INSERT INTO `honbun` (`text`) VALUES (@text);";
            command.Parameters.AddWithValue("text", textBox1.Text);
            command.ExecuteNonQueryAsync();

            // 使い終わった変数を解放する
            command.Dispose();
            connection.Close();
            Random r1 = new System.Random();
            textBox1.Text = GeneratePassword(10); textBox2.Text = r1.Next(1, 1000).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            connection.Open();


            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = "DELETE FROM `honbun` WHERE id = @id";
            command.Parameters.AddWithValue("id", textBox3.Text);
            command.ExecuteNonQuery();

            // 使い終わった変数を解放する
            command.Dispose();
            connection.Close();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            connection.Open();


            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = "DELETE FROM `honbun` WHERE id >= -10000";
            command.Parameters.AddWithValue("id", textBox3.Text);
            command.ExecuteNonQueryAsync();

            // 使い終わった変数を解放する
            command.Dispose();
            connection.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            String t = listBox1.SelectedItem.ToString();
            Match matche = Regex.Match(t, @"[^\s]*[0-9]$");
            textBox3.Text = matche.Value;
            connection.Open();

            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = "DELETE FROM `honbun` WHERE id = @id";
            command.Parameters.AddWithValue("id", textBox3.Text);
            command.ExecuteNonQuery();
            command.Dispose();

            

            string sql = "SELECT * FROM honbun order by time desc";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();
            listBox1.Items.Clear();
            
            Thread.Sleep(3);
            while (rdr.Read())
            {
                Console.WriteLine(rdr[0] + " " + rdr[1] + " " + rdr[2]);

                listBox1.Items.Add(rdr[0] + "     -     " + rdr[1] + "     -     " + rdr[2]);
            }
            cmd.Dispose();
            rdr.Close();

            // 使い終わった変数を解放する
            
            connection.Close();
        }
    }
}