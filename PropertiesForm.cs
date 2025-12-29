using JH_VisionProject.Algorithm;
using JH_VisionProject.Core;
using JH_VisionProject.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace JH_VisionProject
{
    public enum PropertyType
    {
        Binary,
        Filter,
        AIModel
    }

    public partial class PropertiesForm : DockContent
    {
        Dictionary<string, TabPage> _allTabs = new Dictionary<string, TabPage>();

        public PropertiesForm()
        {
            InitializeComponent();

            LoadOptionControl(PropertyType.Filter);
            LoadOptionControl(PropertyType.Binary);
            LoadOptionControl(PropertyType.AIModel);

        }

        private void LoadOptionControl(PropertyType propType)
        {
            string tabName = propType.ToString();
    
            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Text == tabName)
                    return;
            }
            if (_allTabs.TryGetValue(tabName, out TabPage page))
            {
                tabPropControl.TabPages.Add(page);
                return;
            }
            UserControl _propType = CreateUserControl((PropertyType)propType);
            if (_propType == null)
                return;

            // 새 탭 추가
            TabPage newTab = new TabPage(tabName)
            {
                Dock = DockStyle.Fill
            };
            _propType.Dock = DockStyle.Fill;
            newTab.Controls.Add(_propType);
            tabPropControl.TabPages.Add(newTab);
            tabPropControl.SelectedTab = newTab; // 새 탭 선택

            _allTabs[tabName] = newTab;
        }

        private UserControl CreateUserControl(PropertyType propType)
        {
            UserControl curProp = null;
            switch (propType)
            {
                case PropertyType.Binary:
                    BinaryProp blobProp = new BinaryProp();

                    //#7_BINARY_PREVIEW#8 이진화 속성 변경시 발생하는 이벤트 추가
                    blobProp.RangeChanged += RangeSlider_RangeChanged;
                    blobProp.PropertyChanged += PropertyChanged;
                    curProp = blobProp;
                    break;
                case PropertyType.Filter:
                    ImageFilterProp filterProp = new ImageFilterProp();
                    curProp = filterProp;
                    break;
                case PropertyType.AIModel:
                    AImodelProp aiModelProp = new AImodelProp();
                    curProp = aiModelProp;
                    break;
                default:
                    MessageBox.Show("유효하지 않은 옵션입니다.");
                    return null;
            }
            return curProp;
        }
        public void UpdateProperty(BlobAlgorithm blobAlgorithm)
        {
            if (blobAlgorithm is null)
                return;

            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Controls.Count > 0)
                {
                    UserControl uc = tabPage.Controls[0] as UserControl;

                    if (uc is BinaryProp binaryProp)
                    {
                        binaryProp.SetAlgorithm(blobAlgorithm);
                    }
                }
            }
        }
        //#7_BINARY_PREVIEW#7 이진화 속성 변경시 발생하는 이벤트 구현
        private void RangeSlider_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            // 속성값을 이용하여 이진화 임계값 설정
            int lowerValue = e.LowerValue;
            int upperValue = e.UpperValue;
            bool invert = e.Invert;
            ShowBinaryMode showBinMode = e.ShowBinMode;
            Global.Inst.InspStage.PreView?.SetBinary(lowerValue, upperValue, invert, showBinMode);
        }

        private void PropertyChanged(object sender, EventArgs e)
        {
            Global.Inst.InspStage.RedrawMainView();
        }
        private void PropertiesForm_Load(object sender, EventArgs e)
        {

        }

        private void tabPropControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
