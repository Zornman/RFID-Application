using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Intermec.DataCollection.RFID;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Timers;
using System.IO;
using Sonrai.Mobile.BusinessLogic;
using System.Data;
using System.Runtime.InteropServices;
using Mono.Data.Sqlite;
using System.Net;
using System.Diagnostics;
using System.Net.Mail;
using System.Linq;
using System.Globalization;

namespace SimpleConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            myAppsThread oAlpha = new myAppsThread();

            oAlpha.sVersion = "v3 - Alec's Version - The Working One";
            System.Console.WriteLine("===================================================================================");
            System.Console.WriteLine("Program Version: " + oAlpha.sVersion);
            System.Console.WriteLine("- Application runs on IF2+ and IF61 readers");
            System.Console.WriteLine("- Works as trigger and nontrigger on both the readers");
            System.Console.WriteLine("- Added in e-mail notifications for improved tech support");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("**IF YOU NEED TO EDIT THE APP, USE THE USER APP CREATOR ONLY**");
            System.Console.WriteLine("**DO NOT EDIT THE CONFIG FILE WITHOUT PERMISSION**");
            //System.Console.WriteLine("Both antennas triggered. Unique tags recorded");
            //System.Console.WriteLine("Update to 1.26.2");
            //System.Console.WriteLine("Sending data to new web service");
            //System.Console.WriteLine("Update to 1.27.2");
            //System.Console.WriteLine("Antenna on for 5 seconds when trigger goes high");
            System.Console.WriteLine("===================================================================================");

            settings.enable_debug = "1";
            settings.configure();

            // Create the thread object, passing in the Alpha.Beta method
            // via a ThreadStart delegate. This doesx not start the thread.
            Thread oThread = new Thread(new ThreadStart(oAlpha.RunApp));

            //turn on debug messages written to the console
            if (settings.enable_debug == "1")
                oAlpha.bDebug = true;
            else
                oAlpha.bDebug = false;

            // Start the thread
            oThread.Start();

            //wait till thread starts up
            while (!oThread.IsAlive) ;

            GPS oGPS = new GPS();
            
            Thread jThread;

            if (settings.enable_gps == "1")
            {
                if (!string.IsNullOrEmpty(settings.gps_timer))
                {
                    try
                    {
                        int gpstime = Convert.ToInt16(settings.gps_timer);
                        oGPS.gpstimer = gpstime;
                        System.Console.WriteLine("--> Beta GPS time set to XML file value of: " + oGPS.gpstimer.ToString());
                    }
                    catch (Exception e)
                    {
                        oGPS.gpstimer = 1000;
                        System.Console.WriteLine("--> Beta GPS time exception");
                    }
                }
                else
                {
                    System.Console.WriteLine("--> Beta GPS XML value null");
                    oGPS.gpstimer = 1000;
                }

                System.Console.WriteLine("--> Beta GPS thread starting");
                jThread = new Thread(new ThreadStart(oGPS.Beta));
                jThread.Start();
                jThread.Join();
                while (!jThread.IsAlive) ;
                System.Console.WriteLine("--> Beta GPS thread running");
            }

            while (oAlpha.bKill == false)
                Thread.Sleep(1000);

            System.Console.WriteLine("Goodbye");
        }

        public class myAppsThread
        {
            public bool bDebug = false;
            public bool bKill = false;
            public bool AppIsRunning = false;
            public string sVersion = "";

            DataAccess DA = new DataAccess();

            RFIDReaderClass MyReader = new RFIDReaderClass();

            public void RunApp()
            {
                bool bPassPowerTest = true;
                bool bStatus = false;
                string sURL = null;
                int read_duration_ms = 5000;
                int read_pause_time_ms = 1000;
                int loopcount = 0;
                int maxloopcount = 0;

                bKill = false;

                TestDBWrite();

                //datatransmit oDataTransmit = new datatransmit();

                //if (settings.enable_flat_file_storage == "1")
                //    TagStorage.LoadTagsFromDB();

                if (settings.enable_trigger == "0")
                {
                    read_duration_ms = ConvertStrToInt(settings.read_duration_ms, 1000);
                    read_pause_time_ms = ConvertStrToInt(settings.read_pause_time_ms, 1000);
                    if (read_pause_time_ms == 0)
                        maxloopcount = 10;
                    else
                        maxloopcount = settings.transmit_timer / read_pause_time_ms;
                }

                //System.Console.WriteLine("");
                //System.Console.WriteLine("*******************************************************");
                //bPassPowerTest = MyReader.RunPowerCalibrationTest();
                //bPassPowerTest = true;
                //System.Console.WriteLine("*******************************************************");
                //System.Console.WriteLine("");

                if (bPassPowerTest)
                {
                    sURL = GetIPAddress();

                    bStatus = MyReader.CreateReader(sURL);

                    MyReader.VerifyAntennaFaultDiabled(sURL);
                    MyReader.bDebug = bDebug;

                    if (!bStatus)
                    {
                        System.Console.WriteLine("Console Application failed to open reader and aborting");
                        MyReader = null;
                        bStatus = false;
                    }
                    else if (bStatus)
                    {
                        Console.WriteLine("DateTime received from NIST Server: " + GetNistTime());

                        Console.WriteLine("Sleeping 30 seconds...");
                        Thread.Sleep(30000);
                        Console.WriteLine("Awake and running...");
                        
                        AppIsRunning = true;
                        try
                        {
                            System.Console.WriteLine("App Started...");

                            //if (settings.enable_trigger == "1")
                            //    Console.WriteLine("Turning Triggers On...");

                            //=============================================================================================
                            //This is where the magic happens, it's the main portion of the app
                            //What doesn't make sense is why Jim would set the Thread.Sleep & StopReadingTags() functions
                            //Maybe to protect the reader modules?
                            //Commented them out because fuck that
                            //
                            //UPDATE AS OF 1/19/18
                            //- Standard and Trigger versions work without having timers built in, as they should
                            //- Only thing that needs to be tested is antenna faults
                            //=============================================================================================

                            var timer = new System.Timers.Timer(settings.transmit_timer);
                            timer.Elapsed += EventHandlerElapsed;
                            timer.Interval = settings.transmit_timer;

                            if (settings.enable_trigger == "0")
                            {
                                System.Console.WriteLine("====================STARTING NON-TRIGGER VERSION====================");
                                Console.WriteLine(" ");
                                Console.WriteLine("====================START READING TAGS====================");

                                while (AppIsRunning == true)
                                {
                                    MyReader.ReadTagsGen2Tags();
                                    timer.Enabled = true;
                                }
                            }
                            else if (settings.enable_trigger == "1")
                            {
                                System.Console.WriteLine("====================Starting TRIGGER Version====================");

                                while (AppIsRunning == true)
                                {
                                    timer.Enabled = true;
                                }
                            }
                            else
                            {
                                System.Console.WriteLine("====================Starting NON-TRIGGER Version====================");
                                Console.WriteLine(" ");
                                Console.WriteLine("====================START READING TAGS====================");

                                while (AppIsRunning == true)
                                {
                                    MyReader.ReadTagsGen2Tags();
                                    timer.Enabled = true;
                                }
                            }

                            //------------------------------------------------------------------------------------------------------------------
                            //while (AppIsRunning == true)
                            //{
                            //    if (settings.enable_trigger == "0")
                            //    {
                            //        //read tags when you are not using arm sensors
                            //        System.Console.WriteLine("--> Reading tags...");
                            //        MyReader.ReadTagsGen2Tags();
                            //        //Thread.Sleep(read_duration_ms);
                            //        //MyReader.brdr.StopReadingTags();
                            //        System.Console.WriteLine("--> Read stopped.");
                            //    }

                            //    if (settings.enable_trigger == "1")
                            //    {
                            //        //Thread.Sleep(settings.transmit_timer);
                            //        oDataTransmit.transmit();
                            //    }
                            //    else
                            //    {
                            //        Thread.Sleep(read_pause_time_ms);
                            //        if (loopcount >= maxloopcount)
                            //        {
                            //            oDataTransmit.transmit();
                            //            loopcount = 0;
                            //        }
                            //        else
                            //            loopcount++;
                            //    }
                            //}
                            //------------------------------------------------------------------------------------------------------------------
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.ToString());
                        }
                    }
                }

                if (MyReader != null) { MyReader.CloseReader(); MyReader = null; }
                bKill = true;
            }

            //Used for read duration and pause time, convert xml string to int...
            //Why do I need this?
            //Don't need read duration or pause time
            private int ConvertStrToInt(string s, int defaultval)
            {
                int i = 0;

                try
                {
                    i = Convert.ToInt16(s);
                }
                catch (Exception e)
                {
                    i = defaultval;
                }

                return i;
            }

            //This is used only for debugging purposes
            //We will only be testing it on an actual reader
            private string GetIPAddress()
            {
                string sURL = null;

                if (settings.enable_pcmode == "0")
                {
                    sURL = "TCP://" + "127.0.0.1" + ":2189";  //localhost connection in IF61/IF2.
                }
                else if (settings.enable_pcmode == "1")
                {
                    sURL = "TCP://" + settings.reader_ip + ":2189";  //used for debugging code on my PC.
                }

                return sURL;
            }

            private void TestDBWrite()
            {
                try
                {
                    DateTime dt = new DateTime();
                    dt = Convert.ToDateTime(GPS.date + " " + GPS.time);
                    DA.InsertExceptionRecord(8000, MyReader.GetDate());
                    Console.WriteLine(DA.GetExceptionCount());
                }
                catch (Exception e)
                {
                    Console.WriteLine("-->TestDBWrite(): exception!");
                }
            }

//=========================================================================================================================================================================
//Everything from this point on was written by me (Alec), anything above was done by either Atif or Jim OR I modified
//=========================================================================================================================================================================

            public static void SendEmailNotification(string MsgBody)
            {
                System.Console.WriteLine("====================Preparing to Send Email Notification====================");

                var sysLogin = "sonraialert@corporate-data.com";
                var sysPass = "@VaNLOf2";
                var sysAddress = new MailAddress(sysLogin, settings.programName.ToString() + "-" + settings.truckid.ToString() + " Needs Attention");

                var receiverAddress = new MailAddress("service@sonraisystems.com");

                var smtp = new SmtpClient
                {
                    Host = "us2.smtp.mailhostbox.com",   //mailhostbox? James sent the info in an email to Alec.Zorn@sonraisystems.com
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sysLogin, sysPass)
                };

                using (var message = new MailMessage(sysAddress, receiverAddress) { Subject = settings.programName.ToString() + "-" + settings.truckid.ToString() + " is having an issue", Body = MsgBody })
                {
                    try
                    {

                        for (int i = 0; i < settings.emailAddressList.Length; i++)
                        {
                            message.To.Add(settings.emailAddressList[i]);
                        }

                        smtp.Send(message);
                        System.Console.WriteLine("--- Sent email successfully! ---");
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(message);
                            }
                            else
                            {
                                Console.WriteLine("Failed to deliver message to " +
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in RetryIfBusy(): " +
                                ex.ToString());
                    }
                }
            }

            // Overloaded SendEmailNotification() to accept email attachments if applicable
            // Haven't been able to test this as I HAVE NO CLUE what causes a corrupt DB file to occur
            // As far as I can tell, it should work. But I won't know for sure until it's tested
            // Chris claims it comes from when a tag is trying to write to the DB file and the power gets shut off
            // I tested that theory with this app and it didn't work, so I have no idea
            // And this might not even be necessary for the IF2+ but it will be used with the IF61
            // The whole corrupt DB thing doesn't make sense to any of us...
            public static void SendEmailNotification(string MsgBody, string attachmentFilename)
            {
                System.Console.WriteLine("====================Preparing to send Email Notification with Attached Corrupt DB====================");

                // Sets the login and password, if this stops working reach out to James
                // He should have set the domain up for automatic renewal because I had an authentication issue
                var sysLogin = "sonraialert@corporate-data.com";
                var sysPass = "@VaNLOf2";
                var sysAddress = new MailAddress(sysLogin, settings.programName.ToString() + "-" + settings.truckid.ToString() + " needs attention");

                var receiverAddress = new MailAddress("service@sonraisystems.com");

                var smtp = new SmtpClient
                {
                    Host = "us2.smtp.mailhostbox.com",   //mailhostbox? James sent the info in an email to Alec.Zorn@sonraisystems.com
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(sysLogin, sysPass)
                };

                using (var message = new MailMessage(sysAddress, receiverAddress) { Subject = settings.programName.ToString() + "-" + settings.truckid.ToString() + " has a corrupt DB file", Body = MsgBody })
                {
                    try
                    {
                        for(int i = 0; i < settings.emailAddressList.Length; i++)
                        {
                            message.To.Add(settings.emailAddressList[i]);
                        }

                        message.Attachments.Add(new Attachment(attachmentFilename));
                        smtp.Send(message);
                        System.Console.WriteLine("Sent email successfully!");
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(message);
                            }
                            else
                            {
                                Console.WriteLine("Failed to deliver message to " +
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in RetryIfBusy(): " +
                                ex.ToString());
                    }
                }
            }

            // https://stackoverflow.com/questions/6435099/how-to-get-datetime-from-the-internet
            // Code for GetNistTime() was found at the above link

            public static DateTime GetNistTime()
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
                var response = myHttpWebRequest.GetResponse();
                string todaysDates = response.Headers["date"];
                return DateTime.ParseExact(todaysDates,
                                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                           CultureInfo.InvariantCulture.DateTimeFormat,
                                           DateTimeStyles.AssumeUniversal);
            }

            public void EventHandlerElapsed(object sender, ElapsedEventArgs e)
            {
                datatransmit oDataTransmit = new datatransmit();

                //Console.WriteLine("Calling Data Transmission Every: " + (settings.transmit_timer / 1000).ToString() + " Seconds");
                oDataTransmit.transmit();
            }
        }//END public class myAppsThread
    }//END class Program
}//END namespace SimpleConsoleApplication

