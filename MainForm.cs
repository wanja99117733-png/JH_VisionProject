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
using JH_VisionProject.Setting;
using WeifenLuo.WinFormsUI.Docking;

namespace JH_VisionProject
{
    public partial class MainForm : Form
    {
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

            //메인폼 설정
            var runWindow = new RunForm();
            runWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.3);

            //#11_MODEL_TREE#1 검사 결과창 우측에 40% 비율로 모델트리 추가
            var modelTreeWindow = new ModelTreeForm();
            modelTreeWindow.Show(runWindow.Pane, DockAlignment.Right, 0.3);

            //속성창 추가
            var propWindow = new PropertiesForm();
            propWindow.Show(_dockPanel, DockState.DockRight);

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
    }
}
