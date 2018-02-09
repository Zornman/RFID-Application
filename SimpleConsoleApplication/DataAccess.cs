using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;

namespace SimpleConsoleApplication
{
    class DataAccess
    {
        private string strDatabase = "URI=file:Database.db3";
        #region Class Get Functions
        public DataSet dsGetAllRecords()
        {
            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM pickups order by timestamp DESC";
                var reader = cmd.ExecuteReader();
                ds = DataReaderToDataSet(reader);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllRecords()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return ds;
        }
        public DataSet dsGetAllTransmitted()
        {

            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM pickups where transmitted = 1 order by timestamp DESC";
                var reader = cmd.ExecuteReader();
                ds = DataReaderToDataSet(reader);
                //Console.WriteLine("Count: " + ds.Tables[0].Rows.Count);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllTtransmitted()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }

            return ds;
        }
        public DataSet dsGetAllNotTransmitted()
        {
            
            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);         
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM pickups where transmitted = 0 order by timestamp DESC";
                var reader = cmd.ExecuteReader();                
                ds = DataReaderToDataSet(reader);
                ds.Tables[0].Rows.RemoveAt(0);
                //Console.WriteLine("Count: " + ds.Tables[0].Rows.Count);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllNotTransmitted()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return ds;
        }

        public DataSet dsGetAllExceptions()
        {

            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);         
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM exceptions";
                var reader = cmd.ExecuteReader();
                ds = DataReaderToDataSet(reader);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllExceptions()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return ds;
        }

        public DataSet dsGetAllGPS()
        {

            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM gps";
                var reader = cmd.ExecuteReader();
                ds = DataReaderToDataSet(reader);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllGPS()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return ds;
        }

        public DataSet dsGetAllObservations()
        {

            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);         
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM observations";
                var reader = cmd.ExecuteReader();
                ds = DataReaderToDataSet(reader);
                //Console.WriteLine("Count: " + ds.Tables[0].Rows.Count);
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllNotTransmittedObservations()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return ds;
        }

        public int GetAllRecordCount()
        {
            int retVal = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM pickups";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count Record exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }

        public int GetNotTransmittedCount()        
        {
            int retVal=0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM pickups WHERE transmitted = 0";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count Record exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }

        public int GetNotTransmittedObservationCount()
        {
            int retVal = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM observations WHERE transmitted = 0";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count Record exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }

        public int GetExceptionCount()
        {
            int retVal = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM exceptions";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count Record exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }

        public int GetGPSCount()
        {
            int retVal = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM GPS";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count GPS exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }

        public int GetObservationCount()
        {
            int retVal = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(timestamp) FROM observations";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count Observation exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {
                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
            return retVal;
        }
        #endregion

        #region Insert Functions
        public void InsertPickupRecord(string rfidtag, string programname, string truckid, string timestamp, string lat, string lon)
        {
            Console.WriteLine("InsertPickupRecord: " + timestamp);
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
               //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO pickups (rfidtag, programname, truckid, timestamp, lat, lon) VALUES('" + rfidtag + "','" + programname + "','" + truckid + "','" + timestamp + "','" + lat + "','" + lon + "')";
                    //Console.WriteLine(cmd.CommandText);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Console.WriteLine(cmd.ExecuteNonQuery());
                    conn.Close();
                    Console.WriteLine("Record Inserted:" + programname + "-" + truckid + " " + timestamp + " " + rfidtag + " " + lat + "," + lon);
                    Console.WriteLine("Current Total Count: " + GetAllRecordCount() + ", Current Record Count: " + GetNotTransmittedCount());
                }

                // Sends email if the lat and long is coming in at 0,0
                // Tested on 11/24 @ 11:30am, works perfect. DO NOT MODIFY
                if (settings.sent_NoGPSNotification == "0")
                {
                    if (lat == "0" && lon == "0")
                    {
                        string message = "The lat and long is coming in at 0,0. Take the following steps of action:\n\n" +
                            "- Check the portal to make sure this isn't happening with all the reads\n" +
                            "- Check the modem & verify it has a GPS fix\n" +
                            "- If the modem has a fix, check the reader and see if it's getting GPS from the modem\n" +
                            "     > If not, then reach out to the customer and have the local tech check the 9-pin serial cable\n\n" +
                            "**The 9-pin serial cable from the modem to the reader could need to be replaced**";
                        Program.myAppsThread.SendEmailNotification(message);

                        settings.sent_NoGPSNotification = "1";
                    }
                }

                if (settings.sent_DateTimeOffNotification == "0")
                {
                    DateTime timeStampOfReads = DateTime.Parse(timestamp);
                    DateTime currentDateTime = Program.myAppsThread.GetNistTime();

                    if (timeStampOfReads.Year < currentDateTime.Year)
                    {
                        string message = "The timestamp is off on the reads, currently showing: " + timestamp + ", take the following steps of action:\n\n" +
                            "- Get into the reader and update the time to: " + DateTime.Now.ToString();
                        Program.myAppsThread.SendEmailNotification(message);

                        settings.sent_DateTimeOffNotification = "1";
                    }
                    else if (timeStampOfReads.Month < currentDateTime.Month)
                    {
                        string message = "The timestamp is off on the reads, currently showing: " + timestamp + ", take the following steps of action:\n\n" +
                            "- Get into the reader and update the time to: " + DateTime.Now.ToString();
                        Program.myAppsThread.SendEmailNotification(message);

                        settings.sent_DateTimeOffNotification = "1";
                    }
                    else if (timeStampOfReads.Day < currentDateTime.Day)
                    {
                        string message = "The timestamp is off on the reads, currently showing: " + timestamp + ", take the following steps of action:\n\n" +
                            "- Get into the reader and update the time to: " + DateTime.Now.ToString();
                        Program.myAppsThread.SendEmailNotification(message);

                        settings.sent_DateTimeOffNotification = "1";
                    }
                }
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    Console.WriteLine("Corrupt DB!");

                    if (settings.sent_CorruptDBNotification == "0")
                    {
                        try
                        {
                            string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                            Program.myAppsThread.SendEmailNotification(message, strDatabase);

                            settings.sent_CorruptDBNotification = "1";
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine("Error sending email with DB file: " + error.Message);
                        }
                    }
                    ReplaceDB();
                }
                Console.WriteLine("Failed to inserted record:" + rfidtag + "-" + truckid + " " + timestamp + " " + programname + " " + lat + "," + lon);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Query Exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" +
                            "1. Open Command Prompt and browse to the DB Backup folder on your local machine" +
                            "2. FTP to the IP address\n" +
                            "3. Type intermec for the username and password\n" +
                            "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" +
                            "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" +
                            "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" +
                            "7. The app should start transmitting reads again\n" +
                            "8. Take the db3 file you pulled off the reader and upload it to the portal\n" +
                            "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
                Console.WriteLine("Failed to inserted record:" + rfidtag + "-" + truckid + " " + timestamp + " " + programname + " " + lat + "," + lon);
            }
        }

        public void InsertGPSRecord(string programname, string truckid, string timestamp, string lat, string lon)
        {
            //Console.WriteLine("InsertPickupRecord");
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO gps (programname, truckid, timestamp, lat, lon) VALUES('" + programname + "','" + truckid + "','" + timestamp + "','" + lat + "','" + lon + "')";
                    //Console.WriteLine(cmd.CommandText);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Console.WriteLine(cmd.ExecuteNonQuery());
                    conn.Close();
                    Console.WriteLine("GPS Record Inserted:" + truckid + " " + timestamp + " " + lat + "," + lon);
                    //Console.WriteLine("Current Record Count: " + GetAllRecordCount());
                }
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Query Exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void InsertExceptionRecord(int ExcptnCode, string timestamp)
        {
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO exceptions (id,timestamp) VALUES('" + ExcptnCode + "','" + timestamp + "')";
                    //Console.WriteLine(cmd.CommandText);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Console.WriteLine(cmd.ExecuteNonQuery());
                    conn.Close();
                    Console.WriteLine("Exception recorded: " + ExcptnCode.ToString() + " " + timestamp.ToString());
                    Console.WriteLine("Current Exception Count: " + GetExceptionCount());
                }
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Query Exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void InsertObservationRecord(int ObservationCodeID, string timestamp, string rfidtag)
        {
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO observations (timestamp, observation_code_id, rfidtag) VALUES('" + timestamp + "','" + ObservationCodeID + "','" + rfidtag + "')";
                    //Console.WriteLine(cmd.CommandText);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Console.WriteLine(cmd.ExecuteNonQuery());
                    conn.Close();
                    Console.WriteLine("Observation recorded: " + ObservationCodeID.ToString() + " " + timestamp.ToString() + " " + rfidtag);
                    Console.WriteLine("Current Observation Count: " + GetObservationCount());
                }
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Query Exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void InsertObservationRecord_New(int ObservationCodeID, string timestamp, string rfidtag)
        {
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                using (var cmd = conn.CreateCommand())
                {
                    if (ObservationCodeID == 10001)
                    {
                        cmd.CommandText = "UPDATE pickups SET observation1 = '1' WHERE rfidtag='" + rfidtag + "' AND DATE(timestamp)='" + timestamp.Split(' ')[0].ToString() + "'";
                    }
                    else if (ObservationCodeID == 10002)
                    {
                        cmd.CommandText = "UPDATE pickups SET observation2 = '1' WHERE rfidtag='" + rfidtag + "' AND DATE(timestamp)='" + timestamp.Split(' ')[0].ToString() + "'";
                    }
                    else if (ObservationCodeID == 10003)
                    {
                        cmd.CommandText = "UPDATE pickups SET observation3 = '1' WHERE rfidtag='" + rfidtag + "' AND DATE(timestamp)='" + timestamp.Split(' ')[0].ToString() + "'";
                    }                
                    Console.WriteLine(cmd.CommandText);
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    Console.WriteLine(cmd.ExecuteNonQuery());
                    conn.Close();
                    Console.WriteLine("Observation recorded: " + ObservationCodeID.ToString() + " " + timestamp.ToString() + " " + rfidtag);
                    Console.WriteLine("Current Observation Count: " + GetObservationCount());
                }
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert Query Exception: " + ex.Message.ToString());
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }
        #endregion

        #region Delete Functions
        public void DeleteTransnmitted(string timestamp)
        {
            int Cnt = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM pickups WHERE transmitted = 1 AND timestamp <= datetime('" + Convert.ToDateTime(timestamp).ToString("yyyy-MM-dd HH:mm:ss") + "','-1 day')";
                //Console.WriteLine(cmd.CommandText);
                Cnt = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Deleted transmitted pickup records: " + Cnt);
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (DeleteTransmitted()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void DeleteGPS(string timestamp)
        {
            int Cnt = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM gps WHERE timestamp <= datetime('" + Convert.ToDateTime(timestamp).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                //Console.WriteLine(cmd.CommandText);
                Cnt = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Deleted GPS records: " + Cnt);
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (DeleteGPS()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void DeleteExceptions(string timestamp)
        {
            int Cnt = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM exceptions WHERE timestamp <= datetime('" + Convert.ToDateTime(timestamp).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                //Console.WriteLine(cmd.CommandText);
                Cnt = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Deleted exception records: " + Cnt);
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (DeleteExceptions()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }

        public void DeleteObservations(string timestamp)
        {
            int Cnt = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM observations WHERE timestamp <= datetime('" + Convert.ToDateTime(timestamp).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                //Console.WriteLine(cmd.CommandText);
                Cnt = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Deleted Observations records: " + Cnt);
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (DeleteObservations()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        } 
        #endregion

        #region Update Functions
        public void UpdateTransmitted(string timestmap)
        {
            DataSet ds = new DataSet();
            int Cnt = 0;
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE pickups SET transmitted = 1 WHERE transmitted = 0 AND timestamp <='" + timestmap + "'";
                //Console.WriteLine(cmd.CommandText);
                Cnt = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Transmitted records updated: " + Cnt);
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (UpdateTransmitted()): " + ex.Message);
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }
        }    
        #endregion

        #region Misc Functions
        public string dsGetAddressByRFID(string rfidtag)
        {
            string str = "";
            try
            {
                com.sonraisystems.phoenix.SonraiDataService ds = new com.sonraisystems.phoenix.SonraiDataService();
                str = ds.dsGetAddressByRFID(rfidtag, "tooh_t", "SonraiSys1234");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message + " " + ex.StackTrace);
            }
            return str;
        }

        private DataSet DataReaderToDataSet(IDataReader reader)
        {
            DataSet ds = new DataSet();

            try
            {
                DataTable table = new DataTable();
                int fieldCount = reader.FieldCount;
                Console.WriteLine("fieldCount: " + fieldCount);
                for (int i = 0; i < fieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                }
                table.BeginLoadData();
                #region commneted code
                //for (int i = 0; i < reader.; i++)
                //{
                //    reader.Read();
                //    Console.WriteLine(reader.GetString(0).ToString());
                //}
                //while (reader.Read())
                //{
                //    Console.WriteLine(reader[0] + "   " + reader[1]);
                //}
                //try
                //{
                //    table.Load(reader, LoadOption.OverwriteChanges);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Error loading datareader into dataset");
                //}
                #endregion
                object[] values = new object[fieldCount];
                while (reader.Read())
                {
                    //Console.WriteLine("Values");
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();

                ds.Tables.Add(table);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Convert To: " + ex.Message);
            }
            return ds;
        }

        public bool CheckDuplicate(string rfidtag)
        {
            bool retVal = false;
            DataSet ds = new DataSet();
            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                //SqliteConnection conn = new SqliteConnection("URI=file:/tmp/./Database.db3");
                ///home/developer/edgeware/userapp9/./Database.db3
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM pickups WHERE rfidtag ='" + rfidtag + "' AND DATE(timestamp)='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                var reader = cmd.ExecuteReader();
                //ds = DataReaderToDataSet(reader);
                //Console.WriteLine(reader.HasRows);
                //Console.WriteLine(cmd.CommandText);
                if (reader.HasRows)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception (dsGetAllRecords()): " + ex.Message);
            }
            return retVal;
        }

        private void ReplaceDB()
        {
            Console.WriteLine("Replacing DB file, sending email with corrupt one to service@sonraisystems.com...");
            try
            {
                System.IO.File.Copy("./backup/Database.db3", "../Database.db3", true);
                InsertExceptionRecord(9010, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Failed to copy file: " + ex1.Message.ToString());
            }
        }

        //Overloaded Method
        private void ReplaceDB(int db_Index)
        {
            if (db_Index == 0)
            {
                Console.WriteLine("Replacing DB file, sending email with corrupt one to service@sonraisystems.com...");
                try
                {
                    System.IO.File.Copy("./backup/Database.db3", "../Database.db3", true);
                    InsertExceptionRecord(9010, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Failed to copy file: " + ex1.Message.ToString());
                }
            }
            else if (db_Index == 1)
            {
                Console.WriteLine("Replacing DB file, sending email with corrupt one to service@sonraisystems.com...");
                try
                {
                    System.IO.File.Copy("./backup/GPSDB.db3", "GPSDB.db3", true);
                    InsertExceptionRecord(9011, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Failed to copy file: " + ex1.Message.ToString());
                }
            }            
        }

        public int CountTagsReadByTime(string ts, string armTag)
        {
            int retVal = 0;

            try
            {
                SqliteConnection conn = new SqliteConnection(strDatabase);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(rfidtag) FROM pickups WHERE timestamp >= '" + ts + "' AND rfidtag <> '" + armTag + "'";
                Console.WriteLine(cmd.CommandText.ToString());
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retVal = reader.GetInt32(0);
                }
                conn.Close();
            }
            catch (SqliteException sqlEx)
            {
                if (sqlEx.ErrorCode == SqliteErrorCode.Corrupt)
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is corrupt, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    Console.WriteLine("Corrupt DB!");
                    ReplaceDB();
                }
            }
            catch (Exception ex)
            {                
                if (ex.Message.Contains("malformed"))
                {
                    if (settings.sent_CorruptDBNotification == "0")
                    {


                        string message = "The database file is malformed, upload to portal ASAP. Follow these steps:\n\n" + "1. Open Command Prompt and browse to the DB Backup folder on your local machine" + "2. FTP to the IP address\n" + "3. Type intermec for the username and password\n" + "4. Change Directory to edgeware/userapp0 (Or whatever number the userapp is, sometimes 1,2,3 etc.)\n" + "5. Type get Database.db3 ProgramCodeTruckNumber.db3 (ex. PAFL271532.db3)\n" + "6. Type put 1-27/Database.db3 (Or just Database.db3, don't need to seperate them out based on version. They are all the same)\n" + "7. The app should start transmitting reads again\n" + "8. Take the db3 file you pulled off the reader and upload it to the portal\n" + "9. DONE!";
                        Program.myAppsThread.SendEmailNotification(message, strDatabase);

                        settings.sent_CorruptDBNotification = "1";
                    }
                    ReplaceDB();
                }
            }

            return retVal;
        }
        #endregion
    }  
}
