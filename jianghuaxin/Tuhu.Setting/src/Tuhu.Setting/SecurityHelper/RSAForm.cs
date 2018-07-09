using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tuhu.CryptoService;

namespace SecurityHelper
{
    public partial class RSAForm : Form
    {
        public RSAForm()
        {
            InitializeComponent();
        }

        private void btnOld_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            tbPResult.Text = tbOriginal.Text.Trim().Encrypt();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            tbPResult.Text = tbOriginal.Text.Trim().Decrypt();
        }
    }
    
}
