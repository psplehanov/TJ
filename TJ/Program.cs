using System;
using System.Collections.Generic;
using System.Timers;


namespace TJA
{
   
    class Program
    {

        private static Timer aTimer;
        private static List<TJobject> TJList;
        private static TJCoord coord;
        private static TJCoordFin coordFin;
        private static string pathTimer;

        static void Main(string[] args)
        {
#if (DEBUG)
            DateTime localDate = DateTime.Now;
#endif

            String path = @"E:\test\";

            //DebugAllRead(path);
            //DebugCountRead(path);
            DebugCountReadWithChange(@"E:\1c\ТЖ\ALL");


        }

        static void DebugAllRead(string path)
        {
            DateTime localDate = DateTime.Now;

            List<TJobject> TJList = new List<TJobject>();
            int count = 0;

            
            TJ.ReadTJ(path, ref TJList);
            count = TJList.Count;

            string debstr = "";
            foreach (TJobject TJ in TJList)
            {
                debstr = debstr + TJ.date.ToString() + "." + TJ.mks + Environment.NewLine;
            }

            for (int i=0; i<TJList.Count;i++)
            {
                if (TJList[i].tjevent == @"ADMIN")
                {
                    Console.WriteLine("beep");
                }
            }

            DateTime localDateEnd = DateTime.Now;
            Console.WriteLine("Count = " + count);
            Console.WriteLine(localDateEnd - localDate);
            Console.ReadKey();
        }

        static void DebugCountRead(string path)
        {

            DateTime localDate = DateTime.Now;

            List<TJobject> TJList = new List<TJobject>();
            int count = 0;

            TJCoord coord = new TJCoord();
            while (TJ.ReadTJ(path, ref TJList, 1000, ref coord))
            {
                count = TJList.Count + count;
                TJList.Clear();
                Console.WriteLine(coord.filename);
            }

            count = TJList.Count + count;

            DateTime localDateEnd = DateTime.Now;
            Console.WriteLine("Count = " + count);
            Console.WriteLine(localDateEnd - localDate);
            Console.ReadKey();
        }

        static void DebugCountReadWithChange(string path)
        {

            DateTime localDate = DateTime.Now;

            TJList = new List<TJobject>();
            coord = new TJCoord();
            coordFin = new TJCoordFin();
            pathTimer = path;

            
            aTimer = new Timer(5000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

           

            DateTime localDateEnd = DateTime.Now;

            Console.WriteLine(localDateEnd - localDate);
            Console.ReadKey();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int count = 0;

            //приостанавливаем таймер на время чтения
            aTimer.Enabled = false;
            while (TJ.ReadTJ(pathTimer, ref TJList, 1000, ref coord, ref coordFin))
            {
                count = TJList.Count + count;
                TJList.Clear();
                Console.WriteLine(coord.filename);
            }
            aTimer.Enabled = true;

            Console.WriteLine("timer");
            Console.WriteLine("Count = " + count);
        }

    }
}
