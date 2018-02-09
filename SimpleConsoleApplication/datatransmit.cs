using System;
using System.Collections.Generic;
using System.Data;

namespace SimpleConsoleApplication
{
    class datatransmit
    {
        public static DataAccess DA = new DataAccess();
        com.sonraisystems.phoenix.SonraiDataService ds = new SimpleConsoleApplication.com.sonraisystems.phoenix.SonraiDataService();
        com.sonraisystems.sonraipickupsvc1.Service ds1 = new SimpleConsoleApplication.com.sonraisystems.sonraipickupsvc1.Service();
        public int maxCount;
        public string username;
        public string password;

        private void transmit_pickups()
        {
            string lastTS="";
            string lastTag="";
            bool result = false;
                   
            try
            {
                int RecCount = DA.GetNotTransmittedCount();
                
                if (RecCount > 0)
                {
                    RecCount = RecCount - 1;
                }
                //Console.WriteLine("Updated Untransmitted Records: " + RecCount.ToString());
                if (RecCount > 0)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("=== Starting Data Transmission ===");
                    //Console.WriteLine("transmit_pickups()");
                    Console.WriteLine("Max Record Count: " + maxCount.ToString());
                    Console.WriteLine("Untransmitted Records: " + RecCount.ToString());
                    settings.noTransmitTime = 0;

                    DataSet Ds = new DataSet();
                    Ds = DA.dsGetAllNotTransmitted();
                    if (Ds.Tables[0].Rows.Count <= maxCount)
                    {
                        Console.WriteLine("Record Count less than max: " + maxCount.ToString());
                        Console.WriteLine("=== Transmitting " + Ds.Tables[0].Rows.Count + " pickup records ===");
                        List<string> Records = new List<string>();
                        lastTS = Convert.ToDateTime(Ds.Tables[0].Rows[0]["timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        lastTag = Ds.Tables[0].Rows[0]["rfidtag"].ToString();
                        for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
                        {
                            string record = "";
                            record = Convert.ToDateTime(Ds.Tables[0].Rows[i]["timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff") + ","
                                        + Ds.Tables[0].Rows[i]["rfidtag"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["lon"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["lat"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["programname"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["truckid"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["observation1"].ToString() + Ds.Tables[0].Rows[i]["observation2"].ToString() + Ds.Tables[0].Rows[i]["observation3"].ToString();
                            Console.WriteLine("Record to send: " + record);

                            Records.Add(record);
                        }
                        Console.WriteLine("=== End Record List ===");
                        Console.WriteLine(" ");
                        bool resp = ds1.SendPickupRecords(Records.ToArray(), username, password);
                        Console.WriteLine(" ");
                        Console.WriteLine("=== Testing Web Service Communication ===");
                        Console.WriteLine("Web Service Response: " + resp.ToString());
                        result = resp;

                        if (result)
                        {
                            Console.WriteLine("Timestamp on WS Response: " + lastTS.ToString());
                            //Update transmitted records
                            DA.UpdateTransmitted(lastTS);
                            //Delete Obsolete records
                            DA.DeleteTransnmitted(lastTS);
                            Console.WriteLine("=== Finished Web Service Communication ===");
                        }
                        else
                        {
                            Console.WriteLine("=== Finished Web Service Communication ===");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Transmitting " + Ds.Tables[0].Rows.Count + " pickup records");
                        List<string> Records = new List<string>(); 
                        for (int i = Ds.Tables[0].Rows.Count-1; i >= 0; i--)
                        {
                            string record = "";
                            record = Convert.ToDateTime(Ds.Tables[0].Rows[i]["timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff") + ","
                                        + Ds.Tables[0].Rows[i]["rfidtag"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["lon"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["lat"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["programname"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["truckid"].ToString()
                                        + "," + Ds.Tables[0].Rows[i]["observation1"].ToString() + Ds.Tables[0].Rows[i]["observation2"].ToString() + Ds.Tables[0].Rows[i]["observation3"].ToString();
                            Records.Add(record);
                            lastTS = Convert.ToDateTime(Ds.Tables[0].Rows[i]["timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            lastTag = Ds.Tables[0].Rows[i]["rfidtag"].ToString();
                            if (Records.Count == maxCount || i == 0)
                            {
                                Console.WriteLine("Sending " + Records.Count.ToString() + " records of " + RecCount.ToString());
                                Console.WriteLine("=== Testing Web Service Communication ===");
                                bool resp = ds1.SendPickupRecords(Records.ToArray(), username, password);
                                Console.WriteLine("Web Service Response: " + resp.ToString());
                                result = resp;
                                if (result)
                                {
                                    Console.WriteLine("Timestamp on WS Response: " + lastTS.ToString());
                                    DA.UpdateTransmitted(lastTS);
                                    DA.DeleteTransnmitted(lastTS);
                                    Console.WriteLine("=== Finished Web Service Communication - PASSED ===");
                                }
                                else
                                {
                                    Console.WriteLine("=== Finished Web Service Communication - FAILED ===");
                                    break;
                                }
                                Records.Clear();
                            }
                        }
                    }

                    Console.WriteLine("=== Transmit Complete ===");
                    Console.WriteLine("Total Records: " + DA.GetAllRecordCount() + "  Not Transmitted: " + DA.GetNotTransmittedCount() + "  Exceptions: " + DA.GetExceptionCount());
                    Console.WriteLine(" ");
                }
                else if (RecCount == 0)
                {
                    settings.noTransmitTime++;
                    Console.WriteLine("Next Data Transmission Attempt: " + settings.transmit_timer / 1000 + " Seconds - Count: " + settings.noTransmitTime);

                    if (settings.sent_NoDataTransmitted == "0")
                    {
                        if (settings.noTransmitTime == 30)
                        {
                            string message = "No data has been received from this truck in: " + settings.noTransmitTime + " minutes";
                            Program.myAppsThread.SendEmailNotification(message);

                            settings.sent_NoDataTransmitted = "1";
                        }
                    }
                    else if (settings.sent_NoDataTransmitted == "1")
                    {
                        if (settings.noTransmitTime == 60)
                        {
                            string message = "No data has been received from this truck in: " + settings.noTransmitTime/60 + " hour";
                            Program.myAppsThread.SendEmailNotification(message);

                            settings.sent_NoDataTransmitted = "2";
                        }
                    }
                    
                }
                else
                {
                    //Do Nothing
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Pickup record transmission failed: " + Ex.Message);
                settings.transmissionFailed++;

                if(settings.transmissionFailed == 5)
                {
                    if (settings.sent_PickupRecordFailedNotification == "0")
                    {
                        string message = "Pickup record transmission has failed 5 times, check the system log for more details.";
                        Program.myAppsThread.SendEmailNotification(message);

                        settings.sent_PickupRecordFailedNotification = "1";
                    }
                } 
            }
        }

        private void transmit_exceptions()
        {
            //Transmit Exception Records
            try
            {
                if (DA.GetExceptionCount() > 0)
                {
                    //Call webservice to transmit data
                    List<com.sonraisystems.phoenix.clsException> Records = new List<SimpleConsoleApplication.com.sonraisystems.phoenix.clsException>();
                    DataSet Ds = new DataSet();
                    Ds = DA.dsGetAllExceptions();
                    Console.WriteLine("Transmitting " + Ds.Tables[0].Rows.Count + " exception records");
                    for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
                    {
                        com.sonraisystems.phoenix.clsException ExcRec = new SimpleConsoleApplication.com.sonraisystems.phoenix.clsException();
                        ExcRec.ExceptionID = Convert.ToInt32(Ds.Tables[0].Rows[i]["id"]);
                        ExcRec.timestamp = Convert.ToDateTime(Ds.Tables[0].Rows[i]["timestamp"]);
                        ExcRec.programname = settings.programName.ToString();
                        ExcRec.truckid = settings.truckid.ToString();
                        Records.Add(ExcRec);
                    }
                    string str = ds.dsLogException(Records.ToArray(), "tooh_t", "SonraiSys1234");
                    //ds.dsLogExceptionCompleted += new SimpleConsoleApplication.com.sonraisystems.phoenix.dsLogExceptionCompletedEventHandler(ds_dsLogExceptionCompleted);
                    //ds.dsLogExceptionAsync(Records.ToArray(), "tooh_t", "SonraiSys1234");

                    //string str = ds.dsWriteTestData(Records.ToArray(), "tooh_t", "SonraiSys1234", settings.programName.ToString(), settings.truckid.ToString());

                    if (str != "100" && str != "101" && str != "102")
                    {
                        //Delete Exception Record
                        DA.DeleteExceptions(str);
                    }
                }
                else
                {
                    //Do Nothing
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception transmission failed: " + Ex.Message);
            }
        }

        private void transmit_observations()
        {
            try
            {
                if (DA.GetObservationCount() > 0)
                {
                    //Call webservice to transmit data
                    List<com.sonraisystems.phoenix.Observation> Observations = new List<SimpleConsoleApplication.com.sonraisystems.phoenix.Observation>();
                    DataSet Ds = new DataSet();
                    Ds = DA.dsGetAllObservations();
                    Console.WriteLine("Tranmitting " + Ds.Tables[0].Rows.Count + " observation records");
                    for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
                    {
                        com.sonraisystems.phoenix.Observation observation = new SimpleConsoleApplication.com.sonraisystems.phoenix.Observation();
                        observation.observationCode = Convert.ToInt32(Ds.Tables[0].Rows[i]["observation_code_id"]);
                        observation.timestamp = Ds.Tables[0].Rows[i]["timestamp"].ToString();
                        observation.rfidtag = Ds.Tables[0].Rows[i]["rfidtag"].ToString();
                        Observations.Add(observation);
                    }
                    string str = ds.WriteObservation(Observations.ToArray(), "tooh_t", "SonraiSys1234", settings.programName, settings.truckid);

                    //ds.WriteObservationCompleted += new SimpleConsoleApplication.com.sonraisystems.phoenix.WriteObservationCompletedEventHandler(ds_WriteObservationCompleted);
                    //ds.WriteObservationAsync(Observations.ToArray(), "tooh_t", "SonraiSys1234", settings.programName, settings.truckid);

                    if (str != "100" && str != "101" && str != "102")
                    {
                        //Delete Observation Record(s)
                        DA.DeleteObservations(str);
                    }
                }
                else
                {
                    //Do Nothing
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Observation transmission failed: " + Ex.Message);
            }
        }

        private void transmit_gps()
        {
            try
            {
                if (DA.GetGPSCount() > 0)
                {
                    //Call webservice to transmit data
                    //List<com.sonraisystems.phoenix.Observation> Observations = new List<SimpleConsoleApplication.com.sonraisystems.phoenix.Observation>();
                    List<com.sonraisystems.phoenix.clsGPS> gpscollection = new List<SimpleConsoleApplication.com.sonraisystems.phoenix.clsGPS>();
                    DataSet Ds = new DataSet();
                    //Ds = DA.dsGetAllObservations();
                    Ds = DA.dsGetAllGPS();

                    Console.WriteLine("Tranmitting " + Ds.Tables[0].Rows.Count + " GPS records");
                    for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
                    {
                        com.sonraisystems.phoenix.clsGPS gps = new SimpleConsoleApplication.com.sonraisystems.phoenix.clsGPS();
                        
                        gps.lat = Ds.Tables[0].Rows[i]["lat"].ToString();
                        gps.lon = Ds.Tables[0].Rows[i]["lon"].ToString();
                        gps.timestamp = Convert.ToDateTime(Ds.Tables[0].Rows[i]["timestamp"].ToString());
                        
                        gpscollection.Add(gps);
                        
                    }
                    string str = ds.WriteGPSCollection(gpscollection.ToArray(), "tooh_t", "SonraiSys1234", settings.programName, settings.truckid);
                    Console.WriteLine("Resp GPS: " + str);
                    //ds.WriteObservationCompleted += new SimpleConsoleApplication.com.sonraisystems.phoenix.WriteObservationCompletedEventHandler(ds_WriteObservationCompleted);
                    //ds.WriteObservationAsync(Observations.ToArray(), "tooh_t", "SonraiSys1234", settings.programName, settings.truckid);

                    if (str != "100" && str != "101" && str != "102")
                    {
                        //Delete GPS Record(s)
                        DA.DeleteGPS(str);
                    }
                }
                else
                {
                    //Do Nothing
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("GPS transmission failed: " + Ex.Message);
            }
        }

        public void transmit()
        {
            int TransTimer = settings.transmit_timer;

            //while (true)
            {
                maxCount = 10;
                username = "username";
                password = "password";
                //Thread.Sleep(TransTimer);
                
                try
                {
                    TransTimer = settings.transmit_timer;
                    maxCount = settings.transmit_maxCount;
                    username = settings.transmit_username;
                    password = settings.transmit_password;
                }
                catch (Exception ex)
                { 
                }

                //Console.WriteLine("Transmitting Pickups & Exceptions....");
                transmit_pickups();
                transmit_exceptions();

                //transmit_observations();
                //transmit_gps();
                //Console.WriteLine("Next data transmission attempt: " + ((TransTimer / 1000)).ToString() + " seconds");
            }
        }
    }
}

