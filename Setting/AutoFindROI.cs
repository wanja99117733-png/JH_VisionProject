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
        // CameraForm으로 결과 이미지를 보내기 위한 이벤트
        public event Action<Bitmap> ResultImageUpdated;

        private MatchAlgorithm _matchAlgo;
        private Mat _srcImage;
        private OpenCvSharp.Rect _currentRoi;
        public AutoFindROI()
        {
            InitializeComponent();

            // 텍스트 박스에서 포커스 빠질 때 값 검증
            txtScore.Leave += txtScore_Leave;

            // 버튼 클릭 시 템플릿 찾기
            btnApply.Click += btnApply_Click;
        }
        // 외부에서 알고리즘, 이미지, ROI 주입
        public void SetAlgorithm(MatchAlgorithm matchAlgo)
        {
            _matchAlgo = matchAlgo;
            if (_matchAlgo != null)
                txtScore.Text = _matchAlgo.MatchScore.ToString();
        }

        public void SetSourceImage(Mat src)
        {
            _srcImage = src;
        }

        public void SetRoi(OpenCvSharp.Rect roi)
        {
            _currentRoi = roi;
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

            // ROI에 대한 템플릿 매칭 수행
            Mat resultMat = RunMatchInRoi(_srcImage, _currentRoi, _matchAlgo);

            // Bitmap으로 변환해서 CameraForm 쪽으로 전달
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

        /// <summary>
        /// ROI 내부에서 템플릿 매칭을 수행하고, 기준 점수 이상 매칭에 사각형을 그린 Mat 반환
        /// </summary>
        private Mat RunMatchInRoi(Mat src, OpenCvSharp.Rect roiRect, MatchAlgorithm matchAlgo)
        {
            List<Mat> templates = matchAlgo.GetTemplateImages();
            if (templates == null || templates.Count == 0)
            {
                MessageBox.Show("템플릿 이미지가 없습니다.");
                return src;
            }

            Mat template = templates[0];

            // 1) 전체 이미지에서 템플릿 매칭 수행
            //    leftTopPos = (0,0) 으로 해서 결과 좌표가 src 기준이 되게 함
            int count = matchAlgo.MatchTemplateMultiple(
                            src,                 // 전체 이미지
                            new OpenCvSharp.Point(0, 0),
                            out List<OpenCvSharp.Point> matchedPositions);

            Mat drawImg = src.Clone();

            if (count <= 0)
                return drawImg;

            // 2) “표시할 사각형 크기”를 ROI 크기로 사용
            int roiW = roiRect.Width;
            int roiH = roiRect.Height;

            foreach (var pos in matchedPositions)
            {
                Console.WriteLine($"MatchPos: X={pos.X}, Y={pos.Y}, W={roiW}, H={roiH}");
                // pos 는 src 기준의 좌상단 좌표
                var p1 = pos;
                var p2 = new OpenCvSharp.Point(pos.X + roiW, pos.Y + roiH);

                Cv2.Rectangle(drawImg, p1, p2, Scalar.Lime, 2);
            }

            return drawImg;
        }
    }
}

