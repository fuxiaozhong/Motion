using Motion.ViewModel;
using System.Windows.Controls;

namespace Motion.View
{
    /// <summary>
    /// AxisView.xaml 的交互逻辑
    /// </summary>
    public partial class AxisView : UserControl
    {
        public AxisView()
        {
            InitializeComponent();
            DataContext = new AxisViewModel();
        }
    }
}