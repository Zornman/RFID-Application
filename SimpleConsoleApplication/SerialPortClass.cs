using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Intermec.DataCollection.RFID;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;

namespace SimpleConsoleApplication
{
    class SerialPortClass
    {
        public SerialPort scom = new SerialPort();
        public bool bDebug = true;

        public void CloseSerialPort()
        {
            scom.Close();
            scom = null;
        }

        public bool OpenSerialPort(string sCOMPort, string sVersion)
        {
            bool bStatus = false;

            scom.BaudRate = 115200;
            scom.DataBits = 8;
            scom.PortName = sCOMPort;
            if (bDebug == true) { System.Console.WriteLine("OpenSerialPort() -> scom.PortName = " + scom.PortName); }
            try
            {
                scom.Open();
                if (bDebug == true) { System.Console.WriteLine("OpenSerialPort() -> scom.Open()"); }
                bStatus = true;
            }
            catch (System.ArgumentException exp)
            { if (bDebug == true) { System.Console.WriteLine("COM System.ArgumentException->" + exp.ToString()); } }
            catch (System.IO.IOException exp)
            { if (bDebug == true) { System.Console.WriteLine("COM System.IO.IOException->" + exp.ToString()); } }
            catch (System.UnauthorizedAccessException exp)
            { if (bDebug == true) { System.Console.WriteLine("COM System.UnauthorizedAccessException->" + exp.ToString()); } }
            catch (System.InvalidOperationException exp)
            { if (bDebug == true) { System.Console.WriteLine("COM System.InvalidOperationException->" + exp.ToString()); } }

            if (bStatus == false) { return bStatus; }

            bStatus = SendDataSerialPort("Console Application Version: " + sVersion + "\r\nOK>");
            
            return bStatus;
        }

        public bool IsBytesToRead()
        {
            bool bStatus = false;
            if (scom.BytesToRead > 0)
            {
                if (bDebug == true) { System.Console.WriteLine("MyReadThread() -> scom.BytesToRead > 0"); }
                bStatus = true;
                return bStatus;
            }
            return bStatus;
        }

        public string GetSerialData()
        {
            string sMsg = null;

            sMsg = scom.ReadLine().ToUpper();

            if (bDebug == true) { System.Console.WriteLine("GetSerialData() -> sMsg: " + sMsg); }

            return sMsg;
        }

        public bool SendDataSerialPort(string sMsg)
        {
            //send data out serial port
            bool bStatus = false;

            try
            {
                scom.WriteLine(sMsg + "\r\n");
                if (bDebug == true) { System.Console.WriteLine("SendDataSerialPort() -> " + sMsg); }
                bStatus = true;
            }
            catch (System.TimeoutException exp)
            { if (bDebug == true) { System.Console.WriteLine("SendDataSerialPort() System.TimeoutException->" + exp.ToString()); } }
            catch (System.ArgumentNullException exp)
            { if (bDebug == true) { System.Console.WriteLine("SendDataSerialPort() System.ArgumentNullException->" + exp.ToString()); } }
            catch (System.InvalidOperationException exp)
            { if (bDebug == true) { System.Console.WriteLine("SendDataSerialPort() System.InvalidOperationException->" + exp.ToString()); } }

            return bStatus;
        }
    }//END SerialPortClass
}//END namespace SimpleConsoleApplication
