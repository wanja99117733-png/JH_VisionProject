namespace JH_VisionProject
{
    partial class AutoTeaching
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
            this.autoFindROI1 = new JH_VisionProject.Setting.AutoFindROI();
            this.SuspendLayout();
            // 
            // autoFindROI1
            // 
            this.autoFindROI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoFindROI1.Location = new System.Drawing.Point(0, 0);
            this.autoFindROI1.Name = "autoFindROI1";
            this.autoFindROI1.Size = new System.Drawing.Size(800, 450);
            this.autoFindROI1.TabIndex = 0;
            // 
            // AutoTeaching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.autoFindROI1);
            this.Name = "AutoTeaching";
            this.Text = "AutoTeaching";
            this.ResumeLayout(false);

        }

        #endregion

        private Setting.AutoFindROI autoFindROI1;
    }
}