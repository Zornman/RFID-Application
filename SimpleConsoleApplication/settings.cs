using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleConsoleApplication
{
    class settings
    {
        public static ArrayList tag_list;
        public static string[] emailAddressList;

        public static string hwid = "IF2";
        public static string enable_flat_file_storage = "0";
        public static string pc_com_port = "com3";
        public static string AppCurrentDir = "./";
        public static string enable_tag_list;
        public static string gps_timer;
        public static string read_pause_time_ms;
        public static string read_duration_ms;
        public static string park_antenna;
        public static string reader_ip = "169.254.1.1";
        public static string enable_trigger;
        public static string enable_gps;
        public static string server;
        public static string port;
        public static string username;
        public static string password;
        public static string enable_gps_debug;
        public static string enable_debug;
        public static string enable_pcmode;
        public static string enableLiveDataTransmit;
        public static string enable_dump;
        public static string truckid;
        public static string routeid;
        public static string transmitMode;
        public static string pingServer;
        public static string ftpServer;
        public static string ftpUsername;
        public static string ftpPassword;
        public static string ftpDir;
        public static string exDir;
        public static string blDir;
        public static string ants;
        public static string ant1;
        public static string ant2;
        public static string ant3;
        public static string ant4;
        public static string delimiter;
        public static string programName;
        public static string gps;
        public static string observations;
        public static string cc_tag;
        public static string transmit_username;
        public static string transmit_password;
        public static string sent_CorruptDBNotification = "0";
        public static string sent_NoGPSNotification = "0";
        public static string sent_DateTimeOffNotification = "0";
        public static string sent_PickupRecordFailedNotification = "0";
        public static string sent_NoDataTransmitted = "0";
        

        public static Int32 arm_timer;
        public static Int32 transmit_timer;
        public static Int32 transmit_maxCount;
        public static Int32 timerGPO2;
        public static Int32 timerGPO3;
        public static Int32 timerGPO4;
        public static Int32 dataRefreshRate;
        public static Int32 noTransmitTime = 0;
        public static Int32 transmissionFailed = 0;

        private static Int32 ConvertToInt32(string s)
        {
            Int32 i = 0;

            try
            {
                i = Convert.ToInt32(s);
                return i;
            }
            catch (Exception e)
            {
                i = 0;
                return i;
            }
        }

        public static void configure()
        {
            string s = null;

            AppCurrentDir = "./";

            //Default Values
            arm_timer = 5000;
            string path = "Config.xml";
            if (settings.enable_pcmode == "0")
            {
                //path = "/home/developer/edgeware/userapp9/./Config.xml";
            }

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            System.Xml.XmlDocument CXML = new System.Xml.XmlDocument();
            CXML.Load(fs);

            tag_list = null;
            tag_list = new ArrayList();

            //Get the number of nodes in the xml document
            for (int i = 0; i < CXML.DocumentElement.ChildNodes.Count; i++)
            {
                //Add the innertext of the xml document to the listbox
                Console.WriteLine(CXML.DocumentElement.ChildNodes[i].Name.ToString() + "--->" + CXML.DocumentElement.ChildNodes[i].InnerText);
                switch (CXML.DocumentElement.ChildNodes[i].Name.ToString())
                {
                    case "hwid":
                        hwid = CXML.DocumentElement.ChildNodes[i].InnerText.ToUpper();
                        break;
                    case "enable_flat_file_storage":
                        enable_flat_file_storage = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_gps_debug":
                        enable_gps_debug = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_tag_list":
                        enable_tag_list = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "gps_timer":
                        gps_timer = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "tag_list":
                        tag_list.Add(CXML.DocumentElement.ChildNodes[i].InnerText.ToUpper());
                        break;
                    case "enable_gps":
                        enable_gps = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "read_duration_ms":
                        read_duration_ms = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "read_pause_time_ms":
                        read_pause_time_ms = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "park_antenna":
                        park_antenna = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_trigger":
                        enable_trigger = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "reader_ip":
                        reader_ip = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "server":
                        server = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "port":
                        port = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "username":
                        username = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "password":
                        password = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_debug":
                        enable_debug = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_pcmode":
                        enable_pcmode = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_live_data_transmit":
                        enableLiveDataTransmit = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "enable_dump":
                        enable_dump = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "truck":
                        truckid = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "route":
                        routeid = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "transmit_mode":
                        transmitMode = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "pingserver":
                        pingServer = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ftp_server":
                        ftpServer = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ftp_username":
                        ftpUsername = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ftp_password":
                        ftpPassword = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ftp_dir":
                        ftpDir = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ex_dir":
                        exDir = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "bl_dir":
                        blDir = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ants":
                        ants = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ant1":
                        ant1 = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ant2":
                        ant2 = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ant3":
                        ant3 = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "ant4":
                        ant4 = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "delimiter":
                        delimiter = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "program_name":
                        programName = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "gps":
                        gps = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "observations":
                        observations = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "cc_tag":
                        cc_tag = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "transmit_username":
                        transmit_username = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "transmit_password":
                        transmit_password = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "pc_com_port":
                        pc_com_port = CXML.DocumentElement.ChildNodes[i].InnerText;
                        break;
                    case "arm_timer":
                        arm_timer = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "transmit_timer":
                        transmit_timer = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "transmit_maxCount":
                        transmit_maxCount = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "timer_gpo_2":
                        timerGPO2 = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "timer_gpo_3":
                        timerGPO3 = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "timer_gpo_4":
                        timerGPO4 = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "data_refresh_rate":
                        dataRefreshRate = ConvertToInt32(CXML.DocumentElement.ChildNodes[i].InnerText);
                        break;
                    case "emailAddressList":
                        emailAddressList = CXML.DocumentElement.ChildNodes[i].InnerText.Split(',');
                        break;
                }
            }

            //Close the filestream
            fs.Close();
            fs = null;
        }

    }
}
