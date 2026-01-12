namespace JH_VisionProject.Property
{
    partial class MatchInspProp
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
                txtExtendX.Leave -= OnUpdateValue;
                txtExtendY.Leave -= OnUpdateValue;
                txtScore.Leave -= OnUpdateValue;

                patternImageEditor.ButtonChanged -= PatternImage_ButtonChanged;

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.grpMatch = new System.Windows.Forms.GroupBox();
            this.chkInvertResult = new System.Windows.Forms.CheckBox();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.txtExtendY = new System.Windows.Forms.TextBox();
            this.txtExtendX = new System.Windows.Forms.TextBox();
            this.lblX = new System.Windows.Forms.Label();
            this.lbScore = new System.Windows.Forms.Label();
            this.lbExtent = new System.Windows.Forms.Label();
            this.patternImageEditor = new JH_VisionProject.UIControl.PatternImageEditor();
            this.grpMatch.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Checked = true;
            this.chkUse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUse.Location = new System.Drawing.Point(20, 20);
            this.chkUse.Margin = new System.Windows.Forms.Padding(4);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(70, 22);
            this.chkUse.TabIndex = 0;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            this.chkUse.CheckedChanged += new System.EventHandler(this.chkUse_CheckedChanged);
            // 
            // grpMatch
            // 
            this.grpMatch.Controls.Add(this.chkInvertResult);
            this.grpMatch.Controls.Add(this.txtScore);
            this.grpMatch.Controls.Add(this.txtExtendY);
            this.grpMatch.Controls.Add(this.txtExtendX);
            this.grpMatch.Controls.Add(this.lblX);
            this.grpMatch.Controls.Add(this.lbScore);
            this.grpMatch.Controls.Add(this.lbExtent);
            this.grpMatch.Location = new System.Drawing.Point(9, 52);
            this.grpMatch.Name = "grpMatch";
            this.grpMatch.Size = new System.Drawing.Size(420, 154);
            this.grpMatch.TabIndex = 1;
            this.grpMatch.TabStop = false;
            // 
            // chkInvertResult
            // 
            this.chkInvertResult.AutoSize = true;
            this.chkInvertResult.Location = new System.Drawing.Point(12, 125);
            this.chkInvertResult.Margin = new System.Windows.Forms.Padding(4);
            this.chkInvertResult.Name = "chkInvertResult";
            this.chkInvertResult.Size = new System.Drawing.Size(112, 22);
            this.chkInvertResult.TabIndex = 6;
            this.chkInvertResult.Text = "결과 반전";
            this.chkInvertResult.UseVisualStyleBackColor = true;
            this.chkInvertResult.CheckedChanged += new System.EventHandler(this.chkInvertResult_CheckedChanged);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(126, 70);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(70, 28);
            this.txtScore.TabIndex = 5;
            // 
            // txtExtendY
            // 
            this.txtExtendY.Location = new System.Drawing.Point(229, 36);
            this.txtExtendY.Name = "txtExtendY";
            this.txtExtendY.Size = new System.Drawing.Size(70, 28);
            this.txtExtendY.TabIndex = 4;
            // 
            // txtExtendX
            // 
            this.txtExtendX.Location = new System.Drawing.Point(126, 36);
            this.txtExtendX.Name = "txtExtendX";
            this.txtExtendX.Size = new System.Drawing.Size(70, 28);
            this.txtExtendX.TabIndex = 1;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(202, 39);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(18, 18);
            this.lblX.TabIndex = 2;
            this.lblX.Text = "x";
            // 
            // lbScore
            // 
            this.lbScore.AutoSize = true;
            this.lbScore.Location = new System.Drawing.Point(9, 75);
            this.lbScore.Name = "lbScore";
            this.lbScore.Size = new System.Drawing.Size(98, 18);
            this.lbScore.TabIndex = 1;
            this.lbScore.Text = "매칭스코어";
            // 
            // lbExtent
            // 
            this.lbExtent.AutoSize = true;
            this.lbExtent.Location = new System.Drawing.Point(9, 39);
            this.lbExtent.Name = "lbExtent";
            this.lbExtent.Size = new System.Drawing.Size(80, 18);
            this.lbExtent.TabIndex = 0;
            this.lbExtent.Text = "확장영역";
            // 
            // patternImageEditor
            // 
            this.patternImageEditor.Location = new System.Drawing.Point(9, 216);
            this.patternImageEditor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.patternImageEditor.Name = "patternImageEditor";
            this.patternImageEditor.Size = new System.Drawing.Size(420, 239);
            this.patternImageEditor.TabIndex = 4;
            this.patternImageEditor.ButtonChanged += new System.EventHandler<JH_VisionProject.UIControl.PatternImageEventArgs>(this.patternImageEditor_ButtonChanged);
            // 
            // MatchInspProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.patternImageEditor);
            this.Controls.Add(this.grpMatch);
            this.Controls.Add(this.chkUse);
            this.Name = "MatchInspProp";
            this.Size = new System.Drawing.Size(446, 555);
            this.grpMatch.ResumeLayout(false);
            this.grpMatch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUse;
        private System.Windows.Forms.GroupBox grpMatch;
        private System.Windows.Forms.Label lbScore;
        private System.Windows.Forms.Label lbExtent;
        private System.Windows.Forms.CheckBox chkInvertResult;
        private System.Windows.Forms.TextBox txtScore;
        private System.Windows.Forms.TextBox txtExtendY;
        private System.Windows.Forms.TextBox txtExtendX;
        private System.Windows.Forms.Label lblX;
        private UIControl.PatternImageEditor patternImageEditor;
    }
}
