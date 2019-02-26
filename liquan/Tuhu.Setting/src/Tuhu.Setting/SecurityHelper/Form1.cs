using System;
using System.Windows.Forms;

namespace SecurityHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = this.textBox1.Text;

            this.textBox2.Text = Tuhu.Component.Framework.SecurityHelp.EncodeDes(str);

            string s = Tuhu.Component.Framework.SecurityHelp.DecodeDes(this.textBox2.Text);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string str = this.textBox1.Text;

            this.textBox2.Text = Tuhu.Component.Framework.SecurityHelp.EncryptAES(str);

            string s = Tuhu.Component.Framework.SecurityHelp.DecryptAES(this.textBox2.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = this.textBox1.Text;

            this.textBox2.Text = Tuhu.Component.Framework.SecurityHelp.EncryptShift(str);

            string s = Tuhu.Component.Framework.SecurityHelp.DecryptShift(this.textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string str = this.textBox1.Text;

            this.textBox2.Text = Tuhu.Component.Framework.SecurityHelp.EncryptDES(str);

            string s = Tuhu.Component.Framework.SecurityHelp.DecryptDES(this.textBox2.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = Tuhu.Component.Framework.SecurityHelp.DecryptAES(this.textBox2.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = Tuhu.Component.Framework.SecurityHelp.DecodeDes(this.textBox2.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = Tuhu.Component.Framework.SecurityHelp.DecryptShift(this.textBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = Tuhu.Component.Framework.SecurityHelp.DecryptDES(this.textBox2.Text);
        }
    }
}
