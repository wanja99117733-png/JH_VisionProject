namespace JH_VisionProject.Property
{
    partial class BinaryProp
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (binRangeTrackbar != null)
                    binRangeTrackbar.RangeChanged -= Range_RangeChanged;


                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpBinary = new System.Windows.Forms.GroupBox();
            this.binRangeTrackbar = new JH_VisionProject.UIControl.RangeTrackbar();
            this.cbHighlight = new System.Windows.Forms.ComboBox();
            this.lbHighlight = new System.Windows.Forms.Label();
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.grpBinary.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBinary
            // 
            this.grpBinary.Controls.Add(this.binRangeTrackbar);
            this.grpBinary.Controls.Add(this.cbHighlight);
            this.grpBinary.Controls.Add(this.lbHighlight);
            this.grpBinary.Location = new System.Drawing.Point(16, 50);
            this.grpBinary.Margin = new System.Windows.Forms.Padding(4);
            this.grpBinary.Name = "grpBinary";
            this.grpBinary.Padding = new System.Windows.Forms.Padding(4);
            this.grpBinary.Size = new System.Drawing.Size(357, 135);
            this.grpBinary.TabIndex = 0;
            this.grpBinary.TabStop = false;
            this.grpBinary.Text = "이진화";
            // 
            // rangeTrackbar
            // 
            this.binRangeTrackbar.Location = new System.Drawing.Point(12, 28);
            this.binRangeTrackbar.Name = "rangeTrackbar";
            this.binRangeTrackbar.Size = new System.Drawing.Size(338, 57);
            this.binRangeTrackbar.TabIndex = 5;
            this.binRangeTrackbar.ValueLeft = 80;
            this.binRangeTrackbar.ValueRight = 200;
            // 
            // cbHighlight
            // 
            this.cbHighlight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHighlight.FormattingEnabled = true;
            this.cbHighlight.Location = new System.Drawing.Point(110, 88);
            this.cbHighlight.Margin = new System.Windows.Forms.Padding(4);
            this.cbHighlight.Name = "cbHighlight";
            this.cbHighlight.Size = new System.Drawing.Size(165, 26);
            this.cbHighlight.TabIndex = 2;
            this.cbHighlight.SelectedIndexChanged += new System.EventHandler(this.cbHighlight_SelectedIndexChanged);
            // 
            // lbHighlight
            // 
            this.lbHighlight.AutoSize = true;
            this.lbHighlight.Location = new System.Drawing.Point(9, 98);
            this.lbHighlight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbHighlight.Name = "lbHighlight";
            this.lbHighlight.Size = new System.Drawing.Size(98, 18);
            this.lbHighlight.TabIndex = 7;
            this.lbHighlight.Text = "하이라이트";
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Checked = true;
            this.chkUse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUse.Location = new System.Drawing.Point(16, 21);
            this.chkUse.Margin = new System.Windows.Forms.Padding(4);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(70, 22);
            this.chkUse.TabIndex = 4;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // BinaryProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUse);
            this.Controls.Add(this.grpBinary);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BinaryProp";
            this.Size = new System.Drawing.Size(389, 522);
            this.grpBinary.ResumeLayout(false);
            this.grpBinary.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBinary;
        private System.Windows.Forms.ComboBox cbHighlight;
        private System.Windows.Forms.Label lbHighlight;
        private System.Windows.Forms.CheckBox chkUse;
        private UIControl.RangeTrackbar binRangeTrackbar;
    }
}
