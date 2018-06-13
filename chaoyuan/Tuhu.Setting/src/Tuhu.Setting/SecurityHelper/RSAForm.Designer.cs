namespace SecurityHelper
{
    partial class RSAForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.btnOld = new System.Windows.Forms.Button();
            this.tbPResult = new SecurityHelper.WaterTextBox();
            this.tbOriginal = new SecurityHelper.WaterTextBox();
            this.SuspendLayout();
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(96, 184);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(75, 23);
            this.btnEncrypt.TabIndex = 1;
            this.btnEncrypt.Text = "加密";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(284, 184);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(75, 23);
            this.btnDecrypt.TabIndex = 2;
            this.btnDecrypt.Text = "解密";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // btnOld
            // 
            this.btnOld.Location = new System.Drawing.Point(548, 184);
            this.btnOld.Name = "btnOld";
            this.btnOld.Size = new System.Drawing.Size(75, 23);
            this.btnOld.TabIndex = 3;
            this.btnOld.Text = "老方式";
            this.btnOld.UseVisualStyleBackColor = true;
            this.btnOld.Click += new System.EventHandler(this.btnOld_Click);
            // 
            // tbPResult
            // 
            this.tbPResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbPResult.Location = new System.Drawing.Point(0, 213);
            this.tbPResult.Multiline = true;
            this.tbPResult.Name = "tbPResult";
            this.tbPResult.Size = new System.Drawing.Size(832, 233);
            this.tbPResult.TabIndex = 5;
            this.tbPResult.WaterText = "处理后结果文本";
            // 
            // tbOriginal
            // 
            this.tbOriginal.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbOriginal.Location = new System.Drawing.Point(0, 0);
            this.tbOriginal.Multiline = true;
            this.tbOriginal.Name = "tbOriginal";
            this.tbOriginal.Size = new System.Drawing.Size(832, 178);
            this.tbOriginal.TabIndex = 4;
            this.tbOriginal.WaterText = "原文本内容";
            // 
            // RSAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 446);
            this.Controls.Add(this.tbPResult);
            this.Controls.Add(this.tbOriginal);
            this.Controls.Add(this.btnOld);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnEncrypt);
            this.MaximizeBox = false;
            this.Name = "RSAForm";
            this.Text = "RSAForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Button btnOld;
        private WaterTextBox tbOriginal;
        private WaterTextBox tbPResult;
    }
}