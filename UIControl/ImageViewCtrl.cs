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
        }

        private void ResizeCanvas() // 캔버스 크기 재설정
        {
            if (Width <= 0 || Height <= 0 || _bitmapImage == null)
                return;

            Canvas = new Bitmap(Width, Height);
            if(Canvas == null)
                return;

            float virtualWidth = _bitmapImage.Width * _curZoom;                             //가상 이미지의 가로 크기
            float virtualHeight = _bitmapImage.Height * _curZoom;                           //가상 이미지의 세로 크기

            float offsetX = virtualWidth < Width ? (Width - virtualWidth) / 2 : 0;          //가로 중앙 정렬
            float offsetY = virtualHeight < Height ? (Height - virtualHeight) / 2 : 0;      //세로 중앙 정렬

            ImageRect = new RectangleF(offsetX, offsetY, virtualWidth, virtualHeight);      //이미지 렉트 설정
        }

        private void FitImageToScrean()
        {
            RecalcZoomRatio();

            float NewWidth = _bitmapImage.Width * _curZoom;
            float NewHeight = _bitmapImage.Height * _curZoom;

            ImageRect = new RectangleF((Width - NewWidth) / 2, (Height - NewHeight) / 2,
                                        NewWidth, NewHeight);
        }

        private void RecalcZoomRatio()
        {
            if(_bitmapImage == null || Width <= 0 || Height <= 0)
                return;

            Size imageSize = new Size(_bitmapImage.Width, _bitmapImage.Height);

            float aspectRatio = (float)imageSize.Height / imageSize.Width;
            float clinetAspect = (float)Height / (float)Width;

            float ratio;
            if (aspectRatio <= clinetAspect)
            {
                ratio = (float)Width / (float)imageSize.Width;
            }
            else
            {
                ratio = (float)Height / (float)imageSize.Height;
            }

            float minZoom = ratio;

            minZoom = minZoom;

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, ratio));

        }
    }
}
