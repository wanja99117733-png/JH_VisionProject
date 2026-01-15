namespace JH_VisionProject
{
    partial class CameraForm
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
            this.mainViewToolbar1 = new JH_VisionProject.UIControl.MainViewToolbar();
            this.imageViewer = new JH_VisionProject.UIControl.ImageViewCtrl();
            this.SuspendLayout();
            // 
            // mainViewToolbar1
            // 
            this.mainViewToolbar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.mainViewToolbar1.Location = new System.Drawing.Point(687, 0);
            this.mainViewToolbar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainViewToolbar1.Name = "mainViewToolbar1";
            this.mainViewToolbar1.Size = new System.Drawing.Size(113, 450);
            this.mainViewToolbar1.TabIndex = 1;
            // 
            // imageViewer
            // 
            this.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer.Location = new System.Drawing.Point(0, 0);
            this.imageViewer.Name = "imageViewer";
            this.imageViewer.Size = new System.Drawing.Size(800, 450);
            this.imageViewer.TabIndex = 0;
            this.imageViewer.WorkingState = "";
            this.imageViewer.Load += new System.EventHandler(this.imageViewer_Load);
            // 
            // CameraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainViewToolbar1);
            this.Controls.Add(this.imageViewer);
            this.Name = "CameraForm";
            this.Text = "CameraForm";
            this.Load += new System.EventHandler(this.CameraForm_Load);
            this.Resize += new System.EventHandler(this.CameraForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private UIControl.ImageViewCtrl imageViewer;
        private UIControl.MainViewToolbar mainViewToolbar1;
    }
}