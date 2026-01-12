using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JH_VisionProject.Core;
using JH_VisionProject.Inspect;

namespace JH_VisionProject.Property
{

    public partial class AImodelProp : UserControl
    {
        AIEngineType _engineType;
        string _modelPath = string.Empty;
        private SaigeAI _saigeAI = new SaigeAI();

        public AImodelProp()
        {
            InitializeComponent();
            
            cbAImodelType.DataSource = Enum.GetValues(typeof(AIEngineType)).Cast<AIEngineType>().ToList();
            cbAImodelType.SelectedIndex = 0;
        }

        private void AImodelProp_Load(object sender, EventArgs e)
        {
            cbAImodelType.DataSource = Enum.GetValues(typeof(AIEngineType));
        }

        private void txtbAImodelPath_TextChanged(object sender, EventArgs e)
        {

        }


        private void btn_modelselect_Click(object sender, EventArgs e)
        {
            if (cbAImodelType.SelectedItem == null)
            {
                MessageBox.Show("모델 타입을 먼저 선택하세요.");
                return;
            }

            AIEngineType selectedType = (AIEngineType)cbAImodelType.SelectedItem;

            string filter = string.Empty;
            switch (selectedType)
            {
                case AIEngineType.AnomalyDetection:
                    filter = "Image Anormaly Detection Model (*.saigeiad)|*.saigeiad";
                    break;
                case AIEngineType.Segmentation:
                    filter = "Segmentation Model (*.saigeseg)|*.saigeseg";
                    break;
                case AIEngineType.Detection:
                    filter = "Detection Model (*.saigedet)|*.saigedet";
                    break;
                default:
                    filter = "All Files (*.*)|*.*";
                    break;
            }
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "AI 모델 파일 선택";
                openFileDialog.Filter = filter;
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = @"C:\Saige\SaigeVision\engine\Examples\data\sfaw2023\models";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _modelPath = openFileDialog.FileName;
                    txtbAImodelPath.Text = _modelPath;
                }
            }
        }

        private void btn_model_load_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbAImodelPath.Text))
            {
                MessageBox.Show("모델 파일을 먼저 선택하세요.");
                return;
            }

            if (cbAImodelType.SelectedItem == null)
            {
                MessageBox.Show("모델 타입을 선택하세요.");
                return;
            }

            AIEngineType selectedType = (AIEngineType)cbAImodelType.SelectedItem;

            try
            {
                _saigeAI.LoadEngine(txtbAImodelPath.Text, selectedType);
                MessageBox.Show("모델이 정상적으로 로드되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("모델 로드 중 오류가 발생했습니다.\r\n" + ex.Message);
            }
        }

        private void btn_InspAI_Click(object sender, EventArgs e)
        {
            if (_saigeAI == null)
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetBitmap();
            if (bitmap is null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saigeAI.InspAIModule(bitmap);

            Bitmap resultImage = _saigeAI.GetResultImage();

            Global.Inst.InspStage.UpdateDisplay(resultImage);
        }
    }
}
