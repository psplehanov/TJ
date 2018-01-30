using System.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

class TJCoordFin
{
    public List<string> filenames;
    public List<long> offsets;

    public TJCoordFin()
    {
        filenames = new List<string>();
        offsets = new List<long>();
    }

}

class TJCoord
{
    public string filename;
    public long offset;

    public TJCoord(string filename_, long offset_)
    {
        filename = filename_;
        offset = offset_;
    }

    public TJCoord()
    {
        filename = "";
        offset = 0;
    }
}

class TJobject
{
    public DateTime date;
    public int mks;
    public long durability;
    public string tjevent;
    public int level;
    public string parent;

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

    public string Finish;

    public int WaitConnections;
    public int SessionID;
    public string Regions;
    public string Locks;
    public string DeadlockConnectionIntersections;
    public bool escalating;

    public string AppID;
    public string Ref;
    public string Host;
    public int Connection;

}


class TJ
{
    public const int buflen = 65536; //65536

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

        var patternfile = new Regex("[0-9]{8}.log");
        string datesfile = patternfile.Match(filename).Value;
        datesfile = datesfile.Remove(8);

        patternfile = new Regex(@"[^\\]+\\[0-9]{8}.log");
        string parent = patternfile.Match(filename).Value;
        T.parent = parent.Remove(parent.Length - 13);

        //дата и время начала выполнения
        var subpattern = new Regex("[0-9][0-9]:[0-9][0-9]");
        tmp = datesfile + subpattern.Match(strTJ).Value;
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
        T.property.Txt = GetParamTXT(@"Txt=", strTJ);
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
        T.property.Finish = GetParam(@"Finish=", strTJ);

        T.property.WaitConnections = Convert.ToInt32(GetParam(@"WaitConnections=", strTJ));
        T.property.SessionID = Convert.ToInt32(GetParam(@"SessionID=", strTJ));
        T.property.Regions = GetParam(@"Regions=", strTJ);
        T.property.Locks = GetParam(@"Locks=", strTJ);
        T.property.DeadlockConnectionIntersections = GetParam(@"DeadlockConnectionIntersections=", strTJ);
        if (GetParam(@"escalating=", strTJ) == "true") { T.property.escalating = true; } else { T.property.Trans = false; }

        T.property.AppID = GetParam(@"AppID=", strTJ);
        T.property.Ref = GetParam(@"Ref=", strTJ);
        T.property.Host = GetParam(@"Host=", strTJ);
        T.property.Connection = Convert.ToInt32(GetParam(@"Connection=", strTJ));
        return T;
    }


    #region ReadTJ
    /// <summary>
    ///Чтение элементов ТЖ в количестве count с возможностью чтения только измененных файлов
    ///Возвращает true если есть данные для чтения
    /// </summary>
    /// <param name="path">путь к папке с ТЖ</param>
    /// <param name="TJList">возвращает список элементов ТЖ</param>
    /// <param name="count">количество элементов для чтения</param>
    /// <param name="coord">координаты места начала|окончания чтения</param>
    /// <param name="coordFin">координаты места окончания чтения текста</param>
    public static bool ReadTJ(string path, ref List<TJobject> TJList, int count, ref TJCoord coord, ref TJCoordFin coordFin)
    {
        bool pass = true;
        //if (coord.filename == "") { pass = false; }

        List<string> files = new List<string>(Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories));

        foreach (var f in files)
        {
            int len = 0;
            string rollstr = "";
            long offset = 0;
            long offsetfin = 0;

            //Ищем конец файла при чтении с изменениями
            if (coordFin.filenames != null)
            {
                for (int i = 0; i < coordFin.filenames.Count; i++)
                {
                    if (coordFin.filenames[i] == f)
                    {
                        offsetfin = coordFin.offsets[i];
                        break;
                    }
                }
            }

            //смещение при чтении файла, местоположение в файле после чтения блока
            if ((coord.filename == f) || (coord.filename == ""))  { pass = false; }
            //так как мы читаем файлы последовательно, пропускаем те которые уже были прочитаны.
            if (pass) { continue; }

            char[] buffer = new char[buflen];
            byte[] bufferbyte = new byte[buflen];

            BinaryReader objReader = new BinaryReader(File.Open(f, FileMode.Open));
            objReader.BaseStream.Position = offsetfin + coord.offset;

            while (true)
            {
                bufferbyte = objReader.ReadBytes(buflen);
                len = bufferbyte.Length;

                offset = offset + len;

                //Разобьем на строки начинаем со второй
                string bufstr = ArrByteToArrString(bufferbyte);
                string pattern = "(.)([0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,)";
                bufstr = Regex.Replace(bufstr, pattern, "$1" + '\n' + "$2");
                string[] strTJ = bufstr.Split('\n');

                int lastcount = strTJ.Length;
                if (lastcount == 0) { break; }

                //Соединяем последнюю строку из прошлого чтения и первую этого
                int index = FirstAndLastString(ref strTJ, ref rollstr, ref TJList, f);

                for (int i = index; i < lastcount; i++)
                {
                    //Количество возвращаемых элементов достигло count
                    if (count == 0)
                    {
                        coord.filename = f;

                        //Ищем следующий элемент strTJ[i] в tmpstr                                 
                        string findstr = strTJ[i].Remove(13);


                        if (offset < buflen)
                        {
                            coord.offset = FindStrInArrByte(findstr, bufferbyte) + coord.offset;
                        }
                        else
                        {
                            coord.offset = offset - len + FindStrInArrByte(findstr, bufferbyte) + coord.offset;
                        }

                        objReader.Close();
                        return true;
                    }

                    if ((i == lastcount - 1) & (len == buflen)) { rollstr = strTJ[i]; }
                    else { TJList.Add(ParseStringTJ(strTJ[i], f)); }

                    count--;
                }

                //окончание файла. есть возможность, что len == buflen и при этом наступил конец файла 
                if (len != buflen)
                {
                    int indexFile = 0;
                    indexFile = coordFin.filenames.IndexOf(f);
                    if (indexFile == -1)
                    {
                        coordFin.filenames.Add(f);
                        coordFin.offsets.Add(objReader.BaseStream.Position);
                    }
                    else
                    {
                        coordFin.offsets[indexFile] = objReader.BaseStream.Position;
                    }
                    break;
                }
            }
            coord.offset = 0;
            objReader.Close();
        }

        coord.filename = "";
        coord.offset = 0;
        return false;
    }


    /// <summary>
    ///Чтение элементов ТЖ в количестве count
    /// </summary>
    /// <param name="path">путь к папке с ТЖ</param>
    /// <param name="TJList">возвращает список элементов ТЖ</param>
    /// <param name="count">количество элементов для чтения</param>
    /// <param name="coord">координаты места начала|окончания чтения</param>
    public static bool ReadTJ(string path, ref List<TJobject> TJList, int count, ref TJCoord coord)
    {
        bool pass = true;
        if (coord.filename == "") { pass = false; }

        List<string> files = new List<string>(Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories));

        foreach (var f in files)
        {
            int len = 0;
            string rollstr = "";
            long offset = 0;

            //смещение при чтении файла, местоположение в файле после чтения блока
            if (coord.filename == f) { pass = false; }
            if (pass) { continue; }

            char[] buffer = new char[buflen];
            byte[] bufferbyte = new byte[buflen];

            BinaryReader objReader = new BinaryReader(File.Open(f, FileMode.Open));
            objReader.BaseStream.Position = coord.offset;

            while (true)
            {
                bufferbyte = objReader.ReadBytes(buflen);
                len = bufferbyte.Length;

                offset = offset + len;

                //Разобьем на строки начинаем со второй
                string bufstr = ArrByteToArrString(bufferbyte);
                string pattern = "(.)([0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,)";
                bufstr = Regex.Replace(bufstr, pattern, "$1" + '\n' + "$2");
                string[] strTJ = bufstr.Split('\n');

                int lastcount = strTJ.Length;
                if (lastcount == 0) { break; }

                //Соединяем последнюю строку из прошлого чтения и первую этого
                int index = FirstAndLastString(ref strTJ, ref rollstr, ref TJList, f);

                for (int i = index; i < lastcount; i++)
                {
                    //Количество возвращаемых элементов достигло count
                    if (count == 0)
                    {
                        coord.filename = f;

                        //Ищем следующий элемент strTJ[i] в tmpstr                                 
                        string findstr = strTJ[i].Remove(13);


                        if (offset < buflen)
                        {
                            coord.offset = FindStrInArrByte(findstr, bufferbyte) + coord.offset;
                        }
                        else
                        {
                            coord.offset = offset - len + FindStrInArrByte(findstr, bufferbyte) + coord.offset;
                        }

                        objReader.Close();
                        return true;
                    }

                    if ((i == lastcount - 1) & (len == buflen)) { rollstr = strTJ[i]; }
                    else { TJList.Add(ParseStringTJ(strTJ[i], f)); }

                    count--;
                }

                if (len != buflen) { break; }
            }
            coord.offset = 0;
            objReader.Close();
        }

        coord.filename = "";
        coord.offset = 0;
        return false;
    }


    /// <summary>
    /// Чтение всех элементов ТЖ
    /// </summary>
    /// <param name="path">путь к папке с ТЖ</param>
    /// <param name="TJList">возвращает список элементов ТЖ</param>
    /// <returns></returns>
    public static void ReadTJ(string path, ref List<TJobject> TJList)
    {
        char[] buffer = new char[buflen];

        List<string> files = new List<string>(Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories));

        foreach (var f in files)
        {
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

                //Соединяем последнюю строку из прошлого чтения и первую этого
                int index = FirstAndLastString(ref strTJ, ref rollstr, ref TJList, f);

                //просматриваем все остальные строки
                for (int i = index; i < lastcount; i++)
                {
                    if ((i == lastcount - 1) & (len == buflen)) { rollstr = strTJ[i]; }
                    else { TJList.Add(ParseStringTJ(strTJ[i], f)); }
                }
            }
            objReader.Close();
        }
    }

    #endregion

    #region support

    /// <summary>
    /// Нахождение строки в массиве байтов
    /// </summary>
    /// <param name="findstr">Строка поиска</param>
    /// <param name="bufferbyte">Массив в котором осуществляем поиск</param>
    static int FindStrInArrByte(string findstr, byte[] bufferbyte)
    {
        byte[] findbyte = new byte[13];
        for (int j = 0; j < findstr.Length; j++)
        {
            findbyte[j] = Convert.ToByte(findstr[j]);
        }

        bool errfind = false;
        for (int j = 0; j < bufferbyte.Length; j++)
        {
            for (int z = 0; z < 13; z++)
            {
                if (bufferbyte[j + z] != findbyte[z])
                {
                    errfind = true;
                    break;
                }
                errfind = false;
            }
            if (!errfind) { return j; }
        }

        return -1;
    }

    /// <summary>
    /// Преобразование массива байт в строку
    /// </summary>
    /// <param name="bufferbyte">массив байт</param>
    static string ArrByteToArrString(byte[] bufferbyte)
    {
        string bufstr = System.Text.Encoding.UTF8.GetString(bufferbyte).TrimEnd('\0');

        //Из-за "низкого" уровня чтения мы читаем BOM символ
        Encoding unicode = Encoding.Unicode;
        byte[] BOM = new byte[2];
        BOM[0] = 0xff; BOM[1] = 0xfe;
        char[] BOMchar = unicode.GetChars(BOM);
        bufstr = bufstr.Replace(BOMchar[0], '\r');

        //C# не видит все что дальше переноса, заменим его на пустую строку. В запросах исползуются все три вида переноса.
        bufstr = bufstr.Replace(Environment.NewLine, "");
        bufstr = bufstr.Replace("\r", "");
        bufstr = bufstr.Replace("\n", "");

        return bufstr;
    }

    /// <summary>
    /// Извлекает параметр из строки ТЖ.
    /// </summary>
    /// <param name="param">параметр по которому осуществляется поиск в строке ТЖ</param>
    /// <param name="source">строка ТЖ</param>
    static string GetParam(string param, string source)
    {
        int sublen = param.Length;
        var pattern = new Regex(param + "[^,$]*");
        string tmp = pattern.Match(source).Value;
        if (tmp.Length > sublen) { tmp = tmp.Substring(sublen); }
        else { tmp = null; }
        return tmp;
    }

    /// <summary>
    /// Извлекает параметр TXT из строки ТЖ.
    /// </summary>
    /// <param name="param">параметр по которому осуществляется поиск в строке ТЖ</param>
    /// <param name="source">строка ТЖ</param>
    static string GetParamTXT(string param, string source)
    {
        int sublen = param.Length;
        var pattern = new Regex(param + "\'.*\'");
        string tmp = pattern.Match(source).Value;
        if (tmp.Length > sublen) { tmp = tmp.Substring(sublen); }
        else { tmp = null; }
        return tmp;
    }

    /// <summary>
    /// Соединяет последний элемент предыдущего буфера и первый элемент текущего буфера. Разделяет на строки ТЖ и записывает в массив объектов ТЖ TJList
    /// </summary>
    /// <param name="strTJ">массив строк ТЖ текущего буфера</param>
    /// <param name="rollstr">элемент предыдущего буфера</param>
    /// <param name="TJList">массив объектов ТЖ</param>
    /// <param name="f">имя файла</param>
    static int FirstAndLastString(ref string[] strTJ, ref string rollstr, ref List<TJobject> TJList, string f)
    {
        int index = 0;

        if (Regex.IsMatch(strTJ[0], "[0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,") == false)
        {
            rollstr = rollstr.Replace(Environment.NewLine, "") + strTJ[0];

            if (strTJ.Length != 1)
            {
                string rollpattern = "(.)([0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,)";
                rollstr = Regex.Replace(rollstr, rollpattern, "$1" + '\n' + "$2");
                string[] rollstrTJ = rollstr.Split('\n');

                for (int i = 0; i < rollstrTJ.Length; i++)
                {
                    if (Regex.IsMatch(rollstrTJ[i], "[0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,"))
                    {
                        TJList.Add(ParseStringTJ(rollstrTJ[i], f));
                    }
                }

            }
            else
            {
                int test = 0;
                test++;
            }
            index = 1;
        }
        else
        {
            if (Regex.IsMatch(rollstr, "[0-9][0-9]:[0-9][0-9]\\.([0-9]{4}|[0-9]{6})-[0-9]+,") == true)
            {
                TJList.Add(ParseStringTJ(rollstr, f));
            }
        }
        return index;
    }

    #endregion
}