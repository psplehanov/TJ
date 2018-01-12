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
        public int durability;
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            DateTime localDate = DateTime.Now;

            String path = @"D:\ТЖ\наибольший объем памяти";
            ReadTJ(path);

            DateTime localDateEnd = DateTime.Now;
            Console.WriteLine(localDateEnd - localDate);
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
                var pattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]{6}.+");
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

                    //длительность операции
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex("-[0-9]+");
                    tmp = subpattern.Match(tmp).Value;
                    tmp = tmp.Substring(1);
                    T.durability = Convert.ToInt32(tmp);
                    tmp = "";

                    //имя события
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]{6}-[0-9]+,\\w+,");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex(",.+,");
                    tmp = subpattern.Match(tmp).Value;
                    tmp = tmp.Substring(1, tmp.Length - 2);
                    T.tjevent = tmp;
                    tmp = "";

                    //уровень события в стеке
                    subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]{6}-[0-9]+,\\w+,[0-9]+");
                    tmp = subpattern.Match(item.Value).Value;
                    subpattern = new Regex(",[0-9]+");
                    tmp = subpattern.Match(tmp).Value;
                    tmp = tmp.Substring(1);
                    T.level = Convert.ToInt32(tmp);
                    tmp = "";

                    //поиск свойств
                    subpattern = new Regex(",process=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(9); //wtf не работает tmp = tmp.Substring(9, tmp.Length - 2); 
                        tmp = tmp.Remove(tmp.Length - 1);                        
                        
                        T.property.process = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("p:processName=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(14); 
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.processName = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("t:clientID=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(11);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.clientID = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("t:applicationName=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(18);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.applicationName = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("t:computerName=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(15);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.computerName = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("Interface=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(10);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.Interface = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("IName=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(6);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.IName = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("Method=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(7);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.Method = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("CallID=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(7);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.CallID = Convert.ToInt32(tmp);
                    }
                    tmp = "";

                    subpattern = new Regex("MName=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(6);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.MName = tmp;
                    }
                    tmp = "";

                    subpattern = new Regex("Memory=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(7);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.Memory = Convert.ToInt32(tmp);
                    }
                    tmp = "";

                    subpattern = new Regex("MemoryPeak=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(11);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.MemoryPeak = Convert.ToInt32(tmp);
                    }
                    tmp = "";

                    subpattern = new Regex("InBytes=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(8);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.InBytes = Convert.ToInt32(tmp);
                    }
                    tmp = "";

                    subpattern = new Regex("OutBytes=\\w*,");
                    tmp = subpattern.Match(item.Value).Value;
                    if (tmp != "")
                    {
                        tmp = tmp.Substring(9);
                        tmp = tmp.Remove(tmp.Length - 1);

                        T.property.OutBytes = Convert.ToInt32(tmp);
                    }
                    tmp = "";

                    TJList.Add(T);
                }
            }
            return TJList;

        }
    }
}
