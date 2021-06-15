using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortTest
{
    class Util
    {
        public static byte[] startCapt09A = { 0xFF, 0xC9, 0x03, 0xA3, 0xA0 };  //开始检测数据
        public static byte[] stopCapt09A = { 0xFF, 0xC9, 0x03, 0xA4, 0xA1 };  //停止检测
        public static byte[] readDeviceNum09A = { 0xFF, 0xC9, 0x03, 0xA5, 0xA2 };  //读设备号
        public static byte[] readDate09A = { 0xFF, 0xC9, 0x03, 0xA6, 0xA3 };  //读生产日期

        public static byte[] startCapt2000C = { 0xFF, 0xCA, 0x03, 0xA3, 0xA0 };  //开始检测数据
        public static byte[] stopCapt2000C = { 0xFF, 0xCA, 0x03, 0xA4, 0xA1 };  //停止检测
        public static byte[] readDeviceNum2000C = { 0xFF, 0xCA, 0x03, 0xA5, 0xA2 };  //读设备号
        public static byte[] readDate2000C = { 0xFF, 0xCA, 0x03, 0xA6, 0xA3 };  //读生产日期

        public enum device { NODevice, HKT09A, HK2000C };

        //串口通信方法

    }
}
