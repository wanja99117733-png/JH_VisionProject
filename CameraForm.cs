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
        }
        public string CurrentImagePath => _currentImagePath;
        public Bitmap CurrentBitmap => _currentBitmap;

        private void CameraForm_Load(object sender, EventArgs e)
        {

        }
        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (imageViewer != null)
                imageViewer.LoadBitmap(bitmap);
           
            if (bitmap != null)
            {
                _currentBitmap?.Dispose();
                _currentBitmap = (Bitmap)bitmap.Clone();
                _currentImagePath = null; // 파일 경로는 모름
            }
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

        public Bitmap GetDisplayImage()
        {
            Bitmap curImage = null;

            if (imageViewer != null)
                curImage = imageViewer.GetCurBitmap();

            return curImage;
        }

        private void imageViewer_Load(object sender, EventArgs e)
        {

        }

        private void CameraForm_Resize(object sender, EventArgs e)
        {

        }
    }
}
