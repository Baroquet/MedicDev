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
using LiveCharts;
using LiveCharts.Wpf;

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
        private bool[] linkState;//三台设备的连接状态

        private double temDelta; //温度校准差
        
        //private int pulLev;
        //private Util.device curD;  //当前选中的设备标识（枚举类型）

        //chart表绑定的数据集
        public SeriesCollection SeriesCollection { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Content_Closing;
            
            serialPort = null;
            workState = false;
            portName = "";
            linkState = new bool[]{ false, false, false};
            changeState3();
            temDelta = 0.0;
            //pulLev = 0;
            //curD = Util.device.NODevice;
        }

        #region 串口操作函数
        //关闭窗口前调用
        private async void Content_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("关闭程序？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if(null != serialPort && serialPort.IsOpen)
                {
                    if(workState)
                    {
                        try
                        {
                            int num = 0;
                            Task<int> t = Task.Run(() => ChangeDeviceState(false));
                            num = await t;
                            if (num > 0)
                            {
                                workState = false;
                                changeState1();
                            }
                        }
                        catch { }
                        
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


        //遍历设备，根据指示改变其工作状态
        //state为true：开启监测
        //state为false：关闭监测
        //返回扫描到的设备数
        private int ChangeDeviceState(bool state)
        {
            int num = 0;
            try
            {
                //准备开启设备监测
                if(state)
                {
                    if(deviceList.Items.Count == 0)
                    {
                        return 0;
                    }
                    //由于该函数一般是在子线程中调用所以访问界面元素时需要使用Dispatcher
                    Dispatcher.Invoke(() =>
                    {
                        foreach (ListBoxItem d in deviceList.Items)
                        {
                            switch (d.Content)
                            {
                                case Util.device.HKT09A:
                                    serialPort.DataReceived += SerialPort_DataReceived;
                                    SendCommand(Util.startCapt09A);
                                    break;
                                case Util.device.HK2000C:
                                    serialPort.DataReceived += SerialPort_DataReceived1;
                                    SendCommand(Util.startCapt2000C);
                                    break;
                                case Util.device.HKS12C:
                                    serialPort.DataReceived += SerialPort_DataReceived2;
                                    SendCommand(Util.startCaptHKS12C);
                                    break;
                            }
                            num++;
                        }
                    });
                    
                }
                else  //关闭设备
                {
                    if (deviceList.Items.Count == 0)
                    {
                        return 0;
                    }
                    Dispatcher.Invoke(() => {
                        foreach (ListBoxItem d in deviceList.Items)
                        {
                            switch (d.Content)
                            {
                                case Util.device.HKT09A:
                                    serialPort.DataReceived -= SerialPort_DataReceived;
                                    SendCommand(Util.stopCapt09A);
                                    break;
                                case Util.device.HK2000C:
                                    serialPort.DataReceived -= SerialPort_DataReceived1;
                                    SendCommand(Util.stopCapt2000C);
                                    break;
                                case Util.device.HKS12C:
                                    serialPort.DataReceived -= SerialPort_DataReceived2;
                                    SendCommand(Util.stopCaptHKS12C);
                                    break;
                            }
                            num++;
                        }
                    });
                }
            }
            catch { }
            return num;
        }

        //串口对象初始化
        public bool InitCOM(string PortName)
        {
            //重新扫描初始化前关闭串口
            if (null != serialPort && serialPort.IsOpen)
            {
                if (workState)
                {
                    try
                    {
                        int num = ChangeDeviceState(false);
                        if (num > 0)
                        {
                            workState = false;
                            changeState1();
                        }
                    }
                    catch { }

                }
                serialPort.Close();
            }
            serialPort = new SerialPort(PortName, 115200, Parity.None, 8, StopBits.One);
            serialPort.WriteTimeout = 500;  //设置发送超时为500ms
            
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

        //数据接收事件（默认是在子线程被调用，所以界面不卡死？）
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(700);
            byte[] readBuffer = new byte[serialPort.BytesToRead];

            try
            {
                //Console.WriteLine(serialPort.BytesToRead);
                serialPort.Read(readBuffer, 0, serialPort.BytesToRead);
                
                //转换为十进制数据-----温度数据
                double temperature = 0.1 * (readBuffer[5] * 128 + readBuffer[6]);
                temperature += temDelta;  //与校准值求和
                Dispatcher.Invoke(() => { curDataLabel.Content = temperature + "℃"; });
                //波形图更新显示
                //RefreshChart(temperature);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void SerialPort_DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[serialPort.BytesToRead];
            try
            {
                serialPort.Read(readBuffer, 0, serialPort.BytesToRead);
                
                //转换为十进制数据-----脉搏波数据
                double puls = (readBuffer[5] * 128 + readBuffer[6]);
                Dispatcher.Invoke(() => { curDataLabel1.Content = puls + "mmHg"; });
                //RefreshChart(puls);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void SerialPort_DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[serialPort.BytesToRead];
            try
            {
                serialPort.Read(readBuffer, 0, serialPort.BytesToRead);
                
                //转换为十进制数据-----血氧数据
                double scale = readBuffer[5];  //血氧容积波形幅值
                double hue = readBuffer[6];  //血氧饱和度（%）
                double rate = readBuffer[7]; //心率（次/分钟）
                Dispatcher.Invoke(() => { curDataLabel2.Content = rate + "次/分钟"; curDataLabel3.Content = hue + "%"; curDataLabel4.Content = scale; });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //向串口发送数据
        public void SendCommand(byte[] command)
        {
            //byte[] writeBuffer = Encoding.ASCII.GetBytes(command);
            try
            {
                serialPort.Write(command, 0, command.Length);
                Thread.Sleep(100);  //发送成功后在该函数所在线程中延时100ms回到主线程再执行，使数据来得及接收，并且界面没有延迟
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        //按钮状态调整
        private void changeState1()  //刚关闭监测
        {
            stopCaptBtn.IsEnabled = false;
            startCaptBtn.IsEnabled = true;
            upTemBtn.IsEnabled = false;
            downTemBtn.IsEnabled = false;
            pulLevList.IsEnabled = false;
        }
        private void changeState2()  //开始监测之后
        {
            stopCaptBtn.IsEnabled = true;
            startCaptBtn.IsEnabled = false;
            upTemBtn.IsEnabled = linkState[0];
            downTemBtn.IsEnabled = linkState[0];
            pulLevList.IsEnabled = linkState[1];
        }
        private void changeState3()  //初始状态|未扫描到任何设备
        {
            stopCaptBtn.IsEnabled = false;
            startCaptBtn.IsEnabled = false;
            upTemBtn.IsEnabled = false;
            downTemBtn.IsEnabled = false;
            pulLevList.IsEnabled = false;
        }

        #region 组件响应事件

        //扫描设备
        private async void scanDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames(); //扫描当前哪些端口被占用
            deviceList.Items.Clear();
            foreach(string port in portNames)
            {
                int hasDetected = 0;
                bool r = await Task<bool>.Run(() => InitCOM(port));
                if(r)
                {
                    try
                    {
                        //逐一监测各医疗设备是否连接，是则加入到选择列表中
                        Task t1 = Task.Run(() => SendCommand(Util.readDeviceNum09A));  //在子线程里执行可能耗时的操作
                        await t1;  //异步等待子线程t1结束再继续执行，可以防止界面失灵

                        //改为在子线程Task中等待，故注释下面一行代码
                        //Thread.Sleep(100);  //休眠使得数据来得及接收，否则可能扫描不到到设备
                        if (serialPort.BytesToRead != 0)
                        {
                            byte[] buffer = new byte[serialPort.BytesToRead];
                            serialPort.Read(buffer, 0, serialPort.BytesToRead);
                            ListBoxItem item = new ListBoxItem();
                            item.Content = Util.device.HKT09A;
                            deviceList.Items.Add(item);
                            linkState[0] = true;
                            hasDetected++;
                        }
                        Task t2 = Task.Run(() => SendCommand(Util.readDeviceNum2000C));
                        await t2;
                        //Thread.Sleep(100);
                        if (serialPort.BytesToRead != 0)
                        {
                            byte[] buffer = new byte[serialPort.BytesToRead];
                            serialPort.Read(buffer, 0, serialPort.BytesToRead);
                            ListBoxItem item = new ListBoxItem();
                            item.Content = Util.device.HK2000C;
                            deviceList.Items.Add(item);
                            linkState[1] = true;
                            hasDetected++;
                        }
                        Task t3 = Task.Run(() => SendCommand(Util.readDeviceNumHKS12C));
                        await t3;
                        //Thread.Sleep(100);
                        if (serialPort.BytesToRead != 0)
                        {
                            byte[] buffer = new byte[serialPort.BytesToRead];
                            serialPort.Read(buffer, 0, serialPort.BytesToRead);
                            ListBoxItem item = new ListBoxItem();
                            item.Content = Util.device.HKS12C;
                            deviceList.Items.Add(item);
                            linkState[2] = true;
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
            if (deviceList.Items.Count > 0)
            {
                changeState1();
                deviceList.SelectedIndex = 0;
            }
            else  //未扫描到
            {
                linkState = new bool[]{false, false, false};
                changeState3();
            }
        }

        //开启监测
        private async void startCaptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int num = 0;
                Task<int> t = Task.Run(() => ChangeDeviceState(true));
                num = await t;
                if (num > 0)
                {
                    workState = true;
                    changeState2();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //停止监测
        private async void stopCaptBtn_Click(object sender, RoutedEventArgs e)
        {
            //发送命令停止检测
            try
            {
                int num = 0;
                Task<int> t = Task.Run(() => ChangeDeviceState(false));
                num = await t;
                if (num > 0)
                {
                    workState = false;
                    changeState1();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        //上调温度偏差
        private void upTemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Task t = Task.Run(() => SendCommand(Util.upTem09A));
                //await t;
                
                temDelta += 0.1;
                temDeltaLabel.Content = temDelta >= 0.0 ? ("+" + temDelta + "℃") : (temDelta + "℃");
            }
            catch { }
        }

        //下调温度偏差
        private void downTemBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Task t = Task.Run(() => SendCommand(Util.downTem09A));
                //await t;
                
                temDelta -= 0.1;
                temDeltaLabel.Content = temDelta >= 0.0 ? ("+" + temDelta + "℃") : (temDelta + "℃");
            }
            catch { }
        }

        private async void pulLevList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem item = (ComboBoxItem)pulLevList.SelectedItem;
                //设置脉搏幅度级别（0~10）
                int level = int.Parse(item.Content.ToString());
                //事先判断出校验位不会溢出一个字节，所以命令字节数组即时生成
                byte[] command = { 0xFF, 0xCA, 0x04, (byte)(0xA8 + level), 0xA4, (byte)level };
                await Task.Run(() => SendCommand(command));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
