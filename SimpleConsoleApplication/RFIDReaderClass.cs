using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Intermec.DataCollection.RFID;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Data.SqlClient;
using Sonrai.Mobile.BusinessLogic;
using PowerCalibrationDll;

using System.IO;
using System.Data;
using SimpleConsoleApplication.WebReferenceHoneywellWSDL;
using System.Web.Services;
using System.Net;
using System.Diagnostics;

namespace SimpleConsoleApplication
{
    class RFIDReaderClass
    {
        PowerCalibrationDll.PowerCalibrationDll myPT = new PowerCalibrationDll.PowerCalibrationDll();

        WebReferenceHoneywellWSDL.DeviceConfiguration mywebserv = null;

        DataAccess DA = new DataAccess();
        public bool bDebug = true;
        public string[] TagANTList = new string[1500];

        public string lat = "0";
        public string lon = "0";

        public ArrayList MyTagCollection = new ArrayList();
        public int TagCount = 0;
       
        //Changed on 11-30-2012       
        public string tagID = "N/A";
        public string tag_ts = "";
        public bool readState = false;

        public string tmpTagTS = "";
        public string tmpArmTag = "";

        public System.Timers.Timer timerRed = new System.Timers.Timer();
        public System.Timers.Timer timerAlarm = new System.Timers.Timer();
        public System.Timers.Timer timerGreen = new System.Timers.Timer();

        public System.Timers.Timer timerStopRead = new System.Timers.Timer();
        public System.Timers.Timer timerNA_LED = new System.Timers.Timer();

        public System.Timers.Timer timerArmTimer = new System.Timers.Timer();

        public int tRed = 0;
        public int tAlarm = 0;
        public int tGreen = 0;
        
        public BRIReader brdr = null;

        #region Red Timer Event Handler
        public void OnTimerRedEvent(object source, EventArgs e)
        {
            try
            {
                if (tRed > 0)
                {
                    brdr.Execute("WRITEGPO 1 ON");
                    brdr.Execute("WRITEGPO 2 OFF");
                    timerRed.Stop();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX OnTimerRedEvent" + Ex.Message);
            }
        }
        #endregion

        #region Alarm Timer Event Handler
        public void OnTimerAlarmEvent(object source, EventArgs e)
        {
            //Console.WriteLine("AlarmEvent");
            try
            {
                if (tAlarm != 0)
                {
                    brdr.Execute("WriteGPO 3 OFF");
                    timerAlarm.Stop();
                    //brdr.Execute("WriteGPO 1 ON");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX OnTimerAlarmEvent" + Ex.Message);
            }
        }
        #endregion

        #region Green Timer Event Handler
        public void OnTimerGreenEvent(object source, EventArgs e)
        {
            //if (timerReset.Enabled != true)
            //{
            //    timerReset.Enabled = true;
            //    timerReset.Start();
            //    Console.WriteLine("Reset Timer Started");
            //}

            if (tGreen != 0)
            {
                try
                {
                    brdr.Execute("WriteGPO 4 OFF");
                    brdr.Execute("WriteGPO 1 ON");
                    timerGreen.Stop();
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("EX OnTimerGreenEvent" + Ex.Message);
                }
            }
        }
        #endregion

        #region Reset Timer Event Handler
        //public void OnTimerResetEvent(object source, EventArgs e)
        //{
        //    Console.WriteLine("GreenEvent - Poll Tags");
        //    brdr.Execute("READ POLL");
        //    if (timerReset.Enabled == true)
        //    {
        //        brdr.PollTags();
        //        tRed = 0;
        //        tAlarm = 0;
        //        tGreen = 0;
        //    }
        //}
        #endregion

        #region Close Reader
        public void CloseReader()
        {
            //delete macros and triggers
            //dispose of IDL reader class
            string sMsg = null;
            try
            {
                if (brdr != null)
                {
                    if (bDebug == true) { System.Console.WriteLine("Deleting triggers and macros..."); }
                    sMsg = brdr.Execute("TRIGGER RESET");
                    if (bDebug == true) { System.Console.WriteLine("TRIGGER RESET -> " + sMsg); }
                    brdr.StopReadingTags();
                    brdr.Dispose();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX CloseReader: " + Ex.Message);
            }
        }
        #endregion

        #region Create Reader
        public bool CreateReader(string sURL)
        {
            string tMsg = null;
            bool bStatus = false;

            if (bDebug == true) { System.Console.WriteLine("Creating rfid connections..."); }

            tRed = 0;
            tAlarm = 0;
            tGreen = 0;

            timerRed.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerRedEvent);
            timerAlarm.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerAlarmEvent);
            timerGreen.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerGreenEvent);
            timerStopRead.Elapsed += new System.Timers.ElapsedEventHandler(timerStopRead_Elapsed);
            timerNA_LED.Elapsed += new System.Timers.ElapsedEventHandler(timerNA_LED_Elapsed);
            timerArmTimer.Elapsed += new System.Timers.ElapsedEventHandler(timerArmTimer_Elapsed);

            //try connecting to reader till success or 5 attempts
            for (int x = 0; x < 5; x++)
            {
                try
                {
                    //Create reader connection
                    brdr = null;
                    brdr = new BRIReader(null, sURL, 22000, 2000);
                    if (bDebug == true) { System.Console.WriteLine("Connection created"); }
                    tMsg = brdr.Execute("VER");
                    if (bDebug == true) { System.Console.WriteLine("VER->" + tMsg); }
                    if (tMsg.IndexOf("OK>") >= 0)
                    {
                        tMsg = brdr.Execute("TRIGGER RESET");
                        if (bDebug == true) { System.Console.WriteLine("TRIGGER RESET->" + tMsg); }
                        //turn off gpio output lines.
                        tMsg = brdr.Execute("WRITEGPIO=15");
                        if (bDebug == true) { System.Console.WriteLine("WRITEGPIO=15->" + tMsg); }
                        bStatus = AddEventHandlers();

                        GPS.bdebug = bDebug;
                        bStatus = true;
                        break;
                    }
                }
                catch (BasicReaderException eBRI)
                {
                    System.Console.WriteLine("EX CreateReader - BasicReaderException:\r\n" + eBRI.ToString());
                    bStatus = false;
                    return bStatus;
                }
            }

            if (!bStatus) { return bStatus; }

            if (bDebug == true) { System.Console.WriteLine("Reader Ready...starting configuration..."); }

            //configure reader attributes
            ConfigGen2Settings();

            //create the gpio triggers and macros to execute reads
            if (settings.enable_trigger == "1")
                CreateTriggers();

            return bStatus;
        }                    
        #endregion

        #region Create Triggers
        private void CreateTriggers()
        {
            //create the gpio triggers and macros to execute reads

            string[] sCMDsList = new string[9];
            string sMsg = null;

            System.Console.WriteLine("Creating observation triggers...");

            sCMDsList[0] = "TRIGGER RESET";
            sCMDsList[1] = "TRIGGER \"Input1ON\" GPIOEDGE 1 0 FILTER 3000";
            sCMDsList[2] = "TRIGGER \"Input1OFF\" GPIOEDGE 1 1 FILTER 3000";
            sCMDsList[3] = "TRIGGER \"Input2ON\" GPIOEDGE 2 0 FILTER 3000";
            sCMDsList[4] = "TRIGGER \"Input2OFF\" GPIOEDGE 2 2  FILTER 3000";
            sCMDsList[5] = "TRIGGER \"Input3ON\" GPIOEDGE 4 0 FILTER 3000";
            sCMDsList[6] = "TRIGGER \"Input3OFF\" GPIOEDGE 4 4 FILTER 3000";
            if (settings.enable_trigger == "1")
            {
                sMsg = "TRIGGER \"ArmUp\" GPIOEDGE 8 0 FILTER 3000";
                sCMDsList[7] =sMsg;
                sMsg = "TRIGGER \"ArmDown\" GPIOEDGE 8 8 FILTER 3000";
                sCMDsList[8] = sMsg;
            }
            else
            {
                sCMDsList[7] = "";
                sCMDsList[8] = "";
            }

            try
            {
                for (int x = 0; x < sCMDsList.Length; x++)
                {
                    sMsg = brdr.Execute(sCMDsList[x]);
                    if (bDebug == true) { System.Console.WriteLine(sCMDsList[x] + "->" + sMsg); }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX CreateTriggers: " + Ex.Message);
            }
        }
        #endregion

        #region Configure Reader
        private bool ConfigGen2Settings()
        {
            bool retVal = false;
            string sMsg = null;
            string[] sCMDsList = new string[15];
            
            sCMDsList[0] = "ATTRIB ANTS=" + settings.ants;
            sCMDsList[1] = "ATTRIB IDTRIES=1";
            sCMDsList[2] = "ATTRIB ANTTRIES=1";
            sCMDsList[3] = "ATTRIB SCHEDOPT=1";
            sCMDsList[4] = "ATTRIB TIMEOUTMODE=OFF";
            sCMDsList[5] = "ATTRIB INITIALQ=1";
            sCMDsList[6] = "ATTRIB SESSION=2";
            sCMDsList[7] = "ATTRIB NOTAGRPT=ON";
            sCMDsList[8] = "ATTRIB IDREPORT=ON";
            sCMDsList[9] =  "ATTRIB TAGTYPE=EPCC1G2";
            sCMDsList[10] = "ATTRIB FIELDSTRENGTH=" + settings.ant1 + "," + settings.ant2 + "," + settings.ant3 + "," + settings.ant4;
            sCMDsList[11] = "ATTRIB IDTIMEOUT=0";
            sCMDsList[12] = "ATTRIB ANTTIMEOUT=0";
            sCMDsList[13] = "ATTRIB";
            sCMDsList[14] = "WRITEGPO 1 ON";

            try
            {
                for (int x = 0; x < sCMDsList.Length; x++)
                {
                    sMsg = brdr.Execute(sCMDsList[x]);
                    if (x==14 && bDebug == true) { System.Console.WriteLine("-> Get Attributes:"); }
                    if (bDebug == true) { System.Console.WriteLine(sCMDsList[x] + "->" + sMsg); }
                }
                retVal = true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX ConfigGen2Settings: " + Ex.Message);
                retVal = false;
            }
            return retVal;
        }
        #endregion

        #region Read Gen2 Tags
        public void ReadTagsGen2Tags()
        {
            try
            {
                brdr.StartReadingTags(null, "ANT", BRIReader.TagReportOptions.EVENT);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX ReadTagsGen2Tags: " + Ex.Message);
            }
        }
        #endregion

        #region Load Tags
        private void LoadTags(BRIReader tRdr, int iRdrIndex)
        {
            //this code is not currently used.

            //retrieve tag ids
            int RspCount = 0;
            string sTagID = null;


            foreach (Tag tt in tRdr.Tags)
            {
                RspCount++;
                sTagID = tt.ToString();
                if (tt.TagFields.ItemCount > 0)
                {
                    foreach (TagField tf in tt.TagFields.FieldArray)
                    {
                        //get field data
                        sTagID += "," + tf.ToString();
                        break;
                    }
                }
                if (bDebug == true) { System.Console.WriteLine("TagID -> " + sTagID + " " + DateTime.Now.ToString()); }
            }
        }
        #endregion

        #region Event Handlers
        private bool AddEventHandlers()
        {
            //*********************************************************************
            // Add the event handlers
            //*********************************************************************

            try
            {
                this.brdr.EventHandlerTag += new Tag_EventHandlerAdv(brdr_EventHandlerTag);
                this.brdr.EventHandlerGPIO += new GPIO_EventHandlerAdv(brdr_EventHandlerGPIO);
                this.brdr.EventHandlerRadio += new Radio_EventHandlerAdv(brdr_EventHandlerRadio);
                this.brdr.EventHandlerThermal += new Thermal_EventHandler(brdr_EventHandlerThermal);
            }
            catch
            {
                if (bDebug == true) { System.Console.WriteLine("AddEventHandlers(): Error trying to create event handlers"); }
                return false;
            }
            return true;
        }

        void brdr_EventHandlerThermal(object sender, Thermal_EventArgs EvtArgs)
        {
            System.Console.WriteLine("--> Warning Thermal Event!!! " + EvtArgs.StateString);
        }

        #endregion 

        #region Radio Event Handler
        void brdr_EventHandlerRadio(object sender, EVTADV_Radio_EventArgs EvtArgs)
        {
            if (bDebug == true) { System.Console.WriteLine("brdr_EventHandlerRadio: " + EvtArgs.ToString()); }
        }
        #endregion

        #region GPIO Event Handler
        void brdr_EventHandlerGPIO(object sender, EVTADV_GPIO_EventArgs EvtArgs)
        {
            //This will only fire if you have defined GPIO TRIGGERS
            lat = GPS.Latitude;
            lon = GPS.Longitude;

            try
            {
                Console.WriteLine(EvtArgs.TriggerNameString + ": Trigger Activated");

                if (EvtArgs.TriggerNameString == "ArmUp")
                {
                    //System.Console.WriteLine("Arm Position: Up");
                    //Turn OFF ANT2, Turn ON ANT1
                    //07-17-2013: Turn on both antennas to start reading tags
                    try
                    {
                        try
                        {
                            tagID = "N/A";
                            brdr.Execute("ATTRIB ANTS=" + settings.ants);
                            brdr.StartReadingTags(null, "ANT", BRIReader.TagReportOptions.EVENT);
                            Console.WriteLine("----- Start Reading Tags -----");
                        }
                        catch (Exception Ex)
                        { 
                            Console.WriteLine("EX ReadTagsGen2Tags: " + Ex.Message); 
                        }
                        
                        //start a 5 second timer
                        if (readState == false)
                        {
                            readState = true;
                            //timerStopRead.Interval = settings.arm_timer;
                            //timerStopRead.Enabled = true;
                        }
                        else
                        {
                            Console.WriteLine("System is already reading tags");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ArmUP EX: " + ex.Message.ToString());
                        
                    }
                }
                else if (EvtArgs.TriggerNameString == "ArmDown")
                {
                    brdr.StopReadingTags();
                    Console.WriteLine("----- Stop Reading Tags -----");
                    Console.WriteLine(EvtArgs.TriggerNameString + ": Trigger Deactivated");

                    if (tagID == "N/A")
                    {
                        if (bDebug) System.Console.WriteLine("Trying to InsertPickupRecord N/A Tag to DB...");
                        tag_ts = GetDate();
                        DA.InsertPickupRecord(tagID.ToString(), settings.programName, settings.truckid, GetDate(), lat, lon);
                        Console.WriteLine(tagID.ToString() + " | " + settings.programName + " | " + settings.truckid + " | " + tag_ts + " | " + lat + " | " + lon);
                        //timerNA_LED.Interval = 200;
                        //timerNA_LED.Enabled = true;
                    }
                }

                #region Observation Handling Code
                //06-17-2013: Power input not handled in GPIO input triggers
                //06-17-2013: Observation inputs enabled for Curotto Can program
                if (EvtArgs.TriggerNameString == "Input1ON")
                {
                    Console.WriteLine("GPS Date Time: " + tag_ts);
                    DA.InsertObservationRecord_New(10001, tag_ts, tagID);

                    brdr.Execute("WriteGPO 2 ON");
                    brdr.Execute("WriteGPO 1 OFF");
                    timerRed.Interval = settings.timerGPO4;
                    timerRed.Enabled = true;
                    timerRed.Start();
                    tRed = 1;
                }
                if (EvtArgs.TriggerNameString == "Input2ON")
                {
                    Console.WriteLine("GPS Date Time: " + tag_ts);
                    DA.InsertObservationRecord_New(10002, tag_ts, tagID);

                    brdr.Execute("WriteGPO 2 ON");
                    brdr.Execute("WriteGPO 1 OFF");
                    timerRed.Interval = settings.timerGPO4;
                    timerRed.Enabled = true;
                    timerRed.Start();
                    tRed = 1;
                }
                if (EvtArgs.TriggerNameString == "Input3ON")
                {
                    Console.WriteLine("Date Time: " + tag_ts);
                    DA.InsertObservationRecord_New(10003, tag_ts, tagID);

                    brdr.Execute("WriteGPO 2 ON");
                    brdr.Execute("WriteGPO 1 OFF");
                    timerRed.Interval = settings.timerGPO4;
                    timerRed.Enabled = true;
                    timerRed.Start();
                    tRed = 1;
                }
                #endregion
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX brdr_EventHandlerGPIO: " + Ex.Message);
            }
        }
        #endregion

        //public bool RunPowerCalibrationTest()
        //{
        //    bool b = false;

        //    Console.WriteLine("Starting Power Calibration Test...");

        //    myPT.Attrib_Antennas = settings.ants;

        //    b = myPT.RunPowerCalibrationTest(null); //use default

        //    if (b)
        //    {
        //        Console.WriteLine("");
        //        Console.WriteLine("***************************************************************");
        //        Console.WriteLine("Passed Power Calibration Test");
        //        Console.WriteLine("***************************************************************");
        //        Console.WriteLine("");
        //    }
        //    else
        //    {
        //        Console.WriteLine("");
        //        Console.WriteLine("***************************************************************");
        //        Console.WriteLine("Failed Power Calibration Test");
        //        Console.WriteLine("Power Table Value =" + myPT.MinPowerAdjustValue);
        //        Console.WriteLine("Current Power Setting =" + myPT.CurrentPowerAdjustValue);
        //        if (!string.IsNullOrEmpty(myPT.Error_Message))
        //            Console.WriteLine("Error Message: " + myPT.Error_Message);
        //        Console.WriteLine("***************************************************************");
        //        Console.WriteLine("");
        //    }

        //    Console.WriteLine("Power Calibration Test Completed");

        //    return b;
        //}

        //Does what it's called, verifies that antenna faults are disabled

        public bool VerifyAntennaFaultDiabled(string sURL)
        {
            bool b = false;
            string rsp = null;

            string ip = sURL.Replace("TCP://", "").Replace(":2189", "").Trim();
            
            rsp = brdr.Execute("UTIL ENABLE=\"EV98203U\"");
            rsp = brdr.Execute("util nv radio\\antfault_enable");

            if (rsp.IndexOf("RADIO\\ANTFAULT_ENABLE(BYTE):") >= 0)
            {
                if (rsp.IndexOf("RADIO\\ANTFAULT_ENABLE(BYTE):   0 (0x00)") == 0)
                {
                    //Antenna faults disabled
                    System.Console.WriteLine("Antenna faults already disabled!");
                    return true;
                }
                else
                    b = DisableAntennaFaults(rsp, ip);
            }
            else if (rsp.IndexOf("ERR") >= 0)
                b = DisableAntennaFaults(rsp, ip);

            return b;
        }

        //If antenna faults aren't disabled, this function will disable them
        private bool DisableAntennaFaults(string rsp, string ip)
        {
            //rsp = brdr.Execute("util enable=\"EV98203U\"");
            rsp = brdr.Execute("util nv radio\\antfault_enable=0");
            //rsp = brdr.Execute("reset");

            //verify setting
            rsp = brdr.Execute("util nv radio\\antfault_enable");

            if (rsp.IndexOf("RADIO\\ANTFAULT_ENABLE(BYTE):   0 (0x00)") == 0)
            {
                //Antenna faults enabled.
                System.Console.WriteLine("--> Antenna faults disabled!");

                RebootReader(ip);

                return true;
            }
            else
            {
                System.Console.WriteLine("--> Failed to disable antenna faults!");
                return false;
            }
        }


        //Will reboot the reader when called (Not too sure on how exactly this works)
        //From what I understand, these IF2+ readers don't reboot very easily
        public bool RebootReader(string myIPAdddress)
        {
            string user = "intermec";
            string pass = "intermec";

            string host = "http://" + myIPAdddress + ":64907";

            Console.WriteLine("sending reboot to: " + host);

            try
            {
                System.Console.WriteLine("trying to reboot reader...");
                mywebserv = new WebReferenceHoneywellWSDL.DeviceConfiguration();
                mywebserv.Url = host;
                mywebserv.Credentials = new NetworkCredential(user, pass);
                mywebserv.RebootDevice();
                System.Console.WriteLine("reboot sent!");
                mywebserv.Dispose();
                mywebserv = null;
                return true;
            }
            catch (Exception)
            {
                mywebserv = null;
            }

            return false;
        }

        public void timerStopRead_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerStopRead.Enabled = false;
            readState = false;
            Console.WriteLine("==============Timer Elapsed. Stopping Tag Reads.=================");
            //stop reading tags
            //record N/A if no tags read
            try
            {
                brdr.StopReadingTags();
                if (settings.park_antenna != null)
                    if (settings.park_antenna.Length > 0)
                        brdr.Execute("ATTRIB ANTS=" + settings.park_antenna);
            }
            catch (Exception ex)
            {
            }

            //if (settings.enable_trigger == "1")
            //{
                if (tagID == "N/A")
                {
                    if (bDebug) System.Console.WriteLine("--> Trying to InsertPickupRecord N/A Tag to DB...");
                    tag_ts = GetDate();
                    DA.InsertPickupRecord(tagID.ToString(), settings.programName, settings.truckid, GetDate(), lat, lon);
                    Console.WriteLine(tagID.ToString() + " | " + settings.programName + " | " + settings.truckid + " | " + tag_ts + " | " + lat + " | " + lon);
                    timerNA_LED.Interval = 200;
                    timerNA_LED.Enabled = true;
                }
            //}
        }

        void timerNA_LED_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerNA_LED.Enabled = false;
            //Turn on Red LED
            brdr.Execute("WRITEGPO 2 ON");
            Thread.Sleep(500);
            //Turn on Green LED
            brdr.Execute("WRITEGPO 4 ON");
            Thread.Sleep(800);
            //Turn off both Read and Green LEDs
            brdr.Execute("WRITEGPO 2 OFF");
            brdr.Execute("WRITEGPO 4 OFF");
        }

        void timerArmTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("--> timerArmTimer_Elapsed() Fired");
            timerArmTimer.Enabled = false;
            int pastCnt = DA.CountTagsReadByTime(Convert.ToDateTime(tmpTagTS).AddSeconds((settings.arm_timer/1000)*-1).ToString("yyyy-MM-dd HH:mm:ss"),tmpArmTag);
            if (pastCnt > 0 || tagID != tmpArmTag)
            {
                //ignore arm tag
                //keep reading
                try
                {
                    Console.WriteLine("Ignoring arm tag");
                    //Turn on Red LED
                    brdr.Execute("WRITEGPO 2 ON");
                    Thread.Sleep(500);
                    //Turn on Green LED
                    brdr.Execute("WRITEGPO 4 ON");
                    Thread.Sleep(800);
                    //Turn off both Read and Green LEDs
                    brdr.Execute("WRITEGPO 2 OFF");
                    brdr.Execute("WRITEGPO 4 OFF");
                    brdr.StopReadingTags();
                    ReadTagsGen2Tags();
                }
                catch (Exception ex)
                {                    
                }
            }
            else
            { 
                //insert arm tag 
                if (bDebug) System.Console.WriteLine("--> Arm Timer: Trying to InsertPickupRecord N/A Tag to DB...");
                DA.InsertPickupRecord(tmpArmTag, settings.programName, settings.truckid, tmpTagTS, lat, lon);
                try
                {
                    brdr.StopReadingTags();
                    ReadTagsGen2Tags();
                }
                catch (Exception ex)
                {
                }
                brdr.Execute("WriteGPO 3 ON");
                brdr.Execute("WriteGPO 1 OFF");
                timerAlarm.Interval = settings.timerGPO3;
                timerAlarm.Enabled = true;
                timerAlarm.Start();
                tAlarm = 1;

                brdr.Execute("WriteGPO 4 ON");
                timerGreen.Interval = settings.timerGPO4;
                timerGreen.Enabled = true;
                timerGreen.Start();
                tGreen = 1;   
            }

        } 

        #region Reader Report=EVENT Event Handler
        public void brdr_EventHandlerTag(object sender, EVTADV_Tag_EventArgs EvtArgs)
        {
            //will only fire if you are using READ ... REPORT=EVENT
            //good app
            string epc = EvtArgs.Tag.ToString();
            string antenna = EvtArgs.Tag.TagFields.FieldArray[0].ToString();
            string datetimestamp = DateTime.Now.ToString("yyyy-MM-dd,HH:mm:ss");

            lat = GPS.Latitude;
            lon = GPS.Longitude;

            

            try
            {
                if (epc == settings.cc_tag.ToString())
                {
                    System.Console.WriteLine("===========   cc_tag Tag Read  ANT:" + antenna + "  ===========");
                    tmpArmTag = epc;
                    tagID = epc;
                    tmpTagTS = GetDate();
                    timerArmTimer.Interval = settings.arm_timer;
                    timerArmTimer.Enabled = true;
                    tag_ts = GetDate();
                }
                else
                {
                    
                    //System.Console.WriteLine("--> Tag: " + epc + " Antenna: " + antenna + " lat: " + lat + " lon: " + lon);
            
                    if (settings.enable_tag_list == "1")
                    {
                        if (!CheckTagList(epc.Substring(0, 4)))
                        {
                            Console.WriteLine("--> ***************** Tag is NOT in approved list!  Aborting");
                            return;
                        }
                        else
                            Console.WriteLine("--> ***************** EPC value validated against approved list");
                    }

                    //true means not duplicate
                    if (!DA.CheckDuplicate(epc))
                    {
                        if(bDebug) System.Console.WriteLine("--> EventHandlerTag: Trying to InsertPickupRecord N/A Tag to DB...");

                        tagID = epc;
                        tag_ts = GetDate();

                        if (settings.enable_flat_file_storage == "0")
                            DA.InsertPickupRecord(tagID, settings.programName, settings.truckid, tag_ts, lat, lon);
                        
                        string data = tagID + "," + 
                            settings.programName + "," + 
                            settings.truckid + "," + 
                            tag_ts + "," + 
                            lat + "," + 
                            lon;

                        if (settings.enable_flat_file_storage == "1")
                        {
                            TagStorage.AddToTagList(epc, data);
                        }

                        brdr.Execute("WriteGPO 3 ON");
                        brdr.Execute("WriteGPO 1 OFF");
                        timerAlarm.Interval = settings.timerGPO3;
                        timerAlarm.Enabled = true;
                        timerAlarm.Start();
                        tAlarm = 1;

                        brdr.Execute("WriteGPO 4 ON");
                        timerGreen.Interval = settings.timerGPO4;
                        timerGreen.Enabled = true;
                        timerGreen.Start();
                        tGreen = 1;
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("EX brdr_EventHandlerTag: " + Ex.Message);
            }
        }
        #endregion

        private bool CheckTagList(string hex)
        {
            if (settings.tag_list.Contains(hex))
                return true;
            else
                return false;
        }

        public string GetDate()
        {
            string date = "2000-01-01 12:00:00";
            string type = "";

            try
            {
                if (settings.gps == "garmin")
                {
                    type = "garmin";
                    DateTime dt = new DateTime();
                    dt = Convert.ToDateTime(GPS.date + " " + GPS.time);
                    date = dt.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (settings.gps == "pinpoint")
                {
                    type = "pinpoint";
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    type = "other";
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                //Console.WriteLine(type + " " + date);
            }
            catch (Exception ex)
            {
                date = "2000-01-01 12:00:00";
            }

            return date;
        }

    }//END RFIDReaderClass
}//END namespace SimpleConsoleApplication
