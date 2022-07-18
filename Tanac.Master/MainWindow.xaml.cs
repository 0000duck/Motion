using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tanac.Log4Net;
using Tanac.Master.ViewModel;

namespace Tanac.Master
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MainViewModel.Instance;
            Log.InitializeDataGrid(txtbox_showLog);
            Log.Info("软件启动成功！");//测试
        }
    }
}
