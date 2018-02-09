using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace SimpleConsoleApplication
{
    class GPS
    {
        public static string Latitude = "0";
        public static string Longitude = "0";
        public static string date = "01-01-2001";
        public static string time = "12:00:00 AM";
        public int gpstimer = 5000;

        //for IF61
        public System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();

        //for IF2
        FileStream myfsIF2ComPort = null;
        StreamReader mysrIF2ComPort = null;

        public static bool bdebug = false;

        private string[] GetIF2SerialPortData()
        {
            //if2 reader

            string data = null;
            int ii = 0;

            try
            {
                mysrIF2ComPort.DiscardBufferedData();
            }
            catch (Exception e)
            {
                Console.WriteLine("---> Exception mysrIF2ComPort.DiscardBufferedData");
            }

            string s = null;
            while (ii != 5)
            {
                ii++;
                s = mysrIF2ComPort.ReadLine();
                data += s;
            }

            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine("---> GPS Data: NULL");
                return null;
            }

            if (settings.enable_gps_debug == "1")
                Console.WriteLine("---> GPS Data: " + data);
            
            string[] strArr = data.Split('$');
            return strArr;
        }

        private bool ConnectToGPSPort()
        {
            if (settings.hwid.StartsWith("IF61") || settings.enable_pcmode == "1")
            {
                try
                {
                    serialPort1.Close();
                    serialPort1.PortName = "/dev/ttyS1";
                    if (settings.enable_pcmode == "1")
                        serialPort1.PortName = settings.enable_pcmode;
                    serialPort1.BaudRate = 115200;
                    serialPort1.Open();

                    if (!serialPort1.IsOpen)
                    {
                        Console.WriteLine("--> GPS IF61 COM Port Closed, not good aborting gps!!!");
                        return false;
                    }
                    else
                        return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("--> GPS IF61 Serial Port Exception:");
                    Console.WriteLine("--> " + e.Message);
                    return false;
                }
            }
            else
            {
                CloseIF2SerialPort();
                if (!ConnectToIF2SerialPort())
                {
                    Console.WriteLine("--> GPS IF2 COM Port Closed, not good aborting gps!!!");
                    return false;
                }
                else
                    return true;
            }
        }

        private string[] GetIF61SerialPortData()
        {
            //if61 reader/PC mode

            string data = null;
            int ii = 0;
            while (ii != 5)
            {
                ii++;
                data += serialPort1.ReadLine();
            }
            //data = "$GPGGA,123519,4124.8963,N,01131.000,W,1,08,0.9,545.4,M,46.9,M,,*47";
            //data = "$PGRMF,,,050509,205713,14,2503.7230,N,12138.4148,E,A,0,,,,*24";
            if (settings.enable_gps_debug == "1")
                Console.WriteLine("---> GPS Data: " + data);

            string[] strArr = data.Split('$');
            return strArr;
        }

        public void Beta()
        {
            int z = 0;

            if (!ConnectToGPSPort())
                return;

            if (settings.enable_gps_debug == "1")
                Console.WriteLine("--> GPS sleep value: " + gpstimer.ToString());

            while (true)
            {                
                Thread.Sleep(gpstimer);
                string[] strArr = null;

                z++;

                if (settings.hwid.Equals("IF2"))
                {
                    //Console.WriteLine(z.ToString() + ".  Next GPS read IF2");
                    strArr = GetIF2SerialPortData();
                }
                else
                {
                    //Console.WriteLine(z.ToString() + ".  Next GPS read IF61");
                    try
                    {
                        if (serialPort1.IsOpen)
                        {
                            strArr = null;
                            strArr = GetIF61SerialPortData();
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("EX GPS serial port not open: " + Ex.Message);
                    }
                }

                if (strArr == null)
                {
                    Console.WriteLine("NO GDS DATA! NULL RETURNED!");
                }
                else if (strArr.Length == 0)
                {
                    Console.WriteLine("NO GDS DATA! zero data RETURNED!");
                }
                else
                {
                    if (settings.gps == "garmin")
                    {
                        ProcessGarminData(strArr);
                    }
                    else if (settings.gps == "pinpoint")
                    {
                        ProcessPinPointData(strArr);
                    }
                }
            }
        }

        public static void ProcessPinPointData(string[] strArr)
        {
            //Data: $GPGGA,202458.2,,,,,0,,,,,,,,*73
            //$GPRMC,,V,,,,,,,,,,N*53
            //$GPVTG,,T,,M,,N,,K,N*2C
            //$GPGGA,202459.2,,,,,0,,,,,,,,*72
            //$GPRMC,,V,,,,,,,,,,N*53

            //Console.WriteLine("---> processing pinpoint gps data");

            try
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strTemp = strArr[i].Replace("$", "");
                    string[] lineArr = strTemp.Split(',');
                    if (lineArr[0] == "GPGGA")
                    {
                        string lati = lineArr[2];
                        string[] lat = lati.ToString().Split('.');
                        int lengthLatPre = lat[0].Length;
                        int lengthLatSuf = lat[1].Length;
                        double preLat = Convert.ToDouble(lati.Substring(0, 2).ToString());
                        double sufLat = Convert.ToDouble(lati.Substring(2, lati.Length - 2).ToString());

                        //Latitude
                        Double dLat = sufLat / 60;
                        dLat = preLat + dLat;
                        if (lineArr[3].ToString() == "S")
                            dLat *= -1;
                        Latitude = dLat.ToString();

                        //Longitude
                        string loni = lineArr[4];
                        string[] lon = loni.ToString().Split('.');
                        int lengthLonPre = lon[0].Length;
                        int lengthLonSuf = lon[1].Length;
                        double preLon = 0;
                        double sufLon = 0;

                        if (lengthLonPre == 5)
                        {
                            preLon = Convert.ToDouble(loni.Substring(0, 3).ToString());
                            sufLon = Convert.ToDouble(loni.Substring(3, loni.Length - 3).ToString());
                        }
                        else if (lengthLonPre == 4)
                        {
                            preLon = Convert.ToDouble(loni.Substring(0, 2).ToString());
                            sufLon = Convert.ToDouble(loni.Substring(2, lon.Length - 2).ToString());
                        }
                        Double dLon = sufLon / 60;
                        dLon = preLon + dLon;
                        if (lineArr[5].ToString() == "W")
                            dLon *= -1;
                        Longitude = dLon.ToString();

                        //Display
                        if (settings.enable_gps_debug == "1")
                            Console.WriteLine("---> Latitude: " + Latitude + "    Longitude: " + Longitude);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Pinpoint GPS parsing error: ");
                Console.WriteLine("---> " + ex.Message);
            }
        }

        private void ProcessGarminData(string[] strArr)
        {
            Console.WriteLine("---> processing Garmin gps data");

            try
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strTemp = strArr[i].Replace("$", "");
                    string[] lineArr = strTemp.Split(',');
                    if (lineArr[0] == "PGRMF")
                    {
                        Console.WriteLine("Garmin GPS packet to be parsed: ");
                        for (int x = 0; x < lineArr.Length; x++)
                        {
                            Console.WriteLine(x.ToString() + ".  --> Data: " + lineArr[x].ToString());
                        }

                        date = lineArr[3];
                        string dd = date.Substring(0, 2);
                        string mm = date.Substring(2, 2);
                        string yy = date.Substring(4, 2);
                        date = mm + "-" + dd + "-" + yy;
                        time = lineArr[4];
                        string hh = time.Substring(0, 2);
                        string min = time.Substring(2, 2);
                        string ss = time.Substring(4, 2);
                        time = hh + ":" + min + ":" + ss;

                        string lati = lineArr[6];
                        string[] lat = lati.ToString().Split('.');
                        int lengthLatPre = lat[0].Length;
                        int lengthLatSuf = lat[1].Length;
                        double preLat = Convert.ToDouble(lati.Substring(0, 2).ToString());
                        double sufLat = Convert.ToDouble(lati.Substring(2, lati.Length - 2).ToString());
                        
                        //Latitude
                        Double dLat = sufLat / 60;
                        dLat = preLat + dLat;
                        if (lineArr[7].ToString() == "S")
                            dLat *= -1;
                        Latitude = dLat.ToString();
                        
                        //Longitude
                        string loni = lineArr[8];
                        string[] lon = loni.ToString().Split('.');
                        int lengthLonPre = lon[0].Length;
                        int lengthLonSuf = lon[1].Length;
                        double preLon = 0;
                        double sufLon = 0;

                        if (lengthLonPre == 5)
                        {
                            preLon = Convert.ToDouble(loni.Substring(0, 3).ToString());
                            sufLon = Convert.ToDouble(loni.Substring(3, loni.Length - 3).ToString());
                        }
                        else if (lengthLonPre == 4)
                        {
                            preLon = Convert.ToDouble(loni.Substring(0, 2).ToString());
                            sufLon = Convert.ToDouble(loni.Substring(2, lon.Length - 2).ToString());
                        }
                        Double dLon = sufLon / 60;
                        dLon = preLon + dLon;
                        if (lineArr[9].ToString() == "W")
                            dLon *= -1;
                        //string[] lat = dLat.ToString().Split('.');
                        Longitude = dLon.ToString();

                        //Display
                        if (settings.enable_gps_debug == "1")
                            Console.WriteLine("Latitude: " + Latitude + "    Longitude: " + Longitude);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Garmin GPS parsing error: ");
                Console.WriteLine("---> " + ex.Message);
            }
        }

        
        public bool ConnectToIF2SerialPort()
        {
            //IF2 serial port is not a real serial port.  
            //It is actually a file connection
            try
            {
                string sPort = "/dev/ttyS01"; //IF2 plus internal serial port address.
                myfsIF2ComPort = new FileStream(sPort, FileMode.Open, FileAccess.ReadWrite);
                mysrIF2ComPort = new StreamReader(myfsIF2ComPort);
                if (bdebug) System.Console.WriteLine("Serial port opened");
                try
                {
                    mysrIF2ComPort.DiscardBufferedData();
                }
                catch (Exception e)
                {
                    Console.WriteLine("---> Exception mysrIF2ComPort.DiscardBufferedData");
                }

            }
            catch (Exception ee)
            {
                System.Console.WriteLine("FAILED opening serial port bye");
                System.Console.WriteLine(ee.Message);
                return false;
            }

            return true;
        }

        public void CloseIF2SerialPort()
        {
            if (mysrIF2ComPort != null)
            {
                mysrIF2ComPort.Close();
                mysrIF2ComPort = null;
            }
            if (myfsIF2ComPort != null)
            {
                myfsIF2ComPort.Close();
                myfsIF2ComPort = null;
                if (bdebug) System.Console.WriteLine("Serial port is closed");
            }
        }
    }
}
