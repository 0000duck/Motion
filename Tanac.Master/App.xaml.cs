using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Tanac.Log4Net;
using Tanac.Master.View;
using Tanac.Master.ViewModel;

namespace Tanac.Master
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private ManualResetEvent ResetSplashCreated;
        private Thread SplashThread;
        private static StartWindow Splash;
        StartWindowModel startWindowModel = new StartWindowModel();

        public App()
        {
            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
            }
            catch (Exception)
            {
            }
            base.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        public void CloseSplash()
        {
            if (Splash != null)
            {
                Splash.LoadComplete();
            }
        }
        private void ShowSplash()
        {
            ResetSplashCreated = new ManualResetEvent(false);
            SplashThread = new Thread(() =>
            {
                startWindowModel = new StartWindowModel();
                StartWindow welcomeDialog = new StartWindow();
                welcomeDialog.DataContext = startWindowModel;
                welcomeDialog.Show();
                ResetSplashCreated.Set();
                Splash = welcomeDialog;


                System.Windows.Threading.Dispatcher.Run();
            });
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Name = "Splash Screen";
            SplashThread.Start();
            ResetSplashCreated.WaitOne();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.RegisterLog();
            string pName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(pName).Length > 1)
            {
                MessageBox.Show("软件已经运行!!!");
                Environment.Exit(0);
            }
            Current.Startup += Current_Startup;
            ShowSplash();
            bool flag = startWindowModel.CheckIn();
            CloseSplash();

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Show();
        }
        private void Current_Startup(object sender, StartupEventArgs e)
        {

        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (!e.Exception.ToString().Contains("The attached property"))
                {
                    Log.Error("捕获未处理异常:" + e.Exception.ToString());
                }
            }
            catch (Exception ex)
            {
                string text = "程序发生致命错误，将终止，请联系开发人员！" + ex.ToString();
                Log.Error(text);
                System.Windows.MessageBox.Show(text);
            }
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string str = "捕获线程内未处理异常：" + e.Exception.ToString();
            Log.Error(str);
            e.SetObserved();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (e.IsTerminating)
            {
                stringBuilder.Append("程序发生致命错误，将终止，请联系开发人员！\n");
            }
            stringBuilder.Append("捕获未处理异常：");
            if (e.ExceptionObject is Exception)
            {
                stringBuilder.Append(((Exception)e.ExceptionObject).ToString());
            }
            else
            {
                stringBuilder.Append(e.ExceptionObject);
            }
            Log.Error(stringBuilder.ToString());
            if (e.IsTerminating)
            {
                System.Windows.MessageBox.Show(stringBuilder.ToString());
            }
        }
    }
}
