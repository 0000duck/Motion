using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tanac.Core.MachineResources;
using Tanac.Master.Services;
using Tanac.Mvvm;

namespace Tanac.Master.ViewModel
{
    public class IOItem:BindableBase
    {
        string _name;
        public string IOName
        {
            get { return _name; }
            set { _name = value; }
        }
        string _type;
        public string IOType
        {
            get { return _type; }
            set { _type = value; }
        }
        bool _state;

        public IOItem(OutputSetting output)
        {
            _output=output;
            IOName = output.OutputName;
            IOType = "输出";

        }
        private OutputSetting _output;

        public void ReadState()
        {
            IOState=_output.GetStatus();
        }

        public bool IOState
        {
            get { return _state; }
            set { SetProperty(ref _state,value); }
        }
    }
    public class PointItem : BindableBase
    {
        private string _pointName;
        private string _pointType;
        private string _movePost;
        private PointPos _pos;

        public PointItem(PointPos pointPos )
        {
            _pos= pointPos;
            PointName = pointPos.Name;
            _pointType = pointPos.Type.ToString();
            MovePost=pointPos.GetPos();
        }

        public string PointName
        {
            get { return _pointName; }
            set { SetProperty(ref _pointName, value); }
        }
        public string PointType
        {
            get { return _pointType; }
            set { SetProperty(ref _pointType, value); }
        }
        public string MovePost
        {
            get { return _movePost; }
            set           
            {
                _pos.SetPos(value);
                string movepos=_pos.GetPos();
                SetProperty(ref _movePost, movepos);
            }
        }
        //MovePointPostCommand
        //GetPointPostCommand

        public void MovePointPost() 
        {
            _pos.AbsMoveL();
        }
        public void GetPointPostCommand()
        {
            _pos.TeachPos();
            MovePost=_pos.GetPos();
        }

    }
    public class AxisItem : BindableBase
    {
        private Axis _axis;
        public AxisItem(Axis axis)
        {
            _axis = axis;
        }
        /// <summary>
        /// 轴名称
        /// </summary>
        public string AxisName
        {
            get { return _axis.Name; }
            set { _axis.Name = value; }
        }
        /// <summary>
        /// 轴号
        /// </summary>
        public short AxisIndex
        {
            get { return _axis.AxisID; }
            set { _axis.AxisID = value; }
        }

        public string AxisCard
        {
            get { return _axis.CardName; }
            set { _axis.CardName = value; }
        }
        private double prfPos;
        public double PrfPos
        {
            get { return prfPos;}
            set { SetProperty(ref prfPos,value); }
        }
        private double encPos;
        public double EncPos
        {
            get { return encPos; }
            set { SetProperty(ref encPos, value); }
        }
        public double Resolution
        {
            get { return _axis.Rate; }
            set { _axis.Rate = value; }
        }

        public double MoveVel
        {
            get { return _axis.MoveVel; }
            set { _axis.MoveVel = value; }
        }


        public  double JogVel
        {
            get { return _axis.JogVel; }
            set { SetProperty(ref _axis.JogVel, value); }
        }


        public void ReadState()
        {
            double pPos = prfPos;
            double ePos= encPos;

           _axis.GetCmdPosition(ref pPos);
           _axis.GetFeedbackPosition(ref ePos);
            PrfPos = pPos;
            EncPos = ePos;
        }

        public void StopMove()
        {
            _axis.StopMove();
        }
        public void JogMove(Direction dir)
        {
            _axis.JogMoveStart(dir);
        }
        public void AbsMove(double dis)
        {
            _axis.AbsMove(dis);
        }
    }
    public class MainViewModel : BindableBase
    {
        public ObservableCollection<IOItem> IOItems { get; set; } = new ObservableCollection<IOItem>();
        public ObservableCollection<PointItem> PointItems { get; set; } = new ObservableCollection<PointItem>();
        public ObservableCollection<AxisItem> AxisItems { get; set; } = new ObservableCollection<AxisItem>();
        public PointItem SeletedPoint
        { 
            get; 
            set; 
        }

        public AxisItem SeletedAxisItem
        {
            get;
            set;
        }


        private static MainViewModel m_Instace;
        public static MainViewModel Instance
        {
            get
            {
                if (m_Instace == null)
                {
                    m_Instace = new MainViewModel();
                }
                return m_Instace;
            }
            set { m_Instace = value; }
        }

        public MainViewModel()
        {
            foreach(var item in  OutputManager.OutputSettingList)
            {
                IOItems.Add(new IOItem(item));
            }

            foreach (var item in PointPosManager.PointPosList)
            {
                PointItems.Add(new PointItem(item));
            }
            foreach (var item in AxisManager.AxisList)
            {
                AxisItems.Add(new AxisItem(item));
            }

            System.Timers .Timer timer = new System.Timers.Timer();
            timer.Interval=1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var item in IOItems)
            {
                item.ReadState();
            }
            foreach (var item in AxisItems)
            {
                item.ReadState();
            }

        }

        public RelayCommand StartCommand => new RelayCommand(() =>
        {
            Machine.Start();

        });
        public RelayCommand StopCommand => new RelayCommand(() =>
        {
            Machine.Stop();

        });
        public RelayCommand PauseCommand => new RelayCommand(() =>
        {
            Machine.Pause();

        });
        public RelayCommand IntialCommand => new RelayCommand(() =>
        {
            Machine.Intial();

        });

        public RelayCommand SaveCommand => new RelayCommand(() =>
        {
            Machine.MachineSaveConfig();

        });




        public RelayCommand MovePointPostCommand => new RelayCommand(() =>
        {
            if (SeletedPoint != null)
            {
                SeletedPoint.MovePointPost();
            }

        });
        public RelayCommand GetPointPostCommand => new RelayCommand(() =>
        {
            if (SeletedPoint != null)
            {
                SeletedPoint.GetPointPostCommand();
            }
        });

        public double TargetPos { get; set; }
        public RelayCommand<string> MoveAxisCommand => new RelayCommand<string>((s) =>
        {
            if (SeletedAxisItem != null)
            {
                switch(s)
                {
                    case "0":
                        SeletedAxisItem.StopMove();
                        break;
                    case "1":
                        SeletedAxisItem.JogMove(Direction.POSITIVE);
                        break;
                    case "2":
                        SeletedAxisItem.JogMove(Direction.NEGATIVE);
                        break;
                    case "3":
                        SeletedAxisItem.AbsMove(TargetPos);
                        break;
                }
            }
        });

        private string currentState;
        public string CurrentState
        {
            get { return currentState; }
            set { SetProperty(ref currentState, value); }
        }

    }
}
