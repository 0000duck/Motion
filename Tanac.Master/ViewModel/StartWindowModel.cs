using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tanac.Core.MachineResources;
using Tanac.Master.Services;

namespace Tanac.Master.ViewModel
{
    public class StartWindowModel : ViewModelBase
    {
        private string _NameText = "田中精机";
        public string NameText
        {
            get { return _NameText; }
            set { _NameText = value; RaisePropertyChanged(() => NameText); }
        }
        private string _LogText = "Tanac";
        public string LogText
        {
            get { return _LogText; }
            set { _LogText = value; RaisePropertyChanged(() => LogText); }
        }
        private string _ProcessValueText;
        public string ProcessValueText
        {
            get { return _ProcessValueText; }
            set { _ProcessValueText = value; RaisePropertyChanged(() => ProcessValueText); }
        }
        public bool CheckIn()
        {
            bool ret;
            ret = ReadSysConfig();
            ret = ReadSolutionConfig();
            return ret;
        }

        private void LoadingToolMessage(string msg)
        {
            ProcessValueText = msg;
            System.Threading.Thread.Sleep(1000);
        }
        public bool ReadSysConfig()
        {
            LoadingToolMessage("加载IO板卡插件...");
            IOCardManager.InitPlugin();
            LoadingToolMessage("加载运动控制卡插件...");
            MotionCardManager.InitPlugin();
            LoadingToolMessage("加载系统配置文件...");
            Machine.MachineLoadConfig();
            LoadingToolMessage("初始化系统...");
            Machine.MachineInit();
            return false;

        }
        public bool ReadSolutionConfig()
        {
            return true;
        }
    }
}
