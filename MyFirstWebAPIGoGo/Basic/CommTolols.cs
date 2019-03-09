using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.IO.Pipes;
using System.Drawing;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Collections;

namespace MyFirstWebAPIGoGo
{
    public static class Extension
    {
        public static string ToSqlStr(this string s)
        {
            return "'" + s.Replace("'", "") + "'";
        }

        public static string ObjToString(this string s)
        {
            if (s == null)
                return "";
            else
                return s.ToString();
        }

        /// <summary>
        /// 字串截斷特殊字元(\t,\n,\r,\0)
        /// </summary>
        //2018.11.06 Arthur 新增 字串截斷所有異常字元
        public static string TrimSpecChar(this string s)
        {
            string[] MyChar = { "\t", "\n", " ", "\r" };
            string res = "";

            return s.Replace(MyChar[0], res).Replace(MyChar[1], res).Replace(MyChar[2], res).Replace(MyChar[3], res);
        }

        //2018.07.26 Arthur 新增 字串擷取判斷
        //2018.11.22 Arthur 修改欲擷取長度大於字串長度時，擷取長度為字串長度
        //字串擷取判斷
        public static string SubStrFormat(this string s, int startindx, int length)
        {
            if ((s == null) || (s.Trim().Length <= 0) || (startindx < 0) || (length <= 0) || (s.Length < startindx))
                return "";

            if ((s.Length <= (startindx + length - 1))) length = s.Length;

            return s.Substring(startindx, length).Trim();
        }

        /// <summary>
        /// Quoted string for SQL 若空字串則為NULL by Milo 2018.09.14 新增
        /// </summary>
        /// <param name="str">傳入字串</param>
        /// <param name="bNchar">若false不加N'; 若true加N'for nvarchar</param>
        /// <returns></returns>
        public static string QuotedStrSQL(this string str, bool bNchar = false, bool bEmptyToNull = false)
        {
            if (string.IsNullOrEmpty(str) && bEmptyToNull)
            {
                return "NULL";
            }

            if (str.ToUpper() == "NULL")
            { return str; }
            else
            {
                if (bNchar == true)
                { return "N'" + str.Replace("'", "''") + "'"; }
                else
                { return "'" + str.Replace("'", "''") + "'"; }
            }
        }

        //2018.12.21 Arthur 新增 取得陣列內容並回傳下一指標
        //取得陣列內容並回傳下一指標
        public static object GetAndGoNext(this object[] array, int current, out int next)
        {
            next = current + 1;
            return array[current];
        }
        //取得陣列內容並回傳下一指標(跳躍)
        public static object GetAndGoNext(this object[] array, int current, int skip, out int next)
        {
            next = current + skip;
            return array[current];
        }

        //2018.12.27 Arthur 新增 設定物件屬性設定
        public static void SetPropertyValue(this object obj, string propertyname, object value)
        {
            obj.GetType().GetProperty(propertyname).SetValue(obj, value);
        }

        //2018.12.27 Arthur 新增 取得物件屬性值
        public static object GetPropertyValue(this object obj, string propertyname)
        {
            return obj.GetType().GetProperty(propertyname).GetValue(obj);
        }
    }

    class CommTolols
    {
        static object lockMe = new object();

        DataCon conn = new DataCon();

        public DateTime GetDateTimeNow()
        {
            DateTime dtNow = DateTime.Now;
            return dtNow;
        }

        public string GetDateTimeNow(string sFormat)
        {
            return DateTime.Now.ToString(sFormat);
        }

        public string sGetWeekDigi(DateTime datetime)
        {
            return datetime.DayOfWeek.ToString("d");
        }
        public string sGetWeekStr(DateTime datetime)
        {
            return datetime.DayOfWeek.ToString();
        }
        public string sGetDayOfYear(DateTime datetime)
        {
            return datetime.DayOfYear.ToString();
        }
        public string sGetTime(string format)
        {
            return DateTime.Now.ToString(@"" + format + "");
        }
        public string sStrSuppZero(int nNum, string sTemp)
        {
            return String.Format("{0:" + sTemp + "}", nNum);
        }
   
        public OleDbConnection oleGetOleDBcon(string FileName)
        {
            OleDbConnection OleDbCon;
            string M_str_sqlcon = "";
            string[] sArray = FileName.Split('.');

            if (sArray[sArray.Length - 1].Trim().ToUpper() == "XLS")
            {
                M_str_sqlcon =

                     "Data Source=" + FileName + ";" +

                     "Provider=Microsoft.Jet.OLEDB.4.0;" +

                     "Extended Properties='Excel 8.0;" +

                     "HDR=Yes;" +

                     "IMEX=1;'";

            }
            else if (sArray[sArray.Length - 1].Trim().ToUpper() == "XLSX")
            {
                M_str_sqlcon =

                     "Data Source=" + FileName + ";" +

                     "Provider=Microsoft.ACE.OLEDB.12.0;" +

                     "Extended Properties='Excel 12.0;" +

                     "HDR=Yes;" +

                     "IMEX=1;'";
            }
            OleDbCon = new OleDbConnection(M_str_sqlcon);
            return OleDbCon;
        }

        /*
        //螢幕截圖存檔
        public string PrintScreenSave()
        {
            //2017.12.01 Joy 修正截圖有誤會死掉
            try
            {
                string sPath = sys.sLodFolder;
                if (!Directory.Exists(sys.sLodFolder))
                {
                    if (!String.IsNullOrEmpty(sys.sLodFolder.Trim()))
                    {
                        Directory.CreateDirectory(sys.sLodFolder);
                    }
                    else
                    {
                        sPath = sys.sGetFileLocation();
                    }
                }

                sPath = sPath + "\\FileIOErr_" + GetDateTimeNow("yyyy_MM_dd_HH_mm_ss_fff") + ".jpg";
                Bitmap myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(myImage);
                //取得螢幕左上角座標 (0,0) 開始，大小(Size) 螢幕寬x螢幕高 的面積
                g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

                IntPtr dc1 = g.GetHdc();
                g.ReleaseHdc(dc1);
                //存檔
                myImage.Save(sPath);
                //釋放物件
                myImage.Dispose();
                g.Dispose();
                return sPath;
            }
            catch
            {
                throw;
            }
        }

        //螢幕截圖存檔
        public string PrintScreenSave(string sPath)
        {
            //2017.12.01 Joy 修正截圖有誤會死掉
            try
            {
                if (!Directory.Exists(sPath))
                {
                    if (!String.IsNullOrEmpty(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    else
                    {
                        sPath = sys.sGetFileLocation();
                    }
                }

                sPath = sPath + "\\FileIOErr_" + GetDateTimeNow("yyyy_MM_dd_HH_mm_ss_fff") + ".jpg";
                Bitmap myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(myImage);
                //取得螢幕左上角座標 (0,0) 開始，大小(Size) 螢幕寬x螢幕高 的面積
                g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

                IntPtr dc1 = g.GetHdc();
                g.ReleaseHdc(dc1);
                //存檔
                myImage.Save(sPath);
                //釋放物件
                myImage.Dispose();
                g.Dispose();
                return sPath;
            }
            catch
            {
                throw;
            }
        }

        //螢幕截圖回傳圖片
        public Bitmap PrintScreen()
        {
            Bitmap myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(myImage);
            //取得螢幕左上角座標 (0,0) 開始，大小(Size) 螢幕寬x螢幕高 的面積
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);
            //釋放物件
            g.Dispose();
            return myImage;
        }

        public DataTable dtGetCustomerList(string sCustno)
        {
            string sSql = "";

            //客戶資料
            if (sCustno == "<ALL>")
            {
                sSql = "SELECT * FROM o_customer WHERE isnull(datat,'') = 'Y'";
            }
            else
            {
                sSql = "SELECT * FROM o_customer WHERE isnull(datat,'') = 'Y' AND custno = '" + sCustno + "' ";
            }

            return conn.GetDataTable(sSql);
        }

        //2019.02.22 Joy 新增抓品牌資料
        public DataTable dtGetCustomerList(string sCustno, string sBrand)
        {
            string sSql = String.Format("Select a.fpath,a.opath,a.pay3,a.iofor,b.* from o_brand b left join o_customer a " +
                " on a.custno = b.custno where isnull(a.datat,'') = 'Y' AND b.custno = '{0}' and b.brand = '{1}'",
                sCustno, sBrand);

            return conn.GetDataTable(sSql);
        }
        */

        //2017.09.20 Joy 新增中英混字補空白及截斷 ++>
        /// <summary>
        /// 判斷文字的Bytes數有沒有超過上限，有的話截斷，沒有的話補空白
        /// int:true(補左邊)，false(補右邊)
        /// </summary>        
        public string String_Cut_Add(string value, int maxLength, char AddChar, bool type)
        {
            /*if (string.IsNullOrWhiteSpace(value) || maxLength <= 0)
            {
                return string.Empty;
            }*/

            var result = CutString_Base(value, maxLength);

            if (result.Item2 == 0)
            {
                //2017.11.13 Joy 修正如無需要補足的長度，直接回傳原字串即可
                //return result.Item1.PadLeft(maxLength, AddChar);
                return result.Item1;
            }
            else
            {
                if (type)
                    return "".PadLeft(result.Item2, AddChar) + result.Item1;
                else
                    return result.Item1 + "".PadRight(result.Item2, AddChar);
            }
        }
        /// <summary>
        /// 給截字補空與截字使用
        /// </summary>
        private static Tuple<string, int> CutString_Base(string value, int maxLength)
        {
            int padding = 0;
            var buffer = Encoding.GetEncoding("big5").GetBytes(value);
            if (buffer.Length > maxLength)
            {
                int charStartIndex = maxLength - 1;
                int charEndIndex = 0;
                //跑回圈去算出結尾。
                for (int i = 0; i < maxLength;)
                {
                    if (buffer[i] <= 128)
                    {
                        charEndIndex = i; //英數1Byte
                        i += 1;
                    }
                    else
                    {
                        charEndIndex = i + 1; //中文2Byte
                        i += 2;
                    }
                }

                //如果開始不同與結尾，表示截到2Byte的中文字了，要捨棄1Byte
                if (charStartIndex != charEndIndex)
                {
                    value = Encoding.GetEncoding("big5").GetString(buffer, 0, charStartIndex);
                    padding = 1;
                }
                else
                {
                    value = Encoding.GetEncoding("big5").GetString(buffer, 0, maxLength);
                }
            }
            else
            {
                value = Encoding.GetEncoding("big5").GetString(buffer);

                padding = maxLength - buffer.Length;
            }

            return Tuple.Create(value, padding);
        }
        /// <summary>
        /// 判斷文字的Bytes數有沒有超過上限，有的話截斷
        /// </summary>
        public string CutString(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || maxLength <= 0)
            {
                return string.Empty;
            }

            return CutString_Base(value, maxLength).Item1;
        }
        //2017.09.20 Joy 新增中英混字補空白及截斷 <++

        /// <summary>
        /// 計算資料夾路徑內與檔名條件相符的數量
        /// </summary> 
        /// <param name="sPath">資料夾路徑</param>
        /// <param name="sFileName">檔名條件</param>
        public int nGetFileCount(string sPath, string sFileName)
        {
            DirectoryInfo di = new DirectoryInfo(sPath);
            FileInfo[] files = di.GetFiles(sFileName); //篩選檔名
            return files.Length;
        }
        
        //2018.02.26 Arthur 新增 取得OleDb連線
        //取得OleDb連線
        public System.Data.OleDb.OleDbConnection GetOleDBcon(string FileName)
        {
            string M_str_sqlcon = "";
            string sFileExtension = Path.GetExtension(FileName);

            if (sFileExtension.ToUpper() == ".XLS")
            {
                M_str_sqlcon =

                     "Data Source=" + FileName + ";" +

                     "Provider=Microsoft.Jet.OLEDB.4.0;" +

                     "Extended Properties='Excel 8.0;" +

                     "HDR=No;" +

                     "IMEX=1;'";

            }
            else if (sFileExtension.ToUpper() == ".XLSX")
            {
                M_str_sqlcon =

                     "Data Source=" + FileName + ";" +

                     "Provider=Microsoft.ACE.OLEDB.12.0;" +

                     "Extended Properties='Excel 12.0;" +

                     "HDR=YES;" +

                     "IMEX=1;'";
            }
            else if (sFileExtension.ToUpper() == ".CSV")
            {
                string temp = Path.GetDirectoryName(FileName);

                M_str_sqlcon =
                     "Provider=Microsoft.Jet.OLEDB.4.0;" +
                     "Data Source=" + temp + ";" +
                     "Extended Properties='Text;HDR=NO;IMEX=1'";
            }

            System.Data.OleDb.OleDbConnection OleDbCon = new System.Data.OleDb.OleDbConnection(M_str_sqlcon);
            return OleDbCon;
        }

        //2018.02.27 Joe新增
        public string JSON_WebPost(string url, string Json)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            string result = null;
            request.Method = "POST";    // 方法
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/json";

            //2018.08.09 Lara 加入逾時判斷
            int nTimeOut = 20 * 1000;
            request.Timeout = nTimeOut; //逾時時間


            byte[] bs = Encoding.UTF8.GetBytes(Json);

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }

        //2019.03.06 Phoena 新增JSON格式需傳入帳密
        public string JSON_WebPost_PW(string url, string Json, string password)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            string result = null;
            request.Method = "POST";    // 方法
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/json";
            //帳密
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(password)));

            //2018.08.09 Lara 加入逾時判斷
            int nTimeOut = 20 * 1000;
            request.Timeout = nTimeOut; //逾時時間


            byte[] bs = Encoding.UTF8.GetBytes(Json);

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }

        /// <summary>
        /// 2018.02.22 Lara 新增UTF8轉BIG5
        /// </summary>
        /// <param name="strUtf"></param>
        /// <returns></returns>
        public string ConvertBig5(string strUtf)
        {
            Encoding utf81 = Encoding.GetEncoding("utf-8");
            Encoding big51 = Encoding.GetEncoding("big5");

            byte[] strUtf81 = utf81.GetBytes(strUtf.Trim());
            byte[] strBig51 = Encoding.Convert(utf81, big51, strUtf81);

            char[] big5Chars1 = new char[big51.GetCharCount(strBig51, 0, strBig51.Length)];
            big51.GetChars(strBig51, 0, strBig51.Length, big5Chars1, 0);
            string tempString1 = new string(big5Chars1);
            return tempString1;
        }

        /// <summary>
        /// 2018.02.21 Lara 新增C#Big5編碼擷取字串問題，index+1
        /// </summary>
        /// <param name="a_SrcStr"></param>
        /// <param name="a_StartIndex"></param>
        /// <param name="a_Cnt"></param>
        /// <returns></returns>
        public string SubStr2(string a_SrcStr, int a_StartIndex, int a_Cnt)
        {
            a_StartIndex = a_StartIndex - 1;
            Encoding l_Encoding = Encoding.GetEncoding("big5", new EncoderExceptionFallback(), new DecoderReplacementFallback(""));
            byte[] l_byte = l_Encoding.GetBytes(a_SrcStr);
            if (a_Cnt <= 0)
                return "";
            //例若長度10 
            //若a_StartIndex傳入9 -> ok, 10 ->不行 
            if (a_StartIndex + 1 > l_byte.Length)
                return "";
            else
            {
                //若a_StartIndex傳入9 , a_Cnt 傳入2 -> 不行 -> 改成 9,1 
                if (a_StartIndex + a_Cnt > l_byte.Length)
                    a_Cnt = l_byte.Length - a_StartIndex;
            }
            return l_Encoding.GetString(l_byte, a_StartIndex, a_Cnt).Trim();

        }

        /// <summary>
        /// 2018.05.05 Joe 新增解析參數
        /// 範例
        /// order_result=1&company_orderno=S18011215550&ship_no=117000805053&sys_orderno=W20180505365020&distsite_id=HZ09
        /// </summary>
        /// <returns></returns>
        public string GetParaResult(string Key, string ResStr)
        {
            //找出要尋找的字串在全部字串的哪個位置
            var j = ResStr.IndexOf(Key);
            //找位置後+1，再加上自己的位置
            //加一是因為要算進'='的位置
            j = j + Key.Length + 1;
            var result = "";
            //全部字串的長度
            var k = ResStr.Length - 1;

            while (j <= k)
            {
                if ((ResStr[j].ToString() != "&") && (ResStr[j].ToString() != "<") || (j == k))
                {
                    result = result + ResStr[j];
                }
                else
                {
                    j = ResStr.Length;
                }
                j++;
            }

            return result;
        }
        /// <summary>
        /// Bot_ID   要自己去申請Telegram的機器人帳號，請上網搜尋
        /// Chat_ID  看要發給誰
        /// </summary>
        /// <param name="Bot_ID"></param>
        /// <param name="Chat_ID"></param>
        /// <param name="Message"></param>
        public void TGBot_SendMesg(string Bot_ID, string Chat_ID, string Message)
        {
            var sUrl = "https://api.telegram.org/bot" + Bot_ID + "/SendMessage";


            var sParam = "chat_id=" + Chat_ID;
            sParam = sParam + "&" + "text=" + Message.Replace("&", "");
            byte[] byteArray = Encoding.UTF8.GetBytes(sParam);


            HttpWebRequest request = HttpWebRequest.Create(sUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            request.KeepAlive = true; //是否保持連線
            var sResult = "";
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
                reqStream.Close();
            }

            using (WebResponse response = request.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                sResult = reader.ReadToEnd();
                reader.Close();
                response.Close();
            }


        }

        //清除空位元組
        public static byte[] byteCut(byte[] b, byte cut)
        {
            List<byte> list = new List<byte>();
            list.AddRange(b);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == cut)
                    list.RemoveAt(i);
            }
            byte[] lastbyte = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                lastbyte[i] = list[i];
            }
            return lastbyte;
        }

        /// <summary>
        /// 2018.05.17 Joe新增用來判斷是否是日期格式
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        

        //2018.06.25 Arthur 新增 
        //資料列轉物件
        public object SetRowValueToObj(object type, DataRow data)
        {
            object obj = type;
            System.Reflection.PropertyInfo[] propInfo = obj.GetType().GetProperties();
            for (int iprp = 0; iprp < propInfo.Length; iprp++)
            {
                string PropName = propInfo[iprp].Name;

                if (data.Table.Columns.Contains(PropName))
                {
                    string Value = data[PropName].ToString().Trim();

                    if (propInfo[iprp].PropertyType.FullName.Contains("Int"))
                    {
                        Int16 val = new Int16();
                        if (!String.IsNullOrEmpty(Value) && Int16.TryParse(Value, out val))
                            obj.GetType().GetProperty(PropName).SetValue(obj, Convert.ToInt32(Value));
                    }
                    else if (propInfo[iprp].PropertyType.FullName.Contains("Double"))
                    {
                        Double val = new Double();
                        if (!String.IsNullOrEmpty(Value) && Double.TryParse(Value, out val))
                            obj.GetType().GetProperty(PropName).SetValue(obj, val);
                    }
                    else if (propInfo[iprp].PropertyType.FullName.Contains("Decimal"))
                    {
                        decimal val = new decimal();
                        if (!String.IsNullOrEmpty(Value) && Decimal.TryParse(Value, out val))
                            obj.GetType().GetProperty(PropName).SetValue(obj, val);
                    }
                    else if (propInfo[iprp].PropertyType.FullName.Contains("DateTime"))
                    {
                        DateTime val = new DateTime();
                        if (!String.IsNullOrEmpty(Value) && DateTime.TryParse(Value, out val))
                            obj.GetType().GetProperty(PropName).SetValue(obj, val);
                    }
                    else
                    {
                        obj.GetType().GetProperty(PropName).SetValue(obj, Value);
                    }
                }
            }

            return obj;
        }

        //2018.06.25 Arthur 新增 
        //物件內容傳入資料列
        public DataRow SetObjectToTable(object obj, DataTable table)
        {
            if (obj == null) return null;

            System.Reflection.PropertyInfo[] propInfo = obj.GetType().GetProperties();
            DataRow row = table.NewRow();

            try
            {
                for (int iprp = 0; iprp < propInfo.Length; iprp++)
                {
                    string PropName = propInfo[iprp].Name;

                    if (row.Table.Columns.Contains(PropName))
                    {
                        var Value = obj.GetType().GetProperty(PropName).GetValue(obj, null);

                        if (Value == null) continue;

                        switch (propInfo[iprp].PropertyType.Name)
                        {
                            case "String":
                                row[PropName] = Value.ToString();
                                break;
                            case "Int":
                                row[PropName] = Convert.ToInt16(Value);
                                break;
                            case "Double":
                                row[PropName] = Convert.ToDouble(Value);
                                break;
                            case "Float":
                                row[PropName] = Convert.ToDouble(Value);
                                break;
                            case "DateTime":
                                row[PropName] = Convert.ToDateTime(Value);
                                break;
                            default:
                                row[PropName] = Value;
                                break;
                        }
                    }
                }

                row.EndEdit();
            }
            catch (Exception e)
            {
                throw;
            }

            return row;
        }
        
        /// <summary>
        /// 取得檔案資料
        /// </summary>
        /// <param name="tempName"></param>
        /// <returns></returns>
        public string GetFileTemp(string tempName)
        {
            //輸出
            string result = "";
            //實體路徑的html檔 = 應用程式所在的目錄 + 所在頁面
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Template\" + tempName;
            //有資料
            if (File.Exists(path))
            {
                //讀取檔案
                StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("Big5"));
                //轉字串給輸出
                result = streamReader.ReadToEnd();

            }
            return result;
        }

        //以特定編碼字元截斷字串
        public string SubStrOnByte(byte[] data, Encoding encode, int startindx, int length)
        {
            if (data.Length <= 0 || (startindx < 0) || (length <= 0) || (data.Length < startindx) || (data.Length <= (startindx + length - 1)))
                return "";

            return encode.GetString(data, startindx, length).Trim();
        }
        

        //2018.09.16 Arthur 新增 字串轉換
        public static decimal ConvertToDecimal(string str)
        {
            decimal res = new decimal();

            return (decimal.TryParse(str, out res) ? res : 0);

        }
        //2018.09.16 Arthur 新增 字串轉換
        public static double ConvertToDouble(string str)
        {
            double res = new double();

            return (double.TryParse(str, out res) ? res : 0);

        }
        //2018.09.16 Arthur 新增 字串轉換
        public static DateTime ConvertToDatetime(string str)
        {
            DateTime res = DateTime.Now;

            return (DateTime.TryParse(str, out res) ? res : DateTime.Now);

        }
        //2018.09.16 Arthur 新增 物件轉資料列
        public static DataRow CreatDataRowFromObject<Type>(DataTable table, object inputobj)
        {
            DataRow row = table.NewRow();
            Type obj = (Type)inputobj;

            foreach (System.Reflection.PropertyInfo info in obj.GetType().GetProperties())
            {
                if (info.GetValue(obj) == null || !table.Columns.Contains(info.Name)) continue;

                string PropName = info.Name;
                string PropValue = info.GetValue(obj).ToString();

                row[PropName] = PropValue;
            }

            return row;
        }

        //2018.10.02 Arthur 新增 資料表轉換成物件
        //資料表轉換成物件
        public List<RespondTypeObj> TableConvertToObject<RespondTypeObj>(DataTable table) where RespondTypeObj : new()
        {
            if (table == null || table.Rows.Count <= 0) return null;

            List<RespondTypeObj> lsOBJ = new List<RespondTypeObj>();
            foreach (DataRow row in table.Rows)
            {
                RespondTypeObj rsp = new RespondTypeObj();
                rsp = (RespondTypeObj)SetRowValueToObj(rsp, row);

                lsOBJ.Add(rsp);
            }

            return lsOBJ;
        }

        //2018.10.17 Arthur 新增 
        //偵測byte[]是否為BIG5編碼
        public static bool IsBig5Encoding(byte[] bytes)
        {
            Encoding big5 = Encoding.GetEncoding(950);
            //將byte[]轉為string再轉回byte[]看位元數是否有變
            return bytes.Length ==
                big5.GetByteCount(big5.GetString(bytes));
        }
        //偵測檔案否為BIG5編碼
        public static bool IsBig5Encoding(string file)
        {
            if (!File.Exists(file)) return false;
            return IsBig5Encoding(File.ReadAllBytes(file));
        }

        //判斷當日檔案是否重複產檔
        public bool IsSameFile(String sFileName, String sCondition)
        {
            string[] sAry = System.Text.RegularExpressions.Regex.Split(sFileName, sCondition);

            if (sAry.Length > 1)
                return true;
            else
                return false;
        }

        //判斷檔案是否下載過
        public bool IsDownload(String sFileName, String sBakPath)
        {
            foreach (FileInfo filelist in new DirectoryInfo(sBakPath).GetFiles())
            {
                if (sFileName == filelist.Name)
                {
                    return true;
                }
            }
            return false;
        }

    }

    //2018.12.25 Arthur 新增 日期迴圈總集
    //日期迴圈總集
    public static class DateTimeHelper
    {
        public static IEnumerable<DateTime> EachSecond(DateTime from, DateTime thru)
        {
            from = Convert.ToDateTime(from.ToString("yyyy/MM/dd HH:mm:ss"));
            thru = Convert.ToDateTime(thru.ToString("yyyy/MM/dd HH:mm:ss"));

            for (var sec = from; sec <= thru; sec = sec.AddSeconds(1))
                yield return sec;
        }
        public static IEnumerable<DateTime> EachMinute(DateTime from, DateTime thru)
        {
            from = Convert.ToDateTime(from.ToString("yyyy/MM/dd HH:mm:00"));
            thru = Convert.ToDateTime(thru.ToString("yyyy/MM/dd HH:mm:00"));

            for (var min = from; min <= thru; min = min.AddMinutes(1))
                yield return min;
        }
        public static IEnumerable<DateTime> EachHour(DateTime from, DateTime thru)
        {
            from = Convert.ToDateTime(from.ToString("yyyy/MM/dd HH:00:00"));
            thru = Convert.ToDateTime(thru.ToString("yyyy/MM/dd HH:00:00"));

            for (var hour = from; hour <= thru; hour = hour.AddHours(1))
                yield return hour;
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        public static IEnumerable<DateTime> EachMonth(DateTime from, DateTime thru)
        {
            for (var month = from.Date; month.Date <= thru.Date || month.Month == thru.Month; month = month.AddMonths(1))
                yield return month;
        }
        public static IEnumerable<DateTime> EachYear(DateTime from, DateTime thru)
        {
            for (var year = from.Date; year.Date <= thru.Date || year.Year == thru.Year; year = year.AddYears(1))
                yield return year;
        }

        public static IEnumerable<DateTime> EachSecondTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachSecond(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachMinuteTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMinute(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachHourTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachHour(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }
        public static IEnumerable<DateTime> EachYearhTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachYear(dateFrom, dateTo);
        }
    }
}
