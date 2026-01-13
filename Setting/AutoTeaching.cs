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
using JH_VisionProject.Setting;
using OpenCvSharp;
using JH_VisionProject.UIControl;
using JH_VisionProject.Core;


namespace JH_VisionProject
{
    public partial class AutoTeaching : Form
    {
        private CameraForm _cameraForm;   // 메인 카메라 폼 참조

        public AutoTeaching(CameraForm cameraForm)
        {
            InitializeComponent();

            _cameraForm = cameraForm;

            // AutoFindROI 이벤트 구독
            autoFindROI1.ResultImageUpdated += AutoFindROI1_ResultImageUpdated;
            autoFindROI1.RoiFound += AutoFindROI1_RoiFound;
        }

        public void InitForTeach(MatchAlgorithm matchAlgo, Mat src, Rect roi)
        {
            autoFindROI1.SetAlgorithm(matchAlgo);
            autoFindROI1.SetSourceImage(src);
            autoFindROI1.SetRoi(roi);
        }

        private void AutoFindROI1_ResultImageUpdated(Bitmap bmp)
        {
            // CameraForm 이미지 갱신
            _cameraForm?.UpdateDisplay(bmp);
        }

        private void AutoFindROI1_RoiFound(List<OpenCvSharp.Rect> rois)
        {
            if (rois == null || rois.Count == 0)
                return;

            var drawInfos = new List<DrawInspectInfo>();
            foreach (var r in rois)
            {
                drawInfos.Add(new DrawInspectInfo(
                    new OpenCvSharp.Rect(r.X, r.Y, r.Width, r.Height),
                    "", InspectType.InspMatch, DecisionType.Good));
            }

            // WindowArea는 변경하지 않고, CameraForm에 '추가 사각형'으로만 전달
            _cameraForm?.AddRect(drawInfos);

            // 필요 시 뷰 갱신
            _cameraForm?.UpdateImageViewer();
        }
    }
}
