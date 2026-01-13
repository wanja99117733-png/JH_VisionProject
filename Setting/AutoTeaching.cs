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


namespace JH_VisionProject
{
    public partial class AutoTeaching : Form
    {
        public AutoTeaching()
        {
            InitializeComponent();
        }

        // MainForm 등에서 호출할 초기화 메서드
        public void InitForTeach(MatchAlgorithm matchAlgo, Mat src, Rect roi)
        {
            autoFindROI1.SetAlgorithm(matchAlgo);
            autoFindROI1.SetSourceImage(src);
            autoFindROI1.SetRoi(roi);
        }
    }
}
