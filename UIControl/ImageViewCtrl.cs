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
using JH_VisionProject.Teach;

namespace JH_VisionProject.UIControl
{
    //#10_INSPWINDOW#15 ROI 편집에 따라 발생하는 이벤트 정의
    public enum EntityActionType
    {
        None = 0,
        Select,
        Inspect,
        Add,
        Copy,
        Move,
        Resize,
        Delete,
        DeleteList,
        UpdateImage
    }

    //#13_INSP_RESULT#3 검사 양불판정 갯수를 화면에 표시하기 위한 구조체
    public struct InspectResultCount
    {
        public int Total { get; set; }
        public int OK { get; set; }
        public int NG { get; set; }

        public InspectResultCount(int _totalCount, int _okCount, int _ngCount)
        {
            Total = _totalCount;
            OK = _okCount;
            NG = _ngCount;
        }
    }
    public partial class ImageViewCtrl : UserControl
    {

        //ROI를 추가,수정,삭제 등으로 변경 시, 이벤트 발생
        public event EventHandler<DiagramEntityEventArgs> DiagramEntityEvent;

        private bool _isInitialized = false;

        // 현재 로드된 이미지
        private Bitmap _bitmapImage = null;

        // 더블 버퍼링을 위한 캔버스
        // 더블버퍼링 : 화면 깜빡임을 방지하고 부드러운 펜더링위해 사용
        private Bitmap Canvas = null;

        // 화면에 표시될 이미지의 크기 및 위치
        // 부동 소수점(float) 좌표를 사용하는 사각형 구조체
        private RectangleF ImageRect = new RectangleF(0, 0, 0, 0);

        // 현재 줌 배율
        private float _curZoom = 1.0f;
        // 줌 배율 변경 시, 확대/축소 단위
        private float _zoomFactor = 1.1f;

        // 최소 및 최대 줌 제한 값
        private float MinZoom = 1.0f;
        private const float MaxZoom = 100.0f;

        //#8_INSPECT_BINARY#15 템플릿 매칭 결과 출력을 위해 Rectangle 리스트 변수 설정
        private List<DrawInspectInfo> _rectInfos = new List<DrawInspectInfo>();

        //#13_INSP_RESULT#4 검사 양불 판정 갯수를 화면에 표시하기 위한 변수
        private InspectResultCount _inspectResultCount = new InspectResultCount();

        
        private List<DiagramEntity> copyBuffer = new List<DiagramEntity>();

        //#10_INSPWINDOW#15 ROI 편집에 필요한 변수 선언
        private Point _roiStart = Point.Empty;
        private Rectangle _roiRect = Rectangle.Empty;
        private bool _isSelectingRoi = false;
        private bool _isResizingRoi = false;
        private bool _isMovingRoi = false;
        private Point _resizeStart = Point.Empty;
        private Point _moveStart = Point.Empty;
        private int _resizeDirection = -1;
        private const int _ResizeHandleSize = 10;

        //새로 추가할 ROI 타입
        private InspWindowType _newRoiType = InspWindowType.None;

        //여러개 ROI를 관리하기 위한 리스트
        private List<DiagramEntity> _diagramEntityList = new List<DiagramEntity>();

        //현재 선택된 ROI 리스트
        private List<DiagramEntity> _multiSelectedEntities = new List<DiagramEntity>();
        private List<DiagramEntity> _copyBuffer = new List<DiagramEntity>();
        private Point _mousePos;

        private DiagramEntity _selEntity;
        private Color _selColor = Color.White;

        private Rectangle _selectionBox = Rectangle.Empty;
        private bool _isBoxSelecting = false;
        private bool _isCtrlPressed = false;
        private Rectangle _screenSelectedRect = Rectangle.Empty;

        private Size _extSize = new Size(0, 0);

        // ROI 이동용: 이동 시작 시 기준 정보
        private Rectangle _moveStartRoiRect;                       // 단일 선택용
        private Dictionary<DiagramEntity, Rectangle> _moveStartRects =
            new Dictionary<DiagramEntity, Rectangle>();
        // 다중 선택용
        private bool _isGroupMove = false;

        //팝업 메뉴
        private ContextMenuStrip _contextMenu;
        public ImageViewCtrl()
        {
            InitializeComponent();
            IntilaizeCanvas();

            //#10_INSPWINDOW#16 화면상에서, 팝업 메뉴 띄우기
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Delete", null, OnDeleteClicked);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("Teaching", null, OnTeachingClicked);
            _contextMenu.Items.Add("Unlock", null, OnUnlockClicked);

            MouseWheel += new MouseEventHandler(ImageViewCCtrl_MouseWheel);

            _moveStartRects = new Dictionary<DiagramEntity, Rectangle>();

        }
        //#10_INSPWINDOW#17 InspWindow 타입에 따른, 칼라 정보 얻는 함수
        public Color GetWindowColor(InspWindowType inspWindowType)
        {
            Color color = Color.LightBlue;

            switch (inspWindowType)
            {
                case InspWindowType.Base:
                    color = Color.LightBlue;
                    break;
                case InspWindowType.Sub:
                    color = Color.Orange;
                    break;
                case InspWindowType.Body:
                    color = Color.Yellow;
                    break;
            }

            return color;
        }

        //모델트리로 부터 호출되어, 신규 ROI를 추가하도록 하는 기능 시작점
        public void NewRoi(InspWindowType inspWindowType)
        {
            _newRoiType = inspWindowType;
            _selColor = GetWindowColor(inspWindowType);
            Cursor = Cursors.Cross;
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
            //#10_INSPWINDOW#18 ROI 그리기
            _screenSelectedRect = new Rectangle(0, 0, 0, 0);
            foreach (DiagramEntity entity in _diagramEntityList)
            {
                Rectangle screenRect = VirtualToScreen(entity.EntityROI);
                using (Pen pen = new Pen(entity.EntityColor, 2))
                {
                    if (_multiSelectedEntities.Contains(entity))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        pen.Width = 2;

                        if (_screenSelectedRect.IsEmpty)
                        {
                            _screenSelectedRect = screenRect;
                        }
                        else
                        {
                            //선택된 roi가 여러개 일때, 전체 roi 영역 계산
                            //선택된 roi 영역 합치기
                            _screenSelectedRect = Rectangle.Union(_screenSelectedRect, screenRect);
                        }
                    }

                    g.DrawRectangle(pen, screenRect);
                }

                //선택된 ROI가 있다면, 리사이즈 핸들 그리기
                if (_multiSelectedEntities.Count <= 1 && entity == _selEntity)
                {
                    // 리사이즈 핸들 그리기 (8개 포인트: 4 모서리 + 4 변 중간)
                    using (Brush brush = new SolidBrush(Color.LightBlue))
                    {
                        Point[] resizeHandles = GetResizeHandles(screenRect);
                        foreach (Point handle in resizeHandles)
                        {
                            g.FillRectangle(brush, handle.X - _ResizeHandleSize / 2, handle.Y - _ResizeHandleSize / 2, _ResizeHandleSize, _ResizeHandleSize);
                        }
                    }
                }
            }

            //선택된 개별 roi가 없고, 여러개가 선택되었다면
            if (_multiSelectedEntities.Count > 1 && !_screenSelectedRect.IsEmpty)
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    g.DrawRectangle(pen, _screenSelectedRect);
                }

                // 리사이즈 핸들 그리기 (8개 포인트: 4 모서리 + 4 변 중간)
                using (Brush brush = new SolidBrush(Color.LightBlue))
                {
                    Point[] resizeHandles = GetResizeHandles(_screenSelectedRect);
                    foreach (Point handle in resizeHandles)
                    {
                        g.FillRectangle(brush, handle.X - _ResizeHandleSize / 2, handle.Y - _ResizeHandleSize / 2, _ResizeHandleSize, _ResizeHandleSize);
                    }
                }
            }

            //신규 ROI 추가할때, 해당 ROI 그리기
            if (_isSelectingRoi && !_roiRect.IsEmpty)
            {
                Rectangle rect = VirtualToScreen(_roiRect);
                using (Pen pen = new Pen(_selColor, 2))
                {
                    g.DrawRectangle(pen, rect);
                }
            }

            if (_multiSelectedEntities.Count <= 1 && _selEntity != null)
            {
                //#11_MATCHING#8 패턴매칭할 영역 표시
                DrawInspParam(g, _selEntity.LinkedWindow);
            }


            //선택 영역 박스 그리기
            if (_isBoxSelecting && !_selectionBox.IsEmpty)
            {
                using (Pen pen = new Pen(Color.LightSkyBlue, 3))
                {
                    pen.DashStyle = DashStyle.Dash;
                    pen.Width = 2;
                    g.DrawRectangle(pen, _selectionBox);
                }
            }

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

            //#13_INSP_RESULT#5 검사 양불판정 갯수 화면에 표시
            if (_inspectResultCount.Total > 0)
            {
                string resultText = $"Total: {_inspectResultCount.Total}\r\nOK: {_inspectResultCount.OK}\r\nNG: {_inspectResultCount.NG}";

                float fontSize = 12.0f;
                Color resultColor = Color.FromArgb(255, 255, 255);
                PointF textPos = new PointF(Width - 80, 10);
                DrawText(g, resultText, textPos, fontSize, resultColor);
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

        //#11_MATCHING#9 패턴매칭할 영역 크기 얻는 함수,
        //이 함수를 사용하는 코드도 참조 확인하여 추가할것
        public void UpdateInspParam()
        {
            _extSize.Width = _extSize.Height = 0;

            if (_selEntity is null)
                return;

            InspWindow window = _selEntity.LinkedWindow;
            if (window is null)
                return;

            MatchAlgorithm matchAlgo = (MatchAlgorithm)window.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                _extSize.Width = matchAlgo.ExtSize.Width;
                _extSize.Height = matchAlgo.ExtSize.Height;
            }
        }

        private void DrawInspParam(Graphics g, InspWindow window)
        {
            if (_extSize.Width > 0 || _extSize.Height > 0)
            {
                Rectangle extArea = new Rectangle(_roiRect.Left - _extSize.Width,
                    _roiRect.Top - _extSize.Height,
                    _roiRect.Width + _extSize.Width * 2,
                    _roiRect.Height + _extSize.Height * 2);
                Rectangle screenRect = VirtualToScreen(extArea);

                using (Pen pen = new Pen(Color.White, 2))
                {
                    pen.DashStyle = DashStyle.Dot;
                    pen.Width = 2;
                    g.DrawRectangle(pen, screenRect);
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
        public void SetInspResultCount(InspectResultCount inspectResultCount)
        {
            _inspectResultCount = inspectResultCount;
        }


        public void ResetEntity()
        {
            _rectInfos.Clear();
            Invalidate();
        }

        private void ImageViewCtrl_MouseDown(object sender, MouseEventArgs e)
        {
            _isCtrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

            //여러개 ROI 기능에 맞게 코드 수정
            if (e.Button == MouseButtons.Left)
            {
                if (_newRoiType != InspWindowType.None)
                {
                    //새로운 ROI 그리기 시작 위치 설저어
                    _roiStart = e.Location;
                    _isSelectingRoi = true;
                    _selEntity = null;
                }
                else
                {
                    if (!_isCtrlPressed && _multiSelectedEntities.Count > 1 && _screenSelectedRect.Contains(e.Location))
                    {
                        _isMovingRoi = true;
                        _isGroupMove = true;          // 그룹 이동 시작
                        _moveStart = e.Location;

                        // 이동 시작 시점의 각 ROI 원래 위치 저장
                        _moveStartRects.Clear();
                        foreach (var ent in _multiSelectedEntities)
                        {
                            if (ent == null || ent.IsHold) continue;
                            _moveStartRects[ent] = ent.EntityROI;
                        }
                        Invalidate();
                        return;
                    }

                    if (_selEntity != null && !_selEntity.IsHold)
                    {
                        Rectangle screenRect = VirtualToScreen(_selEntity.EntityROI);
                        //마우스 클릭 위치가 ROI 크기 변경을 하기 위한 위치(모서리,엣지)인지 여부 판단
                        _resizeDirection = GetResizeHandleIndex(screenRect, e.Location);
                        if (_resizeDirection != -1)
                        {
                            _isResizingRoi = true;
                            _resizeStart = e.Location;
                            Invalidate();
                            return;
                        }
                    }

                    _selEntity = null;
                    foreach (DiagramEntity entity in _diagramEntityList)
                    {
                        Rectangle screenRect = VirtualToScreen(entity.EntityROI);
                        if (!screenRect.Contains(e.Location))
                            continue;

                        //컨트롤키를 이용해, 개별 ROI 추가/제거
                        if (_isCtrlPressed)
                        {
                            if (_multiSelectedEntities.Contains(entity))
                                _multiSelectedEntities.Remove(entity);
                            else
                                AddSelectedROI(entity);
                        }
                        else
                        {
                            _multiSelectedEntities.Clear();
                            AddSelectedROI(entity);
                        }

                        _selEntity = entity;
                        _roiRect = entity.EntityROI;
                        _isMovingRoi = true;
                        _isGroupMove = false;              // 단일 이동
                        _moveStart = e.Location;

                        // 단일 이동용 기준 위치 저장
                        _moveStartRoiRect = _selEntity.EntityROI;

                        // 다중 선택이면서 여기로 들어올 수도 있으니, 그룹용도 준비
                        _moveStartRects.Clear();
                        foreach (var ent in _multiSelectedEntities)
                        {
                            if (ent == null || ent.IsHold) continue;
                            _moveStartRects[ent] = ent.EntityROI;
                        }

                        UpdateInspParam();
                        break;
                    }

                    if (_selEntity == null && !_isCtrlPressed)
                    {
                        _isBoxSelecting = true;
                        _roiStart = e.Location;
                        _selectionBox = new Rectangle();
                    }

                    Invalidate();
                }
            }
            // 마우스 오른쪽 버튼이 눌렸을 때 클릭 위치 저장
            else if (e.Button == MouseButtons.Right)
            {
                // UserControl이 포커스를 받아야 마우스 휠이 정상적으로 동작함
                Focus();
            }
        }

        private void ImageViewCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePos = e.Location;

            //마우스 이동시, 구현 코드
            if (e.Button == MouseButtons.Left)
            {
                //최초 ROI 생성하여 그리기
                if (_isSelectingRoi)
                {
                    int x = Math.Min(_roiStart.X, e.X);
                    int y = Math.Min(_roiStart.Y, e.Y);
                    int width = Math.Abs(e.X - _roiStart.X);
                    int height = Math.Abs(e.Y - _roiStart.Y);
                    _roiRect = ScreenToVirtual(new Rectangle(x, y, width, height));
                    Invalidate();
                }
                //기존 ROI 크기 변경
                else if (_isResizingRoi)
                {
                    ResizeROI(e.Location);
                    if (_selEntity != null)
                        _selEntity.EntityROI = _roiRect;
                    _resizeStart = e.Location;
                    Invalidate();
                }
                //ROI 위치 이동
                else if (_isMovingRoi)
                {
                    // 1) 화면 좌표 기준 전체 이동량
                    int dxScreen = e.X - _moveStart.X;
                    int dyScreen = e.Y - _moveStart.Y;

                    // 2) Virtual 좌표로 변환 (실수 오차 누적 줄이기)
                    float dxVirtual = dxScreen / _curZoom;
                    float dyVirtual = dyScreen / _curZoom;

                    // (A) 그룹 이동
                    if (_isGroupMove && _multiSelectedEntities.Count > 1)
                    {
                        foreach (var entity in _multiSelectedEntities)
                        {
                            if (entity == null || entity.IsHold)
                                continue;

                            if (!_moveStartRects.TryGetValue(entity, out var baseRect))
                                continue;

                            Rectangle moved = new Rectangle(
                                (int)(baseRect.X + dxVirtual),
                                (int)(baseRect.Y + dyVirtual),
                                baseRect.Width,
                                baseRect.Height);

                            entity.EntityROI = moved;
                        }
                    }
                    // (B) 단일 이동
                    else if (_selEntity != null && !_selEntity.IsHold)
                    {
                        Rectangle moved = new Rectangle(
                            (int)(_moveStartRoiRect.X + dxVirtual),
                            (int)(_moveStartRoiRect.Y + dyVirtual),
                            _moveStartRoiRect.Width,
                            _moveStartRoiRect.Height);

                        _roiRect = moved;
                        _selEntity.EntityROI = moved;
                    }

                    Invalidate();
                }
                //ROI 선택 박스 그리기
                else if (_isBoxSelecting)
                {
                    int x = Math.Min(_roiStart.X, e.X);
                    int y = Math.Min(_roiStart.Y, e.Y);
                    int w = Math.Abs(e.X - _roiStart.X);
                    int h = Math.Abs(e.Y - _roiStart.Y);
                    _selectionBox = new Rectangle(x, y, w, h);
                    Invalidate();

                }
            }
            //마우스 클릭없이, 위치만 이동시에, 커서의 위치가 크기변경또는 이동 위치일때, 커서 변경
            else
            {
                if (_selEntity != null && _newRoiType == InspWindowType.None)
                {
                    Rectangle screenRoi = VirtualToScreen(_roiRect);
                    Rectangle screenRect = VirtualToScreen(_selEntity.EntityROI);
                    int index = GetResizeHandleIndex(screenRect, e.Location);
                    if (index != -1)
                    {
                        Cursor = GetCursorForHandle(index);
                    }
                    else if (screenRoi.Contains(e.Location))
                    {
                        Cursor = Cursors.SizeAll; // ROI 내부 이동
                    }
                    else
                    {
                        Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        private void ImageViewCtrl_MouseUp(object sender, MouseEventArgs e)

        {
            //ROI 크기 변경 또는 이동 완료            
            if (e.Button == MouseButtons.Left)
            {
                if (_isSelectingRoi)
                {
                    _isSelectingRoi = false;

                    if (_bitmapImage is null)
                        return;

                    if (_roiStart == e.Location)
                        return;

                    //ROI 크기가 10보다 작으면, 추가하지 않음
                    if (_roiRect.Width < 10 ||
                        _roiRect.Height < 10 ||
                        _roiRect.X < 0 ||
                        _roiRect.Y < 0 ||
                        _roiRect.Right > _bitmapImage.Width ||
                        _roiRect.Bottom > _bitmapImage.Height)
                        return;

                    _selEntity = new DiagramEntity(_roiRect, _selColor);

                    //모델에 InspWindow 추가하는 이벤트 발생
                    DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Add, null, _newRoiType, _roiRect, new Point()));


                }
                else if (_isResizingRoi)
                {
                    _selEntity.EntityROI = _roiRect;
                    _isResizingRoi = false;

                    //모델에 InspWindow 크기 변경 이벤트 발생
                    DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Resize, _selEntity.LinkedWindow, _newRoiType, _roiRect, new Point()));
                }
                else if (_isMovingRoi)
                {
                    _isMovingRoi = false;
                    _isGroupMove = false; 

                    if (_selEntity != null)
                    {
                        InspWindow linkedWindow = _selEntity.LinkedWindow;

                        Point offsetMove = new Point(0, 0);
                        if (linkedWindow != null)
                        {
                            offsetMove.X = _selEntity.EntityROI.X - linkedWindow.WindowArea.X;
                            offsetMove.Y = _selEntity.EntityROI.Y - linkedWindow.WindowArea.Y;
                        }

                        //모델에 InspWindow 이동 이벤트 발생
                        if (offsetMove.X != 0 || offsetMove.Y != 0)
                            DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Move, linkedWindow, _newRoiType, _roiRect, offsetMove));
                        else
                            //모델에 InspWindow 선택 변경 이벤트 발생
                            DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Select, _selEntity.LinkedWindow));

                    }
                }
                // ROI 선택 완료
                if (_isBoxSelecting)
                {
                    _isBoxSelecting = false;
                    _multiSelectedEntities.Clear();

                    Rectangle selectionVirtual = ScreenToVirtual(_selectionBox);

                    foreach (DiagramEntity entity in _diagramEntityList)
                    {
                        if (selectionVirtual.IntersectsWith(entity.EntityROI))
                        {
                            _multiSelectedEntities.Add(entity);
                        }
                    }

                    if (_multiSelectedEntities.Any())
                        _selEntity = _multiSelectedEntities[0];

                    _selectionBox = Rectangle.Empty;

                    //선택해제
                    DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Select, null));

                    Invalidate();

                    return;
                }
            }

            // 마우스를 떼면 마지막 오프셋 값을 저장하여 이후 이동을 연속적으로 처리
            if (e.Button == MouseButtons.Right)
            {
                if (_newRoiType != InspWindowType.None)
                {
                    //같은 타입의 ROI추가가 더이상 없다면 초기화하여, ROI가 추가되지 않도록 함
                    _newRoiType = InspWindowType.None;
                }
                else if (_selEntity != null)
                {
                    //팝업메뉴 표시
                    _contextMenu.Show(this, e.Location);
                }

                Cursor = Cursors.Arrow;
            }
        }

        private void AddSelectedROI(DiagramEntity entity)
        {
            if (entity is null)
                return;
            if (!_multiSelectedEntities.Contains(entity))
                _multiSelectedEntities.Add(entity);
        }

        #region ROI Handle
        //마우스 위치가 ROI 크기 변경을 위한 여부를 확인하기 위해, 4개 모서리와 사각형 라인의 중간 위치 반환
        private Point[] GetResizeHandles(Rectangle rect)
        {
            return new Point[]
            {
                new Point(rect.Left, rect.Top), // 좌상
                new Point(rect.Right, rect.Top), // 우상
                new Point(rect.Left, rect.Bottom), // 좌하
                new Point(rect.Right, rect.Bottom), // 우하
                new Point(rect.Left + rect.Width / 2, rect.Top), // 상 중간
                new Point(rect.Left + rect.Width / 2, rect.Bottom), // 하 중간
                new Point(rect.Left, rect.Top + rect.Height / 2), // 좌 중간
                new Point(rect.Right, rect.Top + rect.Height / 2) // 우 중간
            };
        }

        //마우스 위치가 크기 변경 위치에 해당하는 지를, 위치 인덱스로 반환
        private int GetResizeHandleIndex(Rectangle screenRect, Point mousePos)
        {
            Point[] handles = GetResizeHandles(screenRect);
            for (int i = 0; i < handles.Length; i++)
            {
                Rectangle handleRect = new Rectangle(handles[i].X - _ResizeHandleSize / 2, handles[i].Y - _ResizeHandleSize / 2, _ResizeHandleSize, _ResizeHandleSize);
                if (handleRect.Contains(mousePos)) return i;
            }
            return -1;
        }

        //사각 모서리와 중간 지점을 인덱스로 설정하여, 해당 위치에 따른 커서 타입 반환
        private Cursor GetCursorForHandle(int handleIndex)
        {
            switch (handleIndex)
            {
                case 0: case 3: return Cursors.SizeNWSE;
                case 1: case 2: return Cursors.SizeNESW;
                case 4: case 5: return Cursors.SizeNS;
                case 6: case 7: return Cursors.SizeWE;
                default: return Cursors.Default;
            }
        }
        #endregion

        //ROI 크기 변경시, 마우스 위치를 입력받아, ROI 크기 변경
        private void ResizeROI(Point mousePos)
        {
            Rectangle roi = VirtualToScreen(_roiRect);
            switch (_resizeDirection)
            {
                case 0:
                    roi.X = mousePos.X;
                    roi.Y = mousePos.Y;
                    roi.Width -= (mousePos.X - _resizeStart.X);
                    roi.Height -= (mousePos.Y - _resizeStart.Y);
                    break;
                case 1:
                    roi.Width = mousePos.X - roi.X;
                    roi.Y = mousePos.Y;
                    roi.Height -= (mousePos.Y - _resizeStart.Y);
                    break;
                case 2:
                    roi.X = mousePos.X;
                    roi.Width -= (mousePos.X - _resizeStart.X);
                    roi.Height = mousePos.Y - roi.Y;
                    break;
                case 3:
                    roi.Width = mousePos.X - roi.X;
                    roi.Height = mousePos.Y - roi.Y;
                    break;
                case 4:
                    roi.Y = mousePos.Y;
                    roi.Height -= (mousePos.Y - _resizeStart.Y);
                    break;
                case 5:
                    roi.Height = mousePos.Y - roi.Y;
                    break;
                case 6:
                    roi.X = mousePos.X;
                    roi.Width -= (mousePos.X - _resizeStart.X);
                    break;
                case 7:
                    roi.Width = mousePos.X - roi.X;
                    break;
            }

            _roiRect = ScreenToVirtual(roi);
        }


        //#13_INSP_RESULT#9 키보드 이벤트 받기 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            _isCtrlPressed = keyData == Keys.Control;

            if (keyData == (Keys.Control | Keys.C))
            {
                CopySelectedROIs();
            }
            else if (keyData == (Keys.Control | Keys.V))
            {
                PasteROIsAt();
            }
            else
            {
                switch (keyData)
                {
                    case Keys.Delete:
                        {
                            if (_selEntity != null)
                            {
                                DeleteSelEntity();
                            }
                        }
                        break;
                    case Keys.Enter:
                        {
                            InspWindow selWindow = null;
                            if (_selEntity != null)
                                selWindow = _selEntity.LinkedWindow;

                            DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Inspect, selWindow));
                        }
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        // ─── 복사(Ctrl+C) ----------------------------------------------------------
        private void CopySelectedROIs() // #ROI COPYPASTE#
        {
            _copyBuffer.Clear();
            for (int i = 0; i < _multiSelectedEntities.Count; i++)
            {
                _copyBuffer.Add(_multiSelectedEntities[i]);
            }
        }

        // ─── 붙여넣기(Ctrl+V) ------------------------------------------------------
        private void PasteROIsAt() // #ROI COPYPASTE#
        {
            if (_copyBuffer == null || _copyBuffer.Count == 0)
                return;

            var valid = _copyBuffer
                .Where(e => e != null)
                .ToList();
            if (valid.Count == 0)
                return;

            // 2) 그룹 기준점(왼쪽 위)을 구함
            int anchorX = valid.Min(e => e.EntityROI.Left);
            int anchorY = valid.Min(e => e.EntityROI.Top);

            // ① 기준점(마우스)을 Virtual 좌표로 변환
            PointF virtBase = ScreenToVirtual(_mousePos);

            // 4) 그룹 전체를 얼마만큼 평행 이동할지 계산
            int dx = (int)(virtBase.X - anchorX + 0.5f);
            int dy = (int)(virtBase.Y - anchorY + 0.5f);

            // 5) 모든 ROI에 동일한 dx, dy 적용 → 상대 위치 유지
            foreach (var entity in valid)
            {
                var srcRect = entity.EntityROI; // 원본 위치

                DiagramEntityEvent?.Invoke(
                    this,
                    new DiagramEntityEventArgs(
                        EntityActionType.Copy,
                        entity.LinkedWindow,
                        entity.LinkedWindow?.InspWindowType ?? InspWindowType.None,
                        srcRect,
                        new Point(dx, dy)));  // 전체 그룹을 같이 이동
            }
            Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control)
                _isCtrlPressed = false;

            base.OnKeyUp(e);
        }


        //#10_INSPWINDOW#20 모델로 부터, 입력된 ROI 리스트를 설정하는 함수
        public bool SetDiagramEntityList(List<DiagramEntity> diagramEntityList)
        {
            //작은 roi가 먼저 선택되도록, 소팅
            _diagramEntityList = diagramEntityList
                                .OrderBy(r => r.EntityROI.Width * r.EntityROI.Height)
                                .ToList();

            _selEntity = null;
            Invalidate();
            return true;
        }

        public void SelectDiagramEntity(InspWindow window)
        {
            DiagramEntity entity = _diagramEntityList.Find(e => e.LinkedWindow == window);
            if (entity != null)
            {
                _multiSelectedEntities.Clear();
                AddSelectedROI(entity);

                _selEntity = entity;
                _roiRect = entity.EntityROI;
            }
        }

        //#10_INSPWINDOW#21 팝업메뉴 함수
        private void OnDeleteClicked(object sender, EventArgs e)
        {
            DeleteSelEntity();
        }

        private void OnTeachingClicked(object sender, EventArgs e)
        {
            if (_selEntity is null)
                return;

            InspWindow window = _selEntity.LinkedWindow;

            if (window is null)
                return;

            window.IsTeach = true;
            _selEntity.IsHold = true;
        }


        private void OnUnlockClicked(object sender, EventArgs e)
        {
            if (_selEntity is null)
                return;

            InspWindow window = _selEntity.LinkedWindow;

            if (window is null)
                return;

            _selEntity.IsHold = false;
        }

        private void DeleteSelEntity()
        {
            List<InspWindow> selected = _multiSelectedEntities
                .Where(d => d.LinkedWindow != null)
                .Select(d => d.LinkedWindow)
                .ToList();

            if (selected.Count > 0)
            {
                DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.DeleteList, selected));
                return;
            }

            if (_selEntity != null)
            {
                InspWindow linkedWindow = _selEntity.LinkedWindow;
                if (linkedWindow is null)
                    return;

                DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Delete, linkedWindow));
            }
        }
    }

    //#10_INSPWINDOW#22 ImageViewCtrl에서 사용하는 이벤트 타입 정의
    #region EventArgs
    public class DiagramEntityEventArgs : EventArgs
    {
        public EntityActionType ActionType { get; private set; }
        public InspWindow InspWindow { get; private set; }
        public InspWindowType WindowType { get; private set; }
        public List<InspWindow> InspWindowList { get; private set; }
        public OpenCvSharp.Rect Rect { get; private set; }
        public OpenCvSharp.Point OffsetMove { get; private set; }
        public DiagramEntityEventArgs(EntityActionType actionType, InspWindow inspWindow)
        {
            ActionType = actionType;
            InspWindow = inspWindow;
        }

        public DiagramEntityEventArgs(EntityActionType actionType, InspWindow inspWindow, InspWindowType windowType, Rectangle rect, Point offsetMove)
        {
            ActionType = actionType;
            InspWindow = inspWindow;
            WindowType = windowType;
            Rect = new OpenCvSharp.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            OffsetMove = new OpenCvSharp.Point(offsetMove.X, offsetMove.Y);
        }

        public DiagramEntityEventArgs(EntityActionType actionType, List<InspWindow> inspWindowList, InspWindowType windowType = InspWindowType.None)
        {
            ActionType = actionType;
            InspWindow = null;
            InspWindowList = inspWindowList;
            WindowType = windowType;
        }
    }

    #endregion
}

