using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace SimpleConsoleApplication
{
    class TagStorage
    {
        public static ArrayList EPCList = new ArrayList();
        public static ArrayList DataList = new ArrayList();

        public static bool AddToTagList(string epc,string data)
        {
            if (!EPCList.Contains(epc))
            {
                EPCList.Add(epc);
                DataList.Add(data);
                SaveTagToDB(data);
                return true;
            }
            else
            {
                EPCList.Add(epc);
                DataList.Add(data);
                SaveTagToDB(data);
                return false;
            }
        }

        private static void SaveTagToDB(string data)
        {
            FileStream fs = new FileStream("tagdb.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(data);
            sw.Flush();
            sw.Close();
            sw = null;
            fs.Close();
            fs = null;
        }
        
        public static void LoadTagsFromDB()
        {
            string s = null;
            string epc = null;
            int i = 0;

            EPCList = null;
            EPCList = new ArrayList();
            DataList = null;
            DataList = new ArrayList();

            if (File.Exists("tagdb.txt"))
            {
                FileStream fs = new FileStream("tagdb.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine().Trim();
                    i = s.IndexOf(",");
                    epc = s.Substring(0, i);
                    EPCList.Add(epc);
                    DataList.Add(s);
                }

                sr.Close();
                sr = null;
                fs.Close();
                fs = null;
            }
        }

        public static void RemoveRecord()
        {
            EPCList.RemoveAt(0);
            DataList.RemoveAt(0);
        }

        public static void UpdateDBFile()
        {
            string data = null;
            int i = 0;

            FileStream fs = new FileStream("tagdb.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            if (DataList.Count > 0)
            {
                data = DataList[i++].ToString();
                sw.WriteLine(data);
                sw.Flush();
            }

            sw.WriteLine(data);
            sw.Flush();

            sw.Flush();
            sw.Close();
            sw = null;
            fs.Close();
            fs = null;
        }
    }
}
