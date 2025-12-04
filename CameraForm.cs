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
        public CameraForm()
        {
            InitializeComponent();
        }

        private void CameraForm_Load(object sender, EventArgs e)
        {

        }
        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (imageViewer != null)
                imageViewer.LoadBitmap(bitmap);
        }
        public void LoadImage(string filename)
        {
            if (File.Exists(filename) == false)
                return;

            Image bitmap = Image.FromFile(filename);
            imageViewer.LoadBitmap((Bitmap)bitmap);
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
