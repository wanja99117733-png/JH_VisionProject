namespace JH_VisionProject.Property
{
    partial class AIModelProp
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
            this.cbAImodelType = new System.Windows.Forms.ComboBox();
            this.txtbAImodelPath = new System.Windows.Forms.TextBox();
            this.btn_modelselect = new System.Windows.Forms.Button();
            this.btn_model_load = new System.Windows.Forms.Button();
            this.btn_InspAI = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbAImodelType
            // 
            this.cbAImodelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAImodelType.FormattingEnabled = true;
            this.cbAImodelType.Location = new System.Drawing.Point(16, 14);
            this.cbAImodelType.Name = "cbAImodelType";
            this.cbAImodelType.Size = new System.Drawing.Size(160, 26);
            this.cbAImodelType.TabIndex = 0;
            // 
            // txtbAImodelPath
            // 
            this.txtbAImodelPath.BackColor = System.Drawing.SystemColors.Control;
            this.txtbAImodelPath.Location = new System.Drawing.Point(16, 47);
            this.txtbAImodelPath.Name = "txtbAImodelPath";
            this.txtbAImodelPath.ReadOnly = true;
            this.txtbAImodelPath.Size = new System.Drawing.Size(336, 28);
            this.txtbAImodelPath.TabIndex = 1;
            this.txtbAImodelPath.TextChanged += new System.EventHandler(this.txtbAImodelPath_TextChanged);
            // 
            // btn_modelselect
            // 
            this.btn_modelselect.Location = new System.Drawing.Point(16, 81);
            this.btn_modelselect.Name = "btn_modelselect";
            this.btn_modelselect.Size = new System.Drawing.Size(113, 45);
            this.btn_modelselect.TabIndex = 2;
            this.btn_modelselect.Text = "AI모델 선택";
            this.btn_modelselect.UseVisualStyleBackColor = true;
            this.btn_modelselect.Click += new System.EventHandler(this.btn_modelselect_Click);
            // 
            // btn_model_load
            // 
            this.btn_model_load.Location = new System.Drawing.Point(16, 132);
            this.btn_model_load.Name = "btn_model_load";
            this.btn_model_load.Size = new System.Drawing.Size(113, 45);
            this.btn_model_load.TabIndex = 3;
            this.btn_model_load.Text = "모델 로딩";
            this.btn_model_load.UseVisualStyleBackColor = true;
            this.btn_model_load.Click += new System.EventHandler(this.btn_model_load_Click);
            // 
            // btn_InspAI
            // 
            this.btn_InspAI.Location = new System.Drawing.Point(16, 183);
            this.btn_InspAI.Name = "btn_InspAI";
            this.btn_InspAI.Size = new System.Drawing.Size(113, 45);
            this.btn_InspAI.TabIndex = 4;
            this.btn_InspAI.Text = "AI 검사";
            this.btn_InspAI.UseVisualStyleBackColor = true;
            this.btn_InspAI.Click += new System.EventHandler(this.btn_InspAI_Click);
            // 
            // AImodelProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_InspAI);
            this.Controls.Add(this.btn_model_load);
            this.Controls.Add(this.btn_modelselect);
            this.Controls.Add(this.txtbAImodelPath);
            this.Controls.Add(this.cbAImodelType);
            this.Name = "AImodelProp";
            this.Size = new System.Drawing.Size(369, 586);
            this.Load += new System.EventHandler(this.AImodelProp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAImodelType;
        private System.Windows.Forms.TextBox txtbAImodelPath;
        private System.Windows.Forms.Button btn_modelselect;
        private System.Windows.Forms.Button btn_model_load;
        private System.Windows.Forms.Button btn_InspAI;
    }
}
