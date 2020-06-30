using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace NAudioDemo
{
    public class COMHelper
    {
        private static readonly SerialPort comPort = new SerialPort();

        private static string portName;

        private static bool isCOMOpen;

        public void SendToCOM(int eventTrigger)
        {
            if (eventTrigger == 69)
                return;
            var toSendString = "01 E1 01 00 ";
            toSendString += eventTrigger.ToString("X2");
            var b = HexStringToByteArray(toSendString);
            sendData(b);
        }

        public void SendLightCommand(int eventTrigger)
        {
            //var toSendString = "01 02 0A 00 02 01 01 03 01 00 10 27 01 00";
            var toSendString = "01 02 0A 00 02 01 01 03 01 00 10 27 ";
            toSendString += eventTrigger.ToString("X2");
            toSendString += " 00";
            var b = HexStringToByteArray(toSendString);
            sendData(b);
        }

        public static void SerialPortInit()
        {
            var ports = SerialPort.GetPortNames();
            if (ports.Length == 0) return;
            //portName = ports[0];
            if (isCOMOpen == false)
                try
                {
                    comPort.PortName = "COM5";
                    comPort.BaudRate = 115200;
                    comPort.Parity = Parity.None;
                    comPort.StopBits = StopBits.One;
                    comPort.DataBits = 8;
                    comPort.Handshake = Handshake.None;
                    comPort.RtsEnable = false;
                    comPort.Open();
                    isCOMOpen = true;
                }
                catch
                {
                    // ignored
                }
            else
                return;
        }

        public static void SerialPortClose()
        {
            isCOMOpen = false;
            comPort.Close();
        }

        private byte[] HexStringToByteArray(string s)
        {
            //16进制字符串转化为字节数组
            s = s.Replace(" ", "");
            var buffer = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        private bool sendData(byte[] sendBuffer)
        {
            if (isCOMOpen == false) return false;
            try
            {
                comPort.Write(sendBuffer, 0, sendBuffer.Length);
                comPort.DiscardOutBuffer();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
