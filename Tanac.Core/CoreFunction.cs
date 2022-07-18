using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tanac.Core.MachineResources;
using Tanac.Log4Net;

namespace Tanac.Core
{
    /// <summary>
    /// 通用方法函数
    /// </summary>
    public class CoreFunction
    {
        /// <summary>
        /// 延时，单位ms
        /// </summary>
        /// <param name="milliSeconds"></param>
        public static void Delay(int milliSeconds)
        {
            if (milliSeconds < 10)
            {
                Thread.Sleep(milliSeconds);
            }
            else
            {
                DateTime tStop = DateTime.Now.AddMilliseconds(milliSeconds);
                while (true)
                {
                    Thread.Sleep(10);
                    if (DateTime.Now > tStop)
                        break;
                }
            }
        }
        /// <summary>
        /// 对象的深度复制
        /// </summary>
        /// <param name="oringinal"></param>
        /// <returns></returns>
        public static object DeepClone(object oringinal)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter
                {
                    Context = new StreamingContext(StreamingContextStates.Clone)
                };
                binaryFormatter.Serialize(memoryStream, oringinal);
                memoryStream.Position = 0L;
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
        /// <summary>
        /// 截屏
        /// </summary>
        public static void CaptureScreen()
        {
            //屏幕宽
            int userWidth =System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //屏幕高
            int userHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //按照屏幕宽高创建位图
            Image img = new Bitmap(userWidth, userHeight);
            //从一个继承自Image类的对象中创建Graphics对象
            Graphics gc = Graphics.FromImage(img);
            //抓屏并拷贝到myimage里
            gc.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(userWidth, userHeight));
            //this.BackgroundImage = img;
            //保存位图
            string fileNameExt = "Capture_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";//文件名
            string path = System.Windows.Forms.Application.StartupPath + @"\Log\CaptureScreen\" + fileNameExt;
            img.Save(path);
        }
        /// <summary>
        /// 停机报警
        /// </summary>
        /// <param name="title"></param>
        /// <param name="excMsg"></param>
        /// <param name="exSolution"></param>
        /// <param name="okOperationTip"></param>
        /// <param name="cancelOperationTip"></param>
        /// <returns></returns>
        public static DialogResult AlarmPause(string title, string excMsg, string exSolution, string okOperationTip, string cancelOperationTip)
        {
            MachineStatusManager.CurrentMachineStatus = MachineStatus.Alarm;
            Log.Error(excMsg + exSolution);
            DateTime startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DialogResult dr = MessageBox.Show(excMsg + Environment.NewLine
                + "【原因(解决方案)】" + exSolution + Environment.NewLine
                + "【确定】" + okOperationTip + Environment.NewLine
                + "【取消】" + cancelOperationTip, title, MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)
            {
                excMsg += ",点击【确定】" + okOperationTip;
            }
            else
            {
                excMsg += ",点击【取消】" + cancelOperationTip;
            }
            DateTime endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString());
            int timeCount = (endTime - startTime).Seconds;
            return dr;

        }

    }
}
