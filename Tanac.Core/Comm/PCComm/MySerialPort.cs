using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Tanac.Comm.Tool;
using Tanac.Log4Net;

namespace PCComm
{
	public delegate string SerialPortDataReceivedFunction(SerialPort serialPort);

	public delegate void ReceiveString(string str);
	internal class MySerialPort
	{
		public enum MessageType
		{
			Incoming,
			Outgoing,
			Normal,
			Warning,
			Error
		}

		private string _baudRate = string.Empty;

		private string _parity = string.Empty;

		private string _stopBits = string.Empty;

		private string _dataBits = string.Empty;

		private string _portName = string.Empty;

		private Color[] MessageColor = new Color[5]
		{
		Color.Blue,
		Color.Green,
		Color.Black,
		Color.Orange,
		Color.Red
		};

		private SerialPort comPort = new SerialPort();

		public byte EndSymbol { get; set; }

		public bool IsReceivedByHex { get; set; } = false;


		public SerialPortDataReceivedFunction DataReceivedFunction { get; set; } = null;


		public bool isPortOpen => comPort.IsOpen;

		public string BaudRate
		{
			get
			{
				return _baudRate;
			}
			set
			{
				_baudRate = value;
			}
		}

		public string Parity
		{
			get
			{
				return _parity;
			}
			set
			{
				_parity = value;
			}
		}

		public string StopBits
		{
			get
			{
				return _stopBits;
			}
			set
			{
				_stopBits = value;
			}
		}

		public string DataBits
		{
			get
			{
				return _dataBits;
			}
			set
			{
				_dataBits = value;
			}
		}

		public string PortName
		{
			get
			{
				return _portName;
			}
			set
			{
				_portName = value;
			}
		}

		public event ReceiveString OnReceiveString;

		public MySerialPort()
		{
			_baudRate = string.Empty;
			_parity = string.Empty;
			_stopBits = string.Empty;
			_dataBits = string.Empty;
			_portName = "COM1";
			comPort.Encoding = Encoding.Default;
			comPort.DataReceived += comPort_DataReceived;
		}

		public bool WriteData(string msg, bool isSendByHex)
		{
			try
			{
				if (!comPort.IsOpen)
				{
					DisplayData(MessageType.Error, "[" + PortName + "] 串口未打开!");
					return false;
				}
				if (isSendByHex)
				{
					byte[] array = HexTool.HexToByte(msg);
					comPort.Write(array, 0, array.Length);
				}
				else
				{
					comPort.Write(msg);
				}
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}

		[STAThread]
		private void DisplayData(MessageType type, string msg)
		{
			switch (type)
			{
				case MessageType.Incoming:
					break;
				case MessageType.Outgoing:
					break;
				case MessageType.Normal:
					break;
				case MessageType.Warning:
					Log.Warn(msg);
					break;
				case MessageType.Error:
					Log.Error(msg);
					break;
			}
		}

		public bool OpenPort()
		{
			try
			{
				ClosePort();
				comPort.BaudRate = int.Parse(_baudRate);
				comPort.DataBits = int.Parse(_dataBits);
				comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);
				comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);
				comPort.PortName = _portName;
				comPort.Open();
				DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now.ToString() + "\n");
				return true;
			}
			catch (Exception ex)
			{
				DisplayData(MessageType.Error, ex.Message + "\n");
				return false;
			}
		}

		public void ClosePort()
		{
			if (comPort.IsOpen)
			{
				comPort.Close();
			}
		}

		private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			string text = "";
			try
			{
				if (DataReceivedFunction == null)
				{
					if (EndSymbol == 0)
					{
						Thread.Sleep(50);
						if (IsReceivedByHex)
						{
							int bytesToRead = comPort.BytesToRead;
							if (bytesToRead > 0)
							{
								byte[] array = new byte[bytesToRead];
								text = "";
								comPort.Read(array, 0, bytesToRead);
								byte[] array2 = array;
								foreach (byte b in array2)
								{
									text += $"{b:X2} ";
								}
							}
						}
						else if (comPort.IsOpen)
						{
							text = comPort.ReadExisting().Trim();
						}
					}
					else
					{
						byte[] array3 = new byte[10240];
						int num = 0;
						while (true)
						{
							try
							{
								byte b2 = (byte)comPort.ReadByte();
								array3[num++] = b2;
								if (b2 == EndSymbol)
								{
									break;
								}
							}
							catch (TimeoutException ex)
							{
								Log.Error(ex.ToString());
								break;
							}
						}
						text = Encoding.ASCII.GetString(array3);
					}
				}
				else
				{
					text = DataReceivedFunction(comPort);
				}
				if (text.Length > 0)
				{
					DisplayData(MessageType.Incoming, text + "\n");
				}
				this.OnReceiveString?.Invoke(text.Trim());
			}
			catch (Exception ex2)
			{
				Log.Error(ex2.ToString());
			}
		}
	}
}
