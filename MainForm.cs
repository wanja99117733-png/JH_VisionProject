using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JH_VisionProject.Algorithm;
using JH_VisionProject.Core;
using JH_VisionProject.Setting;
using JH_VisionProject.Teach;
using JH_VisionProject.Utill;
using OpenCvSharp;
using WeifenLuo.WinFormsUI.Docking;

namespace JH_VisionProject
{
    public partial class MainForm : Form
    {
        private AutoTeaching _autoTeachingForm;

        private static DockPanel _dockPanel;    // 도킹패널 멤버변수 선언
        public MainForm()
        {
            InitializeComponent();

            _dockPanel = new DockPanel()    // 도킹패널 객체 생성
            {
                Dock = DockStyle.Fill       // 도킹패널이 폼 전체를 채우도록 설정
            };
            /*
            위에 코드와 
            _dockPanel = new DockPanel();
            _dockPanel.Dock = DockStyle.Fill;
            가 동일함
            */

            Controls.Add(_dockPanel);       // controls를 통해 폼에  도킹패널 추가

            _dockPanel.Theme = new VS2015BlueTheme();   // 도킹패널 테마 설정

            LoadDockingWindow();

            Global.Inst.InspStage.Initialize();  // 전역변수로 선언된 InspStage의 Initialize함수 호출
        }
        /*
         * DockState     = _dockPanel에 직접 위치를 정할때
         * DockAlignment = 이미 위치가 지정된 폼을 기준으로 위치를 정할때
         */
        private void LoadDockingWindow()
        {
            //도킹해제 금지 설정
            _dockPanel.AllowEndUserDocking = false;

            //메인폼 설정
            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);

            //#13_INSP_RESULT#7 검사 결과창 30% 비율로 추가
            var resultWindow = new ResultForm();
            resultWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.3);

            //# MODEL TREE#3 검사 결과창 우측에 40% 비율로 모델트리 추가
            var modelTreeWindow = new ModelTreeForm();
            modelTreeWindow.Show(resultWindow.Pane, DockAlignment.Right, 0.4);

            //실행창 추가
            var runWindow = new RunForm();
            runWindow.Show(modelTreeWindow.Pane, null);

            //속성창 추가
            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockRight);

            //#14_LOGFORM#2 로그창 추가
            var logwindow = new LogForm();
            logwindow.Show(propWindow.Pane, DockAlignment.Bottom, 0.3);

        }
        //제네릭 함수 사용을 이용해 입력된 타입의 폼 객체 얻기
        
        public static T GetDockForm<T>() where T : DockContent
        {
            var findForm = _dockPanel.Contents.OfType<T>().FirstOrDefault();
            return findForm;
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void imageOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "이미지 파일 선택";
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    //#11_MATCHING#12 이미지 로딩함수 변경
                    Global.Inst.InspStage.SetImageBuffer(filePath);
                    Global.Inst.InspStage.CurModel.InspectImagePath = filePath;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void setupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SLogger.Write($"환경설정창 열기");
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.Inst.Dispose();
        }


        //#12_MODEL SAVE#3 모델 파일 열기,저장, 다른 이름으로 저장 기능 구현
        private string GetMdoelTitle(Model curModel)
        {
            if (curModel is null)
                return "";

            string modelName = curModel.ModelName;
            return $"{Define.PROGRAM_NAME} - MODEL : {modelName}";
        }

        private void modelNewMenuItem_Click(object sender, EventArgs e)
        {
            //신규 모델 추가를 위한 모델 정보를 받기 위한 창 띄우기
            NewModel newModel = new NewModel();
            newModel.ShowDialog();

            Model curModel = Global.Inst.InspStage.CurModel;
            if (curModel != null)
            {
                this.Text = GetMdoelTitle(curModel);
            }
        }

        private void modelOpenMenuItem_Click(object sender, EventArgs e)
        {
            //모델 파일 열기
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "모델 파일 선택";
                openFileDialog.Filter = "Model Files|*.xml;";
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (Global.Inst.InspStage.LoadModel(filePath))
                    {
                        Model curModel = Global.Inst.InspStage.CurModel;
                        if (curModel != null)
                        {
                            this.Text = GetMdoelTitle(curModel);
                        }
                    }
                }
            }
        }

        private void modelSaveMenuItem_Click(object sender, EventArgs e)
        {
            //모델 파일 저장
            Global.Inst.InspStage.SaveModel("");
        }

        private void modelSaveAsMenuItem_Click(object sender, EventArgs e)
        {
            //다른이름으로 모델 파일 저장
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = SettingXml.Inst.ModelDir;
                saveFileDialog.Title = "모델 파일 선택";
                saveFileDialog.Filter = "Model Files|*.xml;";
                saveFileDialog.DefaultExt = "xml";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    Global.Inst.InspStage.SaveModel(filePath);
                }
            }
        }

        private void autoTeachingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (_autoTeachingForm != null && !_autoTeachingForm.IsDisposed)
            {
                _autoTeachingForm.Activate();
                return;
            }

            CameraForm cameraForm = GetDockForm<CameraForm>();
            if (cameraForm == null)
            {
                MessageBox.Show("CameraForm을 찾을 수 없습니다.");
                return;
            }

            _autoTeachingForm = new AutoTeaching(cameraForm);

            InspWindow curWindow = Global.Inst.InspStage.CurInspWindow;

            MatchAlgorithm matchAlgo = null;
            OpenCvSharp.Rect roi = new OpenCvSharp.Rect(0, 0, 0, 0);

            if (curWindow != null)
            {
                var algo = curWindow.FindInspAlgorithm(InspectType.InspMatch);
                matchAlgo = algo as MatchAlgorithm;

                var area = curWindow.WindowArea; // System.Drawing.Rectangle
                roi = new OpenCvSharp.Rect(area.X, area.Y, area.Width, area.Height);
            }

            Mat src = Global.Inst.InspStage.GetMat();
            if (src == null)
            {
                MessageBox.Show("원본 이미지가 없습니다.");
                _autoTeachingForm.Show(this);
                return;
            }

            _autoTeachingForm.InitForTeach(matchAlgo, src, roi);
            _autoTeachingForm.Show(this);
        }
    }
}
