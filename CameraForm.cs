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
using JH_VisionProject.Setting;
using JH_VisionProject.Teach;
using JH_VisionProject.UIControl;
using JH_VisionProject.Utill;
using OpenCvSharp;
using WeifenLuo.WinFormsUI.Docking;

namespace JH_VisionProject
{
    //public partial class CameraForm : Form

    public partial class CameraForm : DockContent
    {
        private string _currentImagePath;
        private Bitmap _currentBitmap;

        // ★ AutoFindROI를 필드로 유지
        private AutoFindROI _autoFindRoi;

        public CameraForm()
        {
            InitializeComponent();

            imageViewer.DiagramEntityEvent += ImageViewer_DiagramEntityEvent;

            // ★ AutoFindROI 인스턴스를 필드로 생성
            _autoFindRoi = new AutoFindROI();
            _autoFindRoi.ResultImageUpdated += AutoFindROI_ResultImageUpdated;
            _autoFindRoi.RoiFound += AutoFindROI_RoiFound;   // ★ 찾은 ROI를 받는 이벤트 핸들러
        }

        private void AutoFindROI_RoiFound(List<OpenCvSharp.Rect> rois)
        {
            if (rois == null || rois.Count == 0)
                return;

            // 기존 모델 기반 ROI는 건드리지 않고,
            // "찾은 ROI들만" 임시로 표시하고 싶다면 별 리스트로 관리해야 함.
            // 여기서는 간단하게: 찾은 ROI들을 DiagramEntity로 만들어 imageViewer에 뿌리는 예를 보여줌.

            List<DiagramEntity> foundEntities = new List<DiagramEntity>();

            foreach (var r in rois)
            {
                var rect = new Rectangle(r.X, r.Y, r.Width, r.Height);

                DiagramEntity entity = new DiagramEntity()
                {
                    LinkedWindow = null,  // 아직 모델에 붙이진 않은 상태라면 null
                    EntityROI = rect,
                    EntityColor = Color.Lime,   // AutoFind 결과 표시용 색
                    IsHold = false
                };

                foundEntities.Add(entity);
            }

            // 1) 기존 모델 ROI도 같이 보이고 싶다면:
            //    - Model에서 만든 DiagramEntity 리스트 + foundEntities를 합쳐서 SetDiagramEntityList에 넣어주면 된다.
            // 2) 지금은 "찾은 ROI들만" 간단히 덮어쓰는 예시:

            imageViewer.SetDiagramEntityList(foundEntities);
        }

        private void ImageViewer_DiagramEntityEvent(object sender, DiagramEntityEventArgs e)
        {
            SLogger.Write($"ImageViewer Action {e.ActionType.ToString()}");
            switch (e.ActionType)
            {
                case EntityActionType.Select:
                    Global.Inst.InspStage.SelectInspWindow(e.InspWindow);
                    imageViewer.Focus();

                    // ROI 자동 전달
                    UpdateAutoFindRoiFromWindow(e.InspWindow);
                    break;

                    break;
                case EntityActionType.Inspect:
                    UpdateDiagramEntity();
                    Global.Inst.InspStage.TryInspection(e.InspWindow);

                    // 검사 버튼(Inspect)로 선택했을 때도 ROI 전달
                    UpdateAutoFindRoiFromWindow(e.InspWindow);
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
        private void UpdateAutoFindRoiFromWindow(InspWindow window)
        {
            if (_autoFindRoi == null)
                return;

            if (window == null)
                return;

            // 1) 선택된 윈도우의 ROI를 AutoFindROI에 전달
            var area = window.WindowArea;
            var roi = new OpenCvSharp.Rect(area.X, area.Y, area.Width, area.Height);
            _autoFindRoi.SetRoi(roi);

            // 2) 원본 Mat도 같이 전달 (채널 없이 전체 이미지)
            Mat mat = Global.Inst.InspStage.GetMat();  // 이미 다른 곳에서 쓰는 패턴과 동일
            _autoFindRoi.SetSourceImage(mat);

            // 3) 매칭 알고리즘 인스턴스도 넘겨주기
            MatchAlgorithm matchAlgo = window.FindInspAlgorithm(InspectType.InspMatch) as MatchAlgorithm;
            if (matchAlgo != null)
            {
                _autoFindRoi.SetAlgorithm(matchAlgo);
            }
        }
        private void AutoFindROI_ResultImageUpdated(Bitmap resultBmp)
        {
            if (resultBmp == null)
                return;

            // imageViewer에 표시
            imageViewer.LoadBitmap(resultBmp);   // CameraForm.UpdateDisplay와 같은 방식[file:44]

            // 필요하면 현재 비트맵으로 캐싱
            _currentBitmap?.Dispose();
            _currentBitmap = (Bitmap)resultBmp.Clone();
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

        //#13_INSP_RESULT#6 검사 양불판정 갯수 설정 함수
        public void SetInspResultCount(int totalArea, int okCnt, int ngCnt)
        {
            imageViewer.SetInspResultCount(new InspectResultCount(totalArea, okCnt, ngCnt));
        }


        //#17_WORKING_STATE#5 작업 상태 화면 표시 설정
        public void SetWorkingState(WorkingState workingState)
        {
            string state = "";
            switch (workingState)
            {
                case WorkingState.INSPECT:
                    state = "INSPECT";
                    break;

                case WorkingState.LIVE:
                    state = "LIVE";
                    break;

                case WorkingState.ALARM:
                    state = "ALARM";
                    break;
            }

            imageViewer.WorkingState = state;
            imageViewer.Invalidate();
        }

    }
}
