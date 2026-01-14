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
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace JH_VisionProject.Setting
{

    public partial class AutoFindROI : UserControl
    {
        // 매칭 결과 이미지
        public event Action<Bitmap> ResultImageUpdated;
        // 찾은 ROI들 (Mat 좌표 기준)
        public event Action<List<OpenCvSharp.Rect>> RoiFound;

        private MatchAlgorithm _matchAlgo;
        private Mat _srcImage;
        private OpenCvSharp.Rect _currentRoi;

        public void SetAlgorithm(MatchAlgorithm matchAlgo)
        {
            _matchAlgo = matchAlgo;
            if (_matchAlgo != null)
                txtScore.Text = _matchAlgo.MatchScore.ToString();
        }

        public void SetSourceImage(Mat src) => _srcImage = src;

        public void SetRoi(OpenCvSharp.Rect roi) => _currentRoi = roi;
        public AutoFindROI()
        {
            InitializeComponent();

            // 텍스트 박스에서 포커스 빠질 때 값 검증
            txtScore.Leave += txtScore_Leave;

            // 버튼 클릭 시 템플릿 찾기
            btnApply.Click += btnApply_Click;
        }
        

        private void lbScore_Click(object sender, EventArgs e)
        {

        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_matchAlgo == null)
            {
                MessageBox.Show("MatchAlgorithm 이 설정되지 않았습니다.");
                return;
            }

            if (_srcImage == null)
            {
                MessageBox.Show("원본 이미지가 없습니다.");
                return;
            }

            if (_currentRoi.Width <= 0 || _currentRoi.Height <= 0)
            {
                MessageBox.Show("ROI가 유효하지 않습니다.");
                return;
            }

            if (!int.TryParse(txtScore.Text, out int score))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                return;
            }

            _matchAlgo.MatchScore = score;

            Mat resultMat = RunMatchInRoi(_srcImage, _currentRoi, _matchAlgo);

            Bitmap resultBmp = BitmapConverter.ToBitmap(resultMat);
            ResultImageUpdated?.Invoke(resultBmp);
        }

        private void txtScore_Leave(object sender, EventArgs e)
        {
            if (_matchAlgo == null)
                return;

            if (!int.TryParse(txtScore.Text, out int score))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                txtScore.Text = _matchAlgo.MatchScore.ToString();
                return;
            }

            _matchAlgo.MatchScore = score;
        }

        private Mat RunMatchInRoi(Mat src, OpenCvSharp.Rect roiRect, MatchAlgorithm matchAlgo)
        {
            var templates = matchAlgo.GetTemplateImages();
            if (templates == null || templates.Count == 0)
            {
                MessageBox.Show("템플릿 이미지가 없습니다.");
                return src;
            }

            int count = matchAlgo.MatchTemplateMultiple(
                src,
                new OpenCvSharp.Point(0, 0),
                out List<OpenCvSharp.Point> matchedPositions,
                out List<float> matchedScores);

            // 더 이상 박스를 그리지 않을 거면, 그냥 원본 복사만
            Mat drawImg = src.Clone();
            if (count <= 0)
                return drawImg;

            int roiW = roiRect.Width;
            int roiH = roiRect.Height;

            var rois = new List<OpenCvSharp.Rect>();

            for (int i = 0; i < matchedPositions.Count; i++)
            {
                var pos = matchedPositions[i];
                var rect = new OpenCvSharp.Rect(pos.X, pos.Y, roiW, roiH);
                rois.Add(rect);

                // ★ 여기서는 아무 것도 그리지 않음
                // (그리기는 ImageViewCtrl에서 DiagramEntity로 처리)
            }

            // 찾은 ROI들만 알려주고,
            RoiFound?.Invoke(rois);

            // drawImg는 화면에 띄울 이미지 (필요하면 다른 가공만 추가)
            return drawImg;
        }
    }
}

