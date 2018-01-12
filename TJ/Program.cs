using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TJ
{
    class TJobject
    {
        public DateTime date;
        public int mks;
        public ulong durability;
        public string tjevent;
        public int level;

        //свойства
        public TJProperty property = new TJProperty();
    }

    
    class TJProperty
    {
        public string process;
        public string processName;
        public string clientID;
        public string applicationName;
        public string computerName;
        public string Interface;
        public string IName;
        public string Method;
        public int CallID;
        public string MName;
        public int Memory;
        public int MemoryPeak;
        public int InBytes;
        public int OutBytes;
        public int Protected;
        public string Txt;
        public string address;
        public string result;
        public string Usr;
    }

    class Program
    {
        static void Main(string[] args)
        {
            DateTime localDate = DateTime.Now;

            String path = @"D:\ТЖ\SDBL\1CV8C_1232";
            ReadTJ(path);

            DateTime localDateEnd = DateTime.Now;
            Console.WriteLine(localDateEnd - localDate);
        }
        
        /// <summary>
        /// Извлекает параметр из строки ТЖ.
        /// </summary>
        /// <param name="param">параметр по которому осуществляется поиск в строке ТЖ</param>
        /// <param name="source">строка ТЖ</param>
        static string GetParam(string param, string source)
        {
            int sublen = param.Length;
            var pattern = new Regex(param + "[^,\\r$]+");
            string tmp = pattern.Match(source).Value;
            if (tmp.Length > sublen) {tmp = tmp.Substring(sublen);}
            else {tmp = null;}
            return tmp;
        }

        static List<TJobject> ReadTJ(string path)
        {
            List<TJobject> TJList = new List<TJobject>();
            CultureInfo provider = CultureInfo.InvariantCulture;

            List<string> files = new List<string>(Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories));


            foreach (var f in files)
            {
                var patternfile = new Regex("[0-9]{8}.log");
                string datesfile = patternfile.Match(f).Value;
                datesfile = datesfile.Remove(8);

                //В файле ТЖ окончание строки CRLF после всех контекстов
                //в контекстах внутри записи используется переносы LF после которых становится символ начала строки
                //поэтому ищем начало записи по времени
                var pattern = new Regex("[0-9][0-9]:[0-9][0-9]\\.[0-9]{4}.+");
                var readText = pattern.Matches(File.ReadAllText(f));

                foreach (Match item in readText)
                {
                    TJobject T = new TJobject();
                    string tmp = "";


                    //дата и время начала выполнения
                    var subpattern = new Regex("[0-9][0-9]:[0-9][0-9]");
                    tmp = datesfile + subpattern.Match(item.Value).Value;
                    T.date = DateTime.ParseExact(tmp, "yyMMddHHmm:ss", provider);
                    tmp = "";   
                    
                    //микросекунды (десятитысячные для 8.2) начала выполнения
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+");
                    tmp = subpattern.Match(item.Value).Value;
                    tmp = tmp.Substring(6);
                    T.mks = Convert.ToInt32(tmp);
                    tmp = "";

                    //длительность операции (для 8.2 десятитысячные после . 4 знака )
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex("-[0-9]+");
                    tmp = subpattern.Match(tmp).Value;
                    tmp = tmp.Substring(1);
                    T.durability = Convert.ToUInt64(tmp);
                    tmp = "";

                    //имя события
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+,[^,\\r$]*");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex(",.+");
                    tmp = subpattern.Match(tmp).Value;
                    if (tmp.Length != 0) { tmp = tmp.Substring(1); }
                    T.tjevent = tmp;
                    tmp = "";

                    //уровень события в стеке
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+,[^,\\r$]*,[0-9]+");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex(",[0-9]+");
                    tmp = subpattern.Match(tmp).Value;
                    if (tmp.Length != 0) { tmp = tmp.Substring(1); } else { tmp = null; }
                    T.level = Convert.ToInt32(tmp);
                    tmp = "";

                    //поиск свойств
                    T.property.process = GetParam(@"process=", item.Value);
                    T.property.processName = GetParam(@"p:processName=", item.Value);
                    T.property.clientID = GetParam(@"t:clientID=", item.Value);
                    T.property.applicationName = GetParam(@"t:applicationName=", item.Value);
                    T.property.computerName = GetParam(@"t:computerName=", item.Value);
                    T.property.Interface = GetParam(@"Interface=", item.Value);
                    T.property.IName = GetParam(@"IName=", item.Value);
                    T.property.Method = GetParam(@"Method=", item.Value);
                    T.property.CallID = Convert.ToInt32(GetParam(@"CallID=", item.Value));
                    T.property.MName = GetParam(@"MName=", item.Value);
                    T.property.Memory = Convert.ToInt32(GetParam(@"Memory=", item.Value));
                    T.property.MemoryPeak = Convert.ToInt32(GetParam(@"MemoryPeak=", item.Value));
                    T.property.MemoryPeak = Convert.ToInt32(GetParam(@"MemoryPeak=", item.Value));
                    T.property.InBytes = Convert.ToInt32(GetParam(@"InBytes=", item.Value));
                    T.property.OutBytes = Convert.ToInt32(GetParam(@"OutBytes=", item.Value));
                    T.property.Protected = Convert.ToInt32(GetParam(@"Protected=", item.Value));
                    T.property.Txt = GetParam(@"Txt=", item.Value);
                    T.property.address = GetParam(@"address=", item.Value);
                    T.property.result = GetParam(@"result=", item.Value);
                    T.property.Usr = GetParam(@"Usr=", item.Value);

                    TJList.Add(T);
                }
            }
            return TJList;

        }
    }
}
