namespace JH_VisionProject.Setting
{
    partial class AutoFindROI
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
            this.btnApply = new System.Windows.Forms.Button();
            this.lbScore = new System.Windows.Forms.Label();
            this.txtScore = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(230, 179);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(126, 74);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "찾기";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lbScore
            // 
            this.lbScore.AutoSize = true;
            this.lbScore.Location = new System.Drawing.Point(25, 53);
            this.lbScore.Name = "lbScore";
            this.lbScore.Size = new System.Drawing.Size(88, 18);
            this.lbScore.TabIndex = 1;
            this.lbScore.Text = "Score (%)";
            this.lbScore.Click += new System.EventHandler(this.lbScore_Click);
            // 
            // txtScore
            // 
            this.txtScore.Location = new System.Drawing.Point(119, 50);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(197, 28);
            this.txtScore.TabIndex = 2;
            this.txtScore.Leave += new System.EventHandler(this.txtScore_Leave);
            // 
            // AutoFindROI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.lbScore);
            this.Controls.Add(this.btnApply);
            this.Name = "AutoFindROI";
            this.Size = new System.Drawing.Size(404, 341);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lbScore;
        private System.Windows.Forms.TextBox txtScore;
    }
}
