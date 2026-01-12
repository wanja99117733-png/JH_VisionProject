using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JH_VisionProject.Algorithm;
using JH_VisionProject.Core;
using JH_VisionProject.Teach;
using JH_VisionProject.UIControl;
using OpenCvSharp;
using WeifenLuo.WinFormsUI.Docking;

namespace JH_VisionProject
{
    //public partial class CameraForm : Form

    public partial class CameraForm : DockContent
    {
        private string _currentImagePath;
        private Bitmap _currentBitmap;
        public CameraForm()
        {
            InitializeComponent();
            //#10_INSPWINDOW#23 ImageViewCtrl에서 발생하는 이벤트 처리
            imageViewer.DiagramEntityEvent += ImageViewer_DiagramEntityEvent;
        }

        private void ImageViewer_DiagramEntityEvent(object sender, DiagramEntityEventArgs e)
        {
            switch (e.ActionType)
            {
                case EntityActionType.Select:
                    Global.Inst.InspStage.SelectInspWindow(e.InspWindow);
                    imageViewer.Focus();
                    break;
                case EntityActionType.Inspect:
                    UpdateDiagramEntity();
                    Global.Inst.InspStage.TryInspection(e.InspWindow);
                    break;
                case EntityActionType.Add:
                    Global.Inst.InspStage.AddInspWindow(e.WindowType, e.Rect);
                    break;
                case EntityActionType.Copy:
                    Global.Inst.InspStage.AddInspWindow(e.InspWindow, e.OffsetMove);
                    break;
                case EntityActionType.Move:
                    Global.Inst.InspStage.MoveInspWindow(e.InspWindow, e.OffsetMove);
                    break;
                case EntityActionType.Resize:
                    Global.Inst.InspStage.ModifyInspWindow(e.InspWindow, e.Rect);
                    break;
                case EntityActionType.Delete:
                    Global.Inst.InspStage.DelInspWindow(e.InspWindow);
                    break;
                case EntityActionType.DeleteList:
                    Global.Inst.InspStage.DelInspWindow(e.InspWindowList);
                    break;
            }
        }
        public string CurrentImagePath => _currentImagePath;
        public Bitmap CurrentBitmap => _currentBitmap;

        private void CameraForm_Load(object sender, EventArgs e)
        {

        }
        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (bitmap == null)
            {
                //#6_INSP_STAGE#3 업데이트시 bitmap이 없다면 InspSpace에서 가져온다
                bitmap = Global.Inst.InspStage.GetBitmap(0);
                if (bitmap == null)
                    return;
            }

            if (imageViewer != null)
                imageViewer.LoadBitmap(bitmap);

            //#7_BINARY_PREVIEW#10 현재 선택된 이미지로 Previwe이미지 갱신
            //이진화 프리뷰에서 각 채널별로 설정이 적용되도록, 현재 이미지를 프리뷰 클래스 설정            
            Mat curImage = Global.Inst.InspStage.GetMat();
            Global.Inst.InspStage.PreView.SetImage(curImage);
        }

        public void LoadImage(string filename)
        {
            if (File.Exists(filename) == false)
                return;

            Image bitmap = Image.FromFile(filename);
            imageViewer.LoadBitmap((Bitmap)bitmap);

            _currentImagePath = filename;

            _currentBitmap?.Dispose();
            _currentBitmap = (Bitmap)bitmap.Clone();
        }

        public Mat GetDisplayImage()
        {
            return Global.Inst.InspStage.ImageSpace.GetMat();
        }

        private void imageViewer_Load(object sender, EventArgs e)
        {

        }

        private void CameraForm_Resize(object sender, EventArgs e)
        {

        }

        //#10_INSPWINDOW#23 모델 정보를 이용해, ROI 갱신
        public void UpdateDiagramEntity()
        {
            imageViewer.ResetEntity();

            Model model = Global.Inst.InspStage.CurModel;
            List<DiagramEntity> diagramEntityList = new List<DiagramEntity>();

            foreach (InspWindow window in model.InspWindowList)
            {
                if (window is null)
                    continue;

                DiagramEntity entity = new DiagramEntity()
                {
                    LinkedWindow = window,
                    EntityROI = new Rectangle(
                        window.WindowArea.X, window.WindowArea.Y,
                            window.WindowArea.Width, window.WindowArea.Height),
                    EntityColor = imageViewer.GetWindowColor(window.InspWindowType),
                    IsHold = window.IsTeach
                };
                diagramEntityList.Add(entity);
            }

            imageViewer.SetDiagramEntityList(diagramEntityList);
        }

        public void SelectDiagramEntity(InspWindow window)
        {
            imageViewer.SelectDiagramEntity(window);
        }

        public void UpdateImageViewer()
        {
            imageViewer.UpdateInspParam();
            imageViewer.Invalidate();
        }

        //#8_INSPECT_BINARY#18 imageViewer에 검사 결과 정보를 연결해주기 위한 함수
        public void ResetDisplay()
        {
            imageViewer.ResetEntity();
        }

        //FIXME 검사 결과를 그래픽으로 출력하기 위한 정보를 받는 함수
        public void AddRect(List<DrawInspectInfo> rectInfos)
        {
            imageViewer.AddRect(rectInfos);
        }

        //#10_INSPWINDOW#24 새로운 ROI를 추가하는 함수
        public void AddRoi(InspWindowType inspWindowType)
        {
            imageViewer.NewRoi(inspWindowType);
        }
    }
}
