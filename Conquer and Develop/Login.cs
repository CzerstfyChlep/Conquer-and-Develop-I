using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Web;



namespace Conquer_and_Develop
{
    public partial class Login : Form
        
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient tcp = new TcpClient();
                tcp.Connect(textBox1.Text, 2055);
                tcp.Close();
                Form1.IP = textBox1.Text;
                Form1.Number = (int)numericUpDown1.Value;
                Form1.Col = ColorLabel.BackColor;
                Form1.AI = AICheckBox.Checked;
                Form1.nick = NickBox.Text;
                this.Close();
                
            }
            catch
            {
                MessageBox.Show("Invalid IP address or host is offline!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            switch ((int)numericUpDown1.Value)
            {
                case 1:
                    ColorLabel.BackColor = Color.Blue;
                    break;
                case 2:
                    ColorLabel.BackColor = Color.Red;
                    break;
                case 3:
                    ColorLabel.BackColor = Color.Green;
                    break;
                case 4:
                    ColorLabel.BackColor = Color.Orange;
                    break;
                case 5:
                    ColorLabel.BackColor = Color.Purple;
                    break;
                case 6:
                    ColorLabel.BackColor = Color.Pink;
                    break;
                case 7:
                    ColorLabel.BackColor = Color.Navy;
                    break;
                case 8:
                    ColorLabel.BackColor = Color.Maroon;
                    break;
            }
        }
    }
}
