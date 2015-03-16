namespace DirectX11Test
{
    partial class Form1
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
            this.directXControl1 = new NG3Dx.DirectXControl();
            this.SuspendLayout();
            // 
            // directXControl1
            // 
            this.directXControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directXControl1.Location = new System.Drawing.Point(0, 0);
            this.directXControl1.Name = "directXControl1";
            this.directXControl1.Size = new System.Drawing.Size(800, 600);
            this.directXControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.directXControl1);
            this.Name = "Form1";
            this.Text = "NG3Dx Engine";
            this.ResumeLayout(false);

        }

        #endregion

        private NG3Dx.DirectXControl directXControl1;
    }
}

