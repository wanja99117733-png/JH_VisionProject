using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using JH_VisionProject.Algorithm;
using JH_VisionProject.Core;

namespace JH_VisionProject.UIControl
{
    public partial class ImageViewCtrl : UserControl
    {
        private bool _isInitialized = false;
        // 초기화 여부

        private Bitmap _bitmapImage = null;
        // 현재 표시된 이미지

        private Bitmap Canvas = null;
        // 이미지를 그릴 캔버스

        private RectangleF ImageRect = new RectangleF(0, 0, 0, 0);
        // 이미지가 그려질 렉트

        private float _curZoom = 1.0f;          //배율 설정
        private float _zoomFactor = 1.1f;       //배율 증감 비율

        private float MinZoom = 0.1f;          //최소 배율 설정
        private float MaxZoom = 100.0f;        //최대 배율 설정

        //#8_INSPECT_BINARY#15 템플릿 매칭 결과 출력을 위해 Rectangle 리스트 변수 설정
        private List<DrawInspectInfo> _rectInfos = new List<DrawInspectInfo>();

        public ImageViewCtrl()
        {
            InitializeComponent();
            IntilaizeCanvas();

            MouseWheel += new MouseEventHandler(ImageViewCCtrl_MouseWheel);
        }

        public void LoadBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return; // 또는 throw new ArgumentNullException(nameof(bitmap));

            // 기존에 로드된 이미지가 있다면 해제 후 초기화, 메모리누수 방지
            if (_bitmapImage != null)
            {
                //이미지 크기가 같다면, 이미지 변경 후, 화면 갱신
                if (_bitmapImage.Width == bitmap.Width && _bitmapImage.Height == bitmap.Height)
                {
                    _bitmapImage = bitmap;
                    Invalidate();
                    return;
                }

                _bitmapImage.Dispose(); // Bitmap 객체가 사용하던 메모리 리소스를 해제
                _bitmapImage = null;  //객체를 해제하여 가비지 컬렉션(GC)이 수집할 수 있도록 설정
            }

            // 새로운 이미지 로드
            _bitmapImage = bitmap;

            ////bitmap==null 예외처리도 초기화되지않은 변수들 초기화
            if (_isInitialized == false)
            {
                _isInitialized = true;
                ResizeCanvas();
            }

            FitToImageToScrean();
        }
        private void ResizeCanvas() // 캔버스 크기 재설정
        {
            if (Width <= 0 || Height <= 0)
                return;

            // 기존 캔버스 해제
            if (Canvas != null)
            {
                Canvas.Dispose();
                Canvas = null;
            }

            Canvas = new Bitmap(Width, Height);

            // 이미지가 아직 없으면 여기까지만
            if (_bitmapImage == null)
            {
                ImageRect = new RectangleF(0, 0, Width, Height);
                return;
            }

            float virtualWidth = _bitmapImage.Width * _curZoom;
            float virtualHeight = _bitmapImage.Height * _curZoom;

            float offsetX = virtualWidth < Width ? (Width - virtualWidth) / 2 : 0;
            float offsetY = virtualHeight < Height ? (Height - virtualHeight) / 2 : 0;

            ImageRect = new RectangleF(offsetX, offsetY, virtualWidth, virtualHeight);
        }

            /*
            if (Width <= 0 || Height <= 0 || _bitmapImage == null)  //크기가 0이하이거나 이미지가 없을때
                return;

            Canvas = new Bitmap(Width, Height);
            // 캔버스 비트맵 생성
            if (Canvas == null)
                return;

            float virtualWidth = _bitmapImage.Width * _curZoom;                             //가상 이미지의 가로 크기
            float virtualHeight = _bitmapImage.Height * _curZoom;                           //가상 이미지의 세로 크기

            float offsetX = virtualWidth < Width ? (Width - virtualWidth) / 2 : 0;          //가로 중앙 정렬
            float offsetY = virtualHeight < Height ? (Height - virtualHeight) / 2 : 0;      //세로 중앙 정렬

            ImageRect = new RectangleF(offsetX, offsetY, virtualWidth, virtualHeight);      //이미지 렉트 설정*/
        

        private void FitToImageToScrean()     // 화면에 이미지 맞추기
        {
            RecalcZoomRatio();

            float NewWidth = _bitmapImage.Width * _curZoom;
            float NewHeight = _bitmapImage.Height * _curZoom;

            ImageRect = new RectangleF(
                (Width - NewWidth) / 2,
                (Height - NewHeight) / 2,
                NewWidth,
                NewHeight);

            Invalidate();
        }

        private void RecalcZoomRatio() // 배율 재계산
        {
            if (_bitmapImage == null || Width <= 0 || Height <= 0)   //이미지가 없거나 컨트롤 크기가 0이하일때
                return;

            Size imageSize = new Size(_bitmapImage.Width, _bitmapImage.Height);
            // 이미지 크기 가져오기

            float aspectRatio = (float)imageSize.Height / imageSize.Width;  //이미지 종횡비
            float clinetAspect = (float)Height / (float)Width;              //컨트롤 종횡비

            float ratio;
            if (aspectRatio <= clinetAspect)    //이미지 종횡비가 컨트롤 종횡비보다 작거나 같을때
            {
                ratio = (float)Width / (float)imageSize.Width;      //가로 기준 비율 계산
            }
            else
            {
                ratio = (float)Height / (float)imageSize.Height;    //세로 기준 비율 계산
            }

            float minZoom = ratio;

            MinZoom = minZoom;

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, ratio));

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Canvas 없으면 그리지 않음
            if (Canvas == null)
                return;

            using (Graphics g = Graphics.FromImage(Canvas))
            {
                g.Clear(Color.BlueViolet);

                if (_bitmapImage != null)
                {
                    g.InterpolationMode =
                        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(_bitmapImage, ImageRect);
                }
                
            //#8_INSPECT_BINARY#16 _rectInfos 그리기
                DrawDiagram(g);
            }
            

            e.Graphics.DrawImage(Canvas, 0, 0);
        

            /*
            base.OnPaint(e);

            if (_bitmapImage != null && Canvas != null)
            {
                using (Graphics g = Graphics.FromImage(Canvas))
                {
                    g.Clear(Color.BlueViolet); // 배경 투명하게 설정

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    //이미지 확대 or 축소때 화질 최적화 방식(Interplation Mode) 설정
                    g.DrawImage(_bitmapImage, ImageRect); // 이미지 그리기

                    e.Graphics.DrawImage(Canvas, 0, 0); // 캔버스 그리기
                }
            }*/
        }

        private void DrawDiagram(Graphics g)
        {
            // 이미지 좌표 → 화면 좌표 변환 후 사각형 그리기
            if (_rectInfos != null)
            {
                foreach (DrawInspectInfo rectInfo in _rectInfos)
                {
                    Color lineColor = Color.LightCoral;
                    if (rectInfo.decision == DecisionType.Defect)
                        lineColor = Color.Red;
                    else if (rectInfo.decision == DecisionType.Good)
                        lineColor = Color.LightGreen;

                    Rectangle rect = new Rectangle(rectInfo.rect.X, rectInfo.rect.Y, rectInfo.rect.Width, rectInfo.rect.Height);
                    Rectangle screenRect = VirtualToScreen(rect);

                    using (Pen pen = new Pen(lineColor, 2))
                    {
                        if (rectInfo.UseRotatedRect)
                        {
                            PointF[] screenPoints = rectInfo.rotatedPoints
                                                    .Select(p => VirtualToScreen(new PointF(p.X, p.Y))) // 화면 좌표계로 변환
                                                    .ToArray();

                            if (screenPoints.Length == 4)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    g.DrawLine(pen, screenPoints[i], screenPoints[(i + 1) % 4]); // 시계방향으로 선 연결
                                }
                            }
                        }
                        else
                        {
                            g.DrawRectangle(pen, screenRect);
                        }
                    }

                    if (rectInfo.info != "")
                    {
                        float baseFontSize = 20.0f;

                        if (rectInfo.decision == DecisionType.Info)
                        {
                            baseFontSize = 3.0f;
                            lineColor = Color.LightBlue;
                        }

                        float fontSize = baseFontSize * _curZoom;

                        // 스코어 문자열 그리기 (우상단)
                        string infoText = rectInfo.info;
                        PointF textPos = new PointF(screenRect.Left, screenRect.Top); // 위로 약간 띄우기

                        if (rectInfo.inspectType == InspectType.InspBinary
                            && rectInfo.decision != DecisionType.Info)
                        {
                            textPos.Y = screenRect.Bottom - fontSize;
                        }

                        DrawText(g, infoText, textPos, fontSize, lineColor);
                    }
                }
            }
        }

        private void DrawText(Graphics g, string text, PointF position, float fontSize, Color color)
        {
            using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
            // 테두리용 검정색 브러시
            using (Brush outlineBrush = new SolidBrush(Color.Black))
            // 본문용 노란색 브러시
            using (Brush textBrush = new SolidBrush(color))
            {
                // 테두리 효과를 위해 주변 8방향으로 그리기
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue; // 가운데는 제외
                        PointF borderPos = new PointF(position.X + dx, position.Y + dy);
                        g.DrawString(text, font, outlineBrush, borderPos);
                    }
                }

                // 본문 텍스트
                g.DrawString(text, font, textBrush, position);
            }
        }

        public Bitmap GetCurBitmap()
        {
            return _bitmapImage;
        }

        private void IntilaizeCanvas()
        {
            ResizeCanvas();

            DoubleBuffered = true;
        }
        private PointF GetScreenOffset()
        {
            return new PointF(ImageRect.X, ImageRect.Y);
        }

        private PointF ScreenToVirtual(PointF screenPos)
        {
            PointF offset = GetScreenOffset();
            return new PointF(
                (screenPos.X - offset.X) / _curZoom,
                (screenPos.Y - offset.Y) / _curZoom);
        }
        private Rectangle ScreenToVirtual(Rectangle screenRect)
        {
            PointF offset = GetScreenOffset();
            return new Rectangle(
                (int)((screenRect.X - offset.X) / _curZoom + 0.5f),
                (int)((screenRect.Y - offset.Y) / _curZoom + 0.5f),
                (int)(screenRect.Width / _curZoom + 0.5f),
                (int)(screenRect.Height / _curZoom + 0.5f));
        }
        private Rectangle VirtualToScreen(Rectangle virtualRect)
        {
            PointF offset = GetScreenOffset();
            return new Rectangle(
                (int)(virtualRect.X * _curZoom + offset.X + 0.5f),
                (int)(virtualRect.Y * _curZoom + offset.Y + 0.5f),
                (int)(virtualRect.Width * _curZoom + 0.5f),
                (int)(virtualRect.Height * _curZoom + 0.5f));
        }
        private PointF VirtualToScreen(PointF virtualPos)
        {
            PointF offset = GetScreenOffset();
            return new PointF(
                virtualPos.X * _curZoom + offset.X,
                virtualPos.Y * _curZoom + offset.Y);
        }
        private void ZoomMove(float zoom, Point zoomOrigin)
        {
            PointF virtualOrigin = ScreenToVirtual(new PointF(zoomOrigin.X, zoomOrigin.Y));

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, zoom));
            if (_curZoom <= MinZoom)
                return;

            PointF zoomedOrigin = VirtualToScreen(virtualOrigin);

            float dx = zoomedOrigin.X - zoomOrigin.X;
            float dy = zoomedOrigin.Y - zoomOrigin.Y;

            ImageRect.X -= dx;
            ImageRect.Y -= dy;
        }
        private void ImageViewCCtrl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                ZoomMove(_curZoom / _zoomFactor, e.Location);
            else
                ZoomMove(_curZoom * _zoomFactor, e.Location);

            if (_bitmapImage != null)
            {
                ImageRect.Width = _bitmapImage.Width * _curZoom;
                ImageRect.Height = _bitmapImage.Height * _curZoom;
            }

            // 다시 그리기 요청
            Invalidate();
        }

 
        

        private void ImageViewCtrl_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            FitToImageToScrean();
        }

        private void ImageViewCtrl_Resize_1(object sender, EventArgs e)
        {
            ResizeCanvas();
            Invalidate();
        }

        //#8_INSPECT_BINARY#17 화면에 보여줄 영역 정보를 표시하기 위해, 위치 입력 받는 함수
        public void AddRect(List<DrawInspectInfo> rectInfos)
        {
            _rectInfos.AddRange(rectInfos);
            Invalidate();
        }

        public void ResetEntity()
        {
            _rectInfos.Clear();
            Invalidate();
        }
    }
}
