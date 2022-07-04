using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Npgsql;

namespace NLogPostgre
{
    public class Log : ILog
    {
        const double CheckInterval = 1000 * 60 * 30; // [мс]*[с]*[мин] Каждые 30 минут проверка
        private int nowDay;
        private Dictionary<string, string> DayErrorCollection = new Dictionary<string, string>();
        private Dictionary<string, string> DayWarningCollection = new Dictionary<string, string>();
        private string LogFilePath;
        private readonly string ApplicationPath = Directory.GetCurrentDirectory();
        public Log()
        {
            this.LogFilePath = ApplicationPath + "\\logs\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
            Init();
            nowDay = DateTime.Now.Day;
            System.Timers.Timer inspector = new System.Timers.Timer(CheckInterval);
            inspector.Elapsed += new ElapsedEventHandler(Checking);
            inspector.Start();
        }
        public Log(int i)
        {
            Init1();
            nowDay = DateTime.Now.Day;
            System.Timers.Timer inspector = new System.Timers.Timer(CheckInterval);
            inspector.Elapsed += new ElapsedEventHandler(Checking1);
            inspector.Start();
        }

        private void Checking(object sender, ElapsedEventArgs e)
        {
            if (nowDay != DateTime.Now.Day)
            {
                Init();
                nowDay = DateTime.Now.Day;
            }
        }
        private void Checking1(object sender, ElapsedEventArgs e)
        {
            if (nowDay != DateTime.Now.Day)
            {
                Init1();
                nowDay = DateTime.Now.Day;
            }
        }
        private void Init()
        {
            try
            {
                //очистка списков
                DayErrorCollection.Clear();
                DayWarningCollection.Clear();

                if (!File.Exists(LogFilePath))
                {
                    if (!Directory.Exists(ApplicationPath + "\\logs"))
                        Directory.CreateDirectory(ApplicationPath + "\\logs");
                    using (File.Create(LogFilePath)) ;
                    StreamWriter sw = new StreamWriter(LogFilePath);
                    sw.WriteLine(DateTime.Now + " # Начало работы логгера");
                    sw.Close();
                }
                else
                {
                    using (StreamReader sr = new StreamReader(LogFilePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            string str = sr.ReadLine();
                            string[] arr = str.Split('-');
                            if (arr[1] == "ERROR")
                                DayErrorCollection.Add(arr[0], arr[2]);
                            if (arr[1] == "WARNING")
                                DayWarningCollection.Add(arr[0], arr[2]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex);
            }
        }
        private void Init1()
        {
            try
            {
                //очистка списков
                DayErrorCollection.Clear();
                DayWarningCollection.Clear();
                Console.WriteLine(DateTime.Now + " # Начало работы логгера");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex);
            }
        }
        private void SendToFile(string message, string e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(LogFilePath, true))
                {
                    sw.WriteLine("{0} # {1} # {2}", DateTime.Now, message, e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        private void SendToConsole(string message, string e)
        {
            try
            {
                Console.WriteLine("{0} # {1} # {2}", DateTime.Now, message, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }

        //Реализация методов интерфейса ILog
        public void Error(string message)
        {
            try
            {

                SendToFile("ERROR", message);
                DayErrorCollection.Add(message, "ERROR");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Error(string message, Exception e)
        {
            try
            {
                SendToFile("ERROR", message + " " + e.Message);
                DayErrorCollection.Add(message, e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Error(Exception e)
        {
            try
            {
                SendToFile("ERROR", e.Message);
                DayErrorCollection.Add(e.Message, "ERROR");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void ErrorUnique(string message, Exception e)
        {
            try
            {
                string value;
                if (!DayErrorCollection.TryGetValue(message + " " + e.Message, out value))
                {
                    SendToFile("ERROR", message + " " + e.Message);
                    DayErrorCollection.Add(message + " " + e.Message, "ERROR");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Error1(string message)
        {
            try
            {

                SendToConsole("ERROR", message);
                DayErrorCollection.Add(message, "ERROR");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Error1(string message, Exception e)
        {
            try
            {
                SendToConsole("ERROR", message + " " + e.Message);
                DayErrorCollection.Add(message, e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Error1(Exception e)
        {
            try
            {
                SendToConsole("ERROR", e.Message);
                DayErrorCollection.Add(e.Message, "ERROR");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void ErrorUnique1(string message, Exception e)
        {
            try
            {
                string value;
                if (!DayErrorCollection.TryGetValue(message + " " + e.Message, out value))
                {
                    SendToConsole("ERROR", message + " " + e.Message);
                    DayErrorCollection.Add(message + " " + e.Message, "ERROR");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Warning(string message)
        {
            try
            {
                SendToFile("WARNING", message);
                DayWarningCollection.Add(message, "WARNING");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Warning(string message, Exception e)
        {
            try
            {
                SendToFile("WARNING", message + " " + e.Message);
                DayWarningCollection.Add(message, e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void WarningUnique(string message)
        {
            try
            {
                string value;
                if (!DayWarningCollection.TryGetValue(message, out value))
                {
                    SendToFile("WARNING", message);
                    DayWarningCollection.Add(message, "WARNING");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Warning1(string message)
        {
            try
            {
                SendToConsole("WARNING", message);
                DayWarningCollection.Add(message, "WARNING");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Warning1(string message, Exception e)
        {
            try
            {
                SendToConsole("WARNING", message + " " + e.Message);
                DayWarningCollection.Add(message, e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void WarningUnique1(string message)
        {
            try
            {
                string value;
                if (!DayWarningCollection.TryGetValue(message, out value))
                {
                    SendToConsole("WARNING", message);
                    DayWarningCollection.Add(message, "WARNING");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void Info(string message)
        {
            SendToFile("INFO", message);
        }
        public void Info(string message, Exception e)
        {
            SendToFile("INFO", message + " " + e.Message);
        }
        public void Info(string message, params object[] args)
        {
            SendToFile("INFO", message + " " + args);
        }
        public void Info1(string message)
        {
            SendToConsole("INFO", message);
        }
        public void Info1(string message, Exception e)
        {
            SendToConsole("INFO", message + " " + e.Message);
        }
        public void Info1(string message, params object[] args)
        {
            SendToConsole("INFO", message + " " + args);
        }
        public void Debug(string message)
        {
            SendToFile("DEBUG", message);
        }
        public void Debug(string message, Exception e)
        {
            SendToFile("DEBUG", message + " " + e.Message);
        }
        public void DebugFormat(string message, params object[] args)
        {
            SendToFile("DEBUG", message + " " + args);
        }
        public void Debug1(string message)
        {
            SendToConsole("DEBUG", message);
        }
        public void Debug1(string message, Exception e)
        {
            SendToConsole("DEBUG", message + " " + e.Message);
        }
        public void DebugFormat1(string message, params object[] args)
        {
            SendToConsole("DEBUG", message + " " + args);
        }
        public void SystemInfo(string message, Dictionary<object, object> properties = null)
        {
            try
            {
                if (properties != null)
                    foreach (var x in properties)
                        SendToFile("SYSTEMINFO", message + " " + x.Key + " " + x.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
        public void SystemInfo1(string message, Dictionary<object, object> properties = null)
        {
            try
            {
                if (properties != null)
                    foreach (var x in properties)
                        SendToConsole("SYSTEMINFO", message + " " + x.Key + " " + x.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! Message: " + ex.Message);
            }
        }
    }
}
