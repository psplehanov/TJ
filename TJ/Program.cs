using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;


namespace TJA
{
   
    class Program
    {
        

        static void Main(string[] args)
        {
#if (DEBUG)
            DateTime localDate = DateTime.Now;
#endif

            String path = @"G:\1c_logs\locks\rphost_10040";

            DebugAllRead(path);
            //DebugCountRead(path);


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
    }
}
