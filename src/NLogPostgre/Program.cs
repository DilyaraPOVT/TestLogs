using NLog;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NLogPostgre
{
    class Program
    {
        static void Main(string[] args)
        {
            //NpgsqlConnection con = new NpgsqlConnection("Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = books; ");
            //NpgsqlCommand com = new NpgsqlCommand("select * from books order by \"name\"", con);
            //con.Open();
            //NpgsqlDataReader reader;
            //reader = com.ExecuteReader();
            //while (reader.Read())
            //{
            //    try
            //    {
            //        string result = reader.GetString(1);//Получаем значение из второго столбца! Первый это (0)!
            //        Console.WriteLine(result);
            //    }
            //    catch { }

            //}

            Console.Write("Справка по сохранению лога: 1 - в файл, 2 - на консоль, 3 - в базу данных \n");
            Console.Write("Введите цифру: ");
            int i = Convert.ToInt32(Console.ReadLine());
            switch (i)
            {
                case 1:
                    Log logger = new Log();
                    logger.Error(new IndexOutOfRangeException());
                    logger.ErrorUnique("До этого такой ошибки не было.", new IndexOutOfRangeException());
                    Dictionary<object, object> dict = new Dictionary<object, object>();
                    dict.Add(1, "Стажировка");
                    dict.Add(2, "Программирование");
                    logger.SystemInfo("SystemInfo", dict);
                    logger.Warning("Предупреждение");
                    logger.WarningUnique("Новое предупреждение!");
                    Console.WriteLine("Лог записан");
                    break;
                case 2:
                    Log logger1 = new Log(i);
                    logger1.Error1(new IndexOutOfRangeException());
                    logger1.ErrorUnique1("До этого такой ошибки не было.", new IndexOutOfRangeException());
                    //демонстрация дообавления нового типа логов
                    Dictionary<object, object> dict1 = new Dictionary<object, object>();
                    dict1.Add(1, "Стажировка");
                    dict1.Add(2, "Программирование");
                    logger1.SystemInfo1("SystemInfo", dict1);
                    logger1.Warning1("Предупреждение");
                    logger1.WarningUnique1("Новое предупреждение!");
                    Console.WriteLine("Лог записан");
                    break;
                case 3:
                    GlobalDiagnosticsContext.Set("configDir", "D:\\source\\Logger");
                    GlobalDiagnosticsContext.Set("connectionString", "User ID=postgres;Password=1234;Server=localhost;Port=5432;Database=books;");
                    var logger2 = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                    logger2.Warn("Предупреждение");
                    logger2.Error(new ArgumentException("Ошибка аргументов"));
                    logger2.Info("Информация");
                    logger2.Debug("Ошибка во время отладки");
                    Console.WriteLine("Лог записан");
                    Console.ReadKey();
                    break;

            }


            //con.Close();
            Console.ReadLine();
        }
    }
}
