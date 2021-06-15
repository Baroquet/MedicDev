using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace PortTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private bool workState;  //当前设备是否处于监测状态
        private string portName;  //用来记录连接使用的串口号
        private Util.device curD;  //当前选中的设备标识（枚举类型）

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Content_Closing;
            serialPort = null;
            workState = false;
            portName = "";
            curD = Util.device.NODevice;
        }

        //关闭窗口前调用
        private void Content_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("关闭程序？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if(null != serialPort && serialPort.IsOpen)
                {
                    if(workState)
                    {
                        //发送命令停止检测
                        SendCommand(Util.stopCapt09A);
                        //workState = false;
                    }
                    serialPort.Close();
                }
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        //串口对象初始化
        public bool InitCOM(string PortName)
        {
            //重新扫描初始化前关闭串口
            if (null != serialPort && serialPort.IsOpen)
            {
                if (workState)
                {
                    //发送命令停止检测
                    switch (curD)
                    {
                        case Util.device.HKT09A:
                            SendCommand(Util.stopCapt09A);
                            break;
                        case Util.device.HK2000C:
                            SendCommand(Util.stopCapt2000C);
                            break;
                    }
                    serialPort.DataReceived -= SerialPort_DataReceived;  //不写这句好像会界面卡死
                    workState = false;
                }
                serialPort.Close();
            }
            serialPort = new SerialPort(PortName, 115200, Parity.None, 8, StopBits.One);
            serialPort.WriteTimeout = 200;
            
            return OpenPort();
        }

        //打开串口的方法
        public bool OpenPort()
        {
            try
            {
                serialPort.Open();
            }
            catch { }
            if(serialPort.IsOpen)
            {
                return true;
            }
            else
            {
                //MessageBox.Show("串口打开失败！");
                return false;
            }
        }

        //数据接收事件
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(700);
            byte[] readBuffer = new byte[serialPort.BytesToRead];
            
            try
            {
                Console.WriteLine(serialPort.BytesToRead);
                serialPort.Read(readBuffer, 0, serialPort.BytesToRead);
                if (curD == Util.device.HKT09A)
                {
                    //转换为十进制数据-----温度数据
                    double temperature = 0.1 * (readBuffer[5] * 128 + readBuffer[6]);
                    Dispatcher.Invoke(() => { curDataLabel.Content = temperature + "℃"; });
                }
                else if (curD == Util.device.HK2000C)
                {
                    //转换为十进制数据-----脉搏波数据
                    double puls = 0.1 * (readBuffer[5] * 128 + readBuffer[6]);
                    Dispatcher.Invoke(() => { curDataLabel.Content = puls + "Hz"; });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        
        //向串口发送数据
        public void SendCommand(byte[] command)
        {
            //byte[] writeBuffer = Encoding.ASCII.GetBytes(command);
            serialPort.Write(command, 0, command.Length);
            
        }

        //按钮状态调整
        private void changeState1()
        {
            stopCaptBtn.IsEnabled = false;
            startCaptBtn.IsEnabled = true;
        }
        private void changeState2()
        {
            stopCaptBtn.IsEnabled = true;
            startCaptBtn.IsEnabled = false;
        }

        //扫描设备
        private void scanDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames(); //扫描当前哪些端口被占用
            deviceList.Items.Clear();
            foreach(string port in portNames)
            {
                int hasDetected = 0;
                if(InitCOM(port))
                {
                    try
                    {
                        //逐一监测各医疗设备是否连接，是则加入到选择列表中
                        //SendCommand(Util.readDeviceNum09A);
                        Task.Run(() => SendCommand(Util.readDeviceNum09A));  //在子线程里执行可能耗时的操作
                        Thread.Sleep(100);  //休眠使得数据来得及接收，否则可能扫描不到到设备
                        if (serialPort.BytesToRead != 0)
                        {
                            byte[] buffer = new byte[serialPort.BytesToRead];
                            serialPort.Read(buffer, 0, serialPort.BytesToRead);
                            ListBoxItem item = new ListBoxItem();
                            //item.Uid = 
                            item.Content = Util.device.HKT09A;
                            deviceList.Items.Add(item);
                            hasDetected++;
                        }
                        //SendCommand(Util.readDeviceNum2000C);
                        Task.Run(() => SendCommand(Util.readDeviceNum2000C));
                        Thread.Sleep(100);
                        if (serialPort.BytesToRead != 0)
                        {
                            byte[] buffer = new byte[serialPort.BytesToRead];
                            serialPort.Read(buffer, 0, serialPort.BytesToRead);
                            ListBoxItem item = new ListBoxItem();
                            item.Content = Util.device.HK2000C;
                            deviceList.Items.Add(item);
                            hasDetected++;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if(hasDetected > 0)  //考虑到多个设备能通过节点排线级联，所以一旦检测到至少一个设备连接，就退出检测，并且记录该串口号
                {
                    this.portName = port;
                    break;
                }
            }
            changeState1();
            if(deviceList.Items.Count > 0)
            {
                deviceList.SelectedIndex = 0;
            }
        }

        //选取不同设备
        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(deviceList.Items.Count > 0)
            {
                ListBoxItem item = (ListBoxItem)deviceList.SelectedItem;
                curD = (Util.device)item.Content;  //将当前设备标识符‘curD’赋值为设备名
            }
        }

        //开启监测
        private void startCaptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (curD)
                {
                    case Util.device.NODevice:
                        break;
                    case Util.device.HKT09A:
                        serialPort.DataReceived += SerialPort_DataReceived;
                        SendCommand(Util.startCapt09A); workState = true;
                        changeState2();
                        break;
                    case Util.device.HK2000C:
                        serialPort.DataReceived += SerialPort_DataReceived;
                        SendCommand(Util.startCapt2000C); workState = true;
                        changeState2();
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //停止监测
        private void stopCaptBtn_Click(object sender, RoutedEventArgs e)
        {
            //发送命令停止检测
            try
            {
                switch (curD)
                {
                    case Util.device.HKT09A:
                        SendCommand(Util.stopCapt09A);
                        break;
                    case Util.device.HK2000C:
                        SendCommand(Util.stopCapt2000C);
                        break;
                }
                serialPort.DataReceived -= SerialPort_DataReceived;  //不写这句好像会界面卡死
                workState = false;
                changeState1();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
