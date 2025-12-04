using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        public ImageViewCtrl()
        {
            InitializeComponent();
            IntilaizeCanvas();

            MouseWheel += new MouseEventHandler(ImageViewCCtrl_MouseWheel);
        }

        public void LoadBitmap(Bitmap bitmap)
        {
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

            ImageRect = new RectangleF(offsetX, offsetY, virtualWidth, virtualHeight);      //이미지 렉트 설정
        }

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

            if (_bitmapImage != null && Canvas != null)
            {
                using (Graphics g = Graphics.FromImage(Canvas))
                {
                    g.Clear(Color.Black); // 배경 투명하게 설정

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    //이미지 확대 or 축소때 화질 최적화 방식(Interplation Mode) 설정
                    g.DrawImage(_bitmapImage, ImageRect); // 이미지 그리기

                    e.Graphics.DrawImage(Canvas, 0, 0); // 캔버스 그리기
                }
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
    }
}
