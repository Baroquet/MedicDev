using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortTest
{
    class Util
    {
        #region 传感器通信命令
        //温度传感器
        public static byte[] startCapt09A = { 0xFF, 0xC9, 0x03, 0xA3, 0xA0 };  //开始检测数据
        public static byte[] stopCapt09A = { 0xFF, 0xC9, 0x03, 0xA4, 0xA1 };  //停止检测
        public static byte[] readDeviceNum09A = { 0xFF, 0xC9, 0x03, 0xA5, 0xA2 };  //读设备号
        public static byte[] readDate09A = { 0xFF, 0xC9, 0x03, 0xA6, 0xA3 };  //读生产日期
        //public static byte[] upTem09A = { 0xFF, 0xC9, 0x04, 0x0E, 0xCA, 0x40 }; //上调温度64
        //public static byte[] downTem09A = { 0xFF, 0xC9, 0x04, 0x11, 0xCD, 0x40 }; //下调温度64

        //脉搏传感器
        public static byte[] startCapt2000C = { 0xFF, 0xCA, 0x03, 0xA3, 0xA0 };  //开始检测数据
        public static byte[] stopCapt2000C = { 0xFF, 0xCA, 0x03, 0xA4, 0xA1 };  //停止检测
        public static byte[] readDeviceNum2000C = { 0xFF, 0xCA, 0x03, 0xA5, 0xA2 };  //读设备号
        public static byte[] readDate2000C = { 0xFF, 0xCA, 0x03, 0xA6, 0xA3 };  //读生产日期
        //public static byte[] upPul2000C = { 0xFF, 0xCA, 0x04, CKSUM, 0xA4, FD };  //设置脉搏幅度级别

        //血氧传感器
        public static byte[] startCaptHKS12C = { 0xFF, 0xC7, 0x03, 0xA3, 0xA0 };  //开始检测数据
        public static byte[] stopCaptHKS12C = { 0xFF, 0xC7, 0x03, 0xA4, 0xA1 };  //停止检测
        public static byte[] readDeviceNumHKS12C = { 0xFF, 0xC7, 0x03, 0xA5, 0xA2 };  //读设备号
        public static byte[] readDateHKS12C = { 0xFF, 0xC7, 0x03, 0xA6, 0xA3 };  //读生产日期

        #endregion

        public enum device { NODevice, HKT09A, HK2000C, HKS12C };

        //串口通信方法

    }
}
