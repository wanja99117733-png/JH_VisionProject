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
            var cameraForm = new CameraForm();
            cameraForm.Show(_dockPanel, DockState.Document); 
            // show클래스는 메인폼에서는 한개밖에 안됨 근데 DockContent를 cameraForm에 선언함으로 여러개 가능


            var runWindow = new RunForm();
            runWindow.Show(cameraForm.Pane, DockAlignment.Bottom, 0.3);

            var propForm = new PropertiesForm();                // var는 자동형식지정
            propForm.Show(_dockPanel, DockState.DockRight);

            var statisticForm = new StatisticForm();
            statisticForm.Show(_dockPanel, DockState.DockRight);

            var logForm = new LogForm();
            logForm.Show(propForm.Pane, DockAlignment.Bottom, 0.35);

        }
        //제네릭 함수 사용을 이용해 입력된 타입의 폼 객체 얻기
        public static T GetDockContent<T>() where T : DockContent 
        {
            var findForm = _dockPanel.Contents.OfType<T>().FirstOrDefault();    
            return findForm;
        }
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
                    cameraForm.LoadImage(filePath);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
