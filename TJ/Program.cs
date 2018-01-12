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
        public long durability;
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

        public int OSThread;
        public int connectID;
        public bool Trans;
        public string Sdbl;
        public int Rows;
        public string Func;
        public string Context;
    }

    class Program
    {
        public const int buflen = 16384;

        static void Main(string[] args)
        {
#if (DEBUG)
            DateTime localDate = DateTime.Now;
#endif

            String path = @"G:\1c_logs";
            ReadTJ(path);

#if (DEBUG)
            DateTime localDateEnd = DateTime.Now;
            Console.WriteLine(localDateEnd - localDate);
            Console.ReadKey();
#endif
        }

        /// <summary>
        /// Извлекает параметр из строки ТЖ.
        /// </summary>
        /// <param name="param">параметр по которому осуществляется поиск в строке ТЖ</param>
        /// <param name="source">строка ТЖ</param>
        static string GetParam(string param, string source)
        {
            int sublen = param.Length;
            var pattern = new Regex(param + "[^,]*");
            string tmp = pattern.Match(source).Value;
            if (tmp.Length > sublen) {tmp = tmp.Substring(sublen);}
            else {tmp = null;}
            return tmp;
        }

        /// <summary>
        /// Конвертирование строки ТЖ в объект TJObject
        /// </summary>
        /// <param name="strTJ">Строка ТЖ</param>
        /// <param name="filename">Имя файла без .log</param>
        /// <returns></returns>
        static TJobject ParseStringTJ(string strTJ, string filename)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            TJobject T = new TJobject();

            string tmp = "";

            //дата и время начала выполнения
            var subpattern = new Regex("[0-9][0-9]:[0-9][0-9]");
            tmp = filename + subpattern.Match(strTJ).Value;
            T.date = DateTime.ParseExact(tmp, "yyMMddHHmm:ss", provider);
            tmp = "";

            //микросекунды (десятитысячные для 8.2) начала выполнения
            subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+");
            tmp = subpattern.Match(strTJ).Value;
            tmp = tmp.Substring(6);
            T.mks = Convert.ToInt32(tmp);
            tmp = "";

            //длительность операции (для 8.2 десятитысячные после . 4 знака )
            subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+");
            tmp = subpattern.Match(strTJ).Value;
            subpattern = new Regex("-[0-9]+");
            tmp = subpattern.Match(tmp).Value;
            tmp = tmp.Substring(1);
            T.durability = Convert.ToInt64(tmp);
            tmp = "";

            //имя события
            subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+,[^,\\r$]*");
            tmp = subpattern.Match(strTJ).Value;
            subpattern = new Regex(",.+");
            tmp = subpattern.Match(tmp).Value;
            if (tmp.Length != 0) { tmp = tmp.Substring(1); }
            T.tjevent = tmp;
            tmp = "";

            //уровень события в стеке
            subpattern = new Regex("[0-9][0-9]:[0-9][0-9].[0-9]+-[0-9]+,[^,\\r$]*,[0-9]+");
            tmp = subpattern.Match(strTJ).Value;
            subpattern = new Regex(",[0-9]+");
            tmp = subpattern.Match(tmp).Value;
            if (tmp.Length != 0) { tmp = tmp.Substring(1); } else { tmp = null; }
            T.level = Convert.ToInt32(tmp);
            tmp = "";

            //поиск свойств
            T.property.process = GetParam(@"process=", strTJ);
            T.property.processName = GetParam(@"p:processName=", strTJ);
            T.property.clientID = GetParam(@"t:clientID=", strTJ);
            T.property.applicationName = GetParam(@"t:applicationName=", strTJ);
            T.property.computerName = GetParam(@"t:computerName=", strTJ);
            T.property.Interface = GetParam(@"Interface=", strTJ);
            T.property.IName = GetParam(@"IName=", strTJ);
            T.property.Method = GetParam(@"Method=", strTJ);
            T.property.CallID = Convert.ToInt32(GetParam(@"CallID=", strTJ));
            T.property.MName = GetParam(@"MName=", strTJ);
            T.property.Memory = Convert.ToInt32(GetParam(@"Memory=", strTJ));
            T.property.MemoryPeak = Convert.ToInt32(GetParam(@"MemoryPeak=", strTJ));
            T.property.MemoryPeak = Convert.ToInt32(GetParam(@"MemoryPeak=", strTJ));
            T.property.InBytes = Convert.ToInt32(GetParam(@"InBytes=", strTJ));
            T.property.OutBytes = Convert.ToInt32(GetParam(@"OutBytes=", strTJ));
            T.property.Protected = Convert.ToInt32(GetParam(@"Protected=", strTJ));
            T.property.Txt = GetParam(@"Txt=", strTJ);
            T.property.address = GetParam(@"address=", strTJ);
            T.property.result = GetParam(@"result=", strTJ);
            T.property.Usr = GetParam(@"Usr=", strTJ);
            T.property.OSThread = Convert.ToInt32(GetParam(@"OSThread=", strTJ));
            T.property.connectID = Convert.ToInt32(GetParam(@"t:connectID=", strTJ));
            if (GetParam(@"Trans=", strTJ) == "0") { T.property.Trans = false; } else { T.property.Trans = true; }
            T.property.Sdbl = GetParam(@"Sdbl=", strTJ);
            T.property.Rows = Convert.ToInt32(GetParam(@"Rows=", strTJ));
            T.property.Func = GetParam(@"Func=", strTJ);
            T.property.Context = GetParam(@"Context=", strTJ);

            return T;
        }

        static List<TJobject> ReadTJ(string path)
        {
            List<TJobject> TJList = new List<TJobject>();
            char[] buffer = new char[buflen];

            List<string> files = new List<string>(Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories));

            foreach (var f in files)
            {
                var patternfile = new Regex("[0-9]{8}.log");
                string datesfile = patternfile.Match(f).Value;
                datesfile = datesfile.Remove(8);

                StreamReader objReader = new StreamReader(f);

                int len = 0;
                string rollstr = "";
                while ((len = objReader.ReadBlock(buffer, 0, buflen)) != 0)
                {
                    string bufstr = new string(buffer);
                    //очищаем хвост буфера при последнем чтении
                    if (len < buflen) { bufstr = bufstr.Remove(len); }

                    //C# не видит все что дальше переноса, заменим его на пустую строку. В запросах исползуются все три вида переноса.
                    bufstr = bufstr.Replace(Environment.NewLine, "");
                    bufstr = bufstr.Replace("\r", "");
                    bufstr = bufstr.Replace("\n", "");
                    //Разобьем на строки начинаем со второй
                    string pattern = "(.)([0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,)";
                    bufstr = Regex.Replace(bufstr, pattern, "$1" + '\n' + "$2");
                    string[] strTJ = bufstr.Split('\n');

                    int lastcount = strTJ.Length;
                    if (lastcount == 0) { break; }

                    int index = 0;

                    if (Regex.IsMatch(strTJ[0], "[0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,") == false)
                    {
                        rollstr = rollstr.Replace(Environment.NewLine, "");
                        TJList.Add(ParseStringTJ(rollstr + strTJ[0], datesfile));
                        index = 1;
                    }

                    for (int i= index; i<lastcount;i++)
                    {
                        if ((i == lastcount - 1) & (len == buflen)) { rollstr = strTJ[i]; }
                        else { TJList.Add(ParseStringTJ(strTJ[i], datesfile)); }
                    }
                }
            }
            return TJList;

        }
    }
}
