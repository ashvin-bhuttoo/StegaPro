namespace StegaPro
{
    partial class frmMain
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
            this.txtText = new System.Windows.Forms.TextBox();
            this.btnSelectSrc = new System.Windows.Forms.Button();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblBytesAvailable = new System.Windows.Forms.Label();
            this.lblsrcImage = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblmsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(9, 25);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtText.Size = new System.Drawing.Size(796, 84);
            this.txtText.TabIndex = 0;
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            // 
            // btnSelectSrc
            // 
            this.btnSelectSrc.Location = new System.Drawing.Point(9, 115);
            this.btnSelectSrc.Name = "btnSelectSrc";
            this.btnSelectSrc.Size = new System.Drawing.Size(143, 23);
            this.btnSelectSrc.TabIndex = 1;
            this.btnSelectSrc.Text = "Select Source Image";
            this.btnSelectSrc.UseVisualStyleBackColor = true;
            this.btnSelectSrc.Click += new System.EventHandler(this.btnSelectSrc_Click);
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(12, 164);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(796, 339);
            this.pbImage.TabIndex = 2;
            this.pbImage.TabStop = false;
            // 
            // lblBytesAvailable
            // 
            this.lblBytesAvailable.AutoSize = true;
            this.lblBytesAvailable.Location = new System.Drawing.Point(252, 120);
            this.lblBytesAvailable.Name = "lblBytesAvailable";
            this.lblBytesAvailable.Size = new System.Drawing.Size(178, 13);
            this.lblBytesAvailable.TabIndex = 3;
            this.lblBytesAvailable.Text = "Image Size: ? x ?, Bytes Available: 0";
            // 
            // lblsrcImage
            // 
            this.lblsrcImage.AutoSize = true;
            this.lblsrcImage.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblsrcImage.Location = new System.Drawing.Point(15, 144);
            this.lblsrcImage.Name = "lblsrcImage";
            this.lblsrcImage.Size = new System.Drawing.Size(0, 13);
            this.lblsrcImage.TabIndex = 3;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(158, 115);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear Text";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblmsg
            // 
            this.lblmsg.AutoSize = true;
            this.lblmsg.Location = new System.Drawing.Point(6, 9);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(53, 13);
            this.lblmsg.TabIndex = 4;
            this.lblmsg.Text = "Message:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 515);
            this.Controls.Add(this.lblmsg);
            this.Controls.Add(this.lblsrcImage);
            this.Controls.Add(this.lblBytesAvailable);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSelectSrc);
            this.Controls.Add(this.txtText);
            this.MaximumSize = new System.Drawing.Size(836, 553);
            this.MinimumSize = new System.Drawing.Size(836, 553);
            this.Name = "frmMain";
            this.Text = "StegaPro";
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Button btnSelectSrc;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label lblBytesAvailable;
        private System.Windows.Forms.Label lblsrcImage;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblmsg;
    }
}

