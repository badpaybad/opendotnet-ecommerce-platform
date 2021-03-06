using System;
using System.Text.RegularExpressions;

namespace DomainDrivenDesign.Core.Utils
{
    public static class StringExtensions
    {
        static Random _random = new Random();
        private static string _alphabet = "qwertyuiopasdfghjklzxcvbnm";
        private static string _number = "1234567890";
        private static string _special = "~!@#$%^&*";

        public static string RandomString(short lenght)
        {
            if (lenght > 100 || lenght <= 0) throw new Exception("lenght must be between 0 - 100");
            var temp = string.Empty;
            var combin = _alphabet + _number;
            while (true)
            {
                if (temp.Length >= lenght) break;
                temp += combin[_random.Next(0, combin.Length)];
            }
            return temp;
        }

        public static string RandomPassword(short lenght)
        {
            if (lenght > 100 || lenght <= 0) throw new Exception("lenght must be between 0 - 100");
            var temp = string.Empty;
            var combin = _alphabet + _number+ _special;
            while (true)
            {
                if (temp.Length >= lenght) break;
                temp += combin[_random.Next(0, combin.Length)];
            }
            return temp;
        }

        private static string KillSign(string needReplace, string signUnicode, string replaceChar)
        {
            for (int i = 0; i < signUnicode.Length; i++)
            {
                //if(needReplace.IndexOf(signUnicode[i])>=0)
                needReplace = needReplace.Replace(signUnicode[i].ToString(), replaceChar);
            }
            return needReplace;
        }

        public static string KillSign(this string strUnicode)
        {
            strUnicode = strUnicode.ToLower();
            string a = "âăáàảãạấầẩẫậắằẳẵặ";
            string o = "ôơóòỏõọốồổỗộớờởỡợ";
            string e = "eéèẻẹẽêếềệểễ";
            string i = "iíìỉịĩ";
            string y = "yýỳỷỵỹ";
            string u = "uưúùủũụứừửữự";
            string d = "đ";
            //string special = "\\/*|:<>?\"-";

            int l = a.Length + o.Length + e.Length + i.Length + y.Length + u.Length + d.Length;
            if (strUnicode.Length > l * 3)
            {

                strUnicode = KillSign(strUnicode, a, "a");
                strUnicode = KillSign(strUnicode, o, "o");
                strUnicode = KillSign(strUnicode, e, "e");
                strUnicode = KillSign(strUnicode, i, "i");
                strUnicode = KillSign(strUnicode, y, "y");
                strUnicode = KillSign(strUnicode, u, "u");
                //strUnicode = KillSign(strUnicode, d, "d");
                // strUnicode = KillSign(strUnicode, special, "");
            }
            else
            {
                int ll = strUnicode.Length;
                for (int ii = 0; ii < ll; ii++)
                {
                    string temp = strUnicode.Substring(ii, 1);

                    if (a.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "a");
                    }
                    if (e.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "e");
                    }
                    if (o.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "o");
                    }
                    if (i.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "i");
                    }
                    if (y.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "y");
                    }
                    if (u.IndexOf(temp) >= 0)
                    {
                        strUnicode = strUnicode.Replace(temp, "u");
                    }
                }
            }

            return strUnicode.Replace("đ", "d");

        }

        private const string InvalidSegmentNames = @"[^a-zA-Z0-9\-]+";
        private static readonly Regex RegexInvalidSegmentNames = new Regex(InvalidSegmentNames, RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public static string ToUrlSegment(this string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            source = KillSign(source);

            var res = RegexInvalidSegmentNames.Replace(source, "-");
            while (res.IndexOf("--") >= 0)
            {
                res = res.Replace("--", "-");
            }
            return res;
        }

        static string _regexPatternEmail = "^[_a-z0-9-]+[._a-z0-9-]+@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z0-9-]+)$";

        public static bool IsValidEmail(this string src)
        {
            return Regex.IsMatch(src, _regexPatternEmail);
        }

    }
}