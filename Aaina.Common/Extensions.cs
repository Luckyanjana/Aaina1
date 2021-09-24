using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Aaina.Common
{
    public static class Extensions
    {

        public static DateTime? ToDateTime(this string source, string format)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                DateTime convertedDateTime;
                if (DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDateTime))
                {
                    return convertedDateTime;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static decimal GetRoundOff(this decimal val, int? place)
        {
            return decimal.Round(val, place.HasValue ? place.Value : 0);
        }


        public static decimal GetRoundCeiling(this decimal val)
        {
            return decimal.Ceiling(val);
        }

        #region "String"

        public static string ToSelfURL(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string outputStr = text.Trim().Replace(":", "").Replace("&", "").Replace(" ", "-").Replace("'", "").Replace(",", "").Replace("(", "").Replace(")", "").Replace("--", "").Replace(".", "");
            return Regex.Replace(outputStr.Trim().ToLower().Replace("--", ""), "[^a-zA-Z0-9_-]+", "", RegexOptions.Compiled);
        }

        public static string TrimLength(this string input, int length, bool Incomplete = true)
        {
            if (String.IsNullOrEmpty(input)) { return String.Empty; }
            return input.Length > length ? String.Concat(input.Substring(0, length), Incomplete ? "..." : "") : input;
        }

        public static string ToTitle(this string input)
        {
            return String.IsNullOrEmpty(input) ? String.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static bool ContainsAny(this string input, params string[] values)
        {
            return String.IsNullOrEmpty(input) ? false : values.Any(S => input.Contains(S));
        }

        public static string GetBucketName(this Guid instituteId)
        {
            return instituteId.ToString().Replace("-", string.Empty);
        }
        public static string GetAWSURL(this string keyName, Guid instituteId)
        {
            return $"https://{instituteId.GetBucketName()}.s3.amazonaws.com/{keyName}";
        }

        #endregion

        #region "Dictionary"

        public static void AddOrReplace(this IDictionary<string, object> DICT, string key, object value)
        {
            if (DICT.ContainsKey(key))
                DICT[key] = value;
            else
                DICT.Add(key, value);
        }

        public static dynamic GetObjectOrDefault(this IDictionary<string, object> DICT, string key)
        {
            if (DICT.ContainsKey(key))
                return DICT[key];
            else
                return null;
        }

        public static T GetObjectOrDefault<T>(this IDictionary<string, object> DICT, string key)
        {
            if (DICT.ContainsKey(key))
                return (T)Convert.ChangeType(DICT[key], typeof(T));
            else
                return default(T);
        }


        public static string GetEnumDescription(this Enum Enm)
        {
            FieldInfo fi = Enm.GetType().GetField(Enm.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return Enm.ToString();
            }
        }

        

        #endregion

        #region DateTime Formats..
        public static string ToFormatDateString(this DateTime? date, string format = "MM/dd/yyyy")
        {
            try
            {
                if (!date.HasValue)
                    return string.Empty;
                return date.Value.ToString(format);
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ToDateWithMonthString(this DateTime date)
        {
            try
            {

                return date.ToString("dd MMM yyyy");
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ToDateWithTimeString(this DateTime? date)
        {
            try
            {
                if (!date.HasValue)
                    return string.Empty;
                return date.Value.ToString("dd MMM yyyy hh:mm tt");
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ToFormatDateString(this DateTime date, string format = "dd/MM/yyyy")
        {
            try
            {
                return date.ToString(format);
            }
            catch
            {
                return string.Empty;
            }

        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public static DateTime ToFormatDateStringNew(string date, string format = "dd/MM/yyyy")
        {
            try
            {
                return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            }
            catch
            {

            }
            return new DateTime();

        }


        public static DateTime ToDateTime(this string str, bool isWithTime = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                return DateTime.Now;

            string[] formats = { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "dd/MM/yyyy h:mm:ss tt", "d/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-MM-dd h:mm:ss tt", "yyyy-M-dd h:mm:ss tt", "yyyy-MM-d h:mm:ss tt", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "dd-MM-yyyy h:mm:ss tt", "d-MM-yyyy h:mm:ss tt", "dd-M-yyyy h:mm:ss tt", "yyyy/MM/dd", "yyyy/M/dd", "yyyy/MM/d", "yyyy/MM/dd  h:mm:ss tt", "yyyy/M/dd  h:mm:ss tt", "yyyy/MM/d  h:mm:ss tt", "d/M/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "MM/d/yyyy h:mm:ss tt", "M/dd/yyyy", "MM/dd/yyyy", "MM/d/yyyy", "yyyyMMdd", "d/M/yy", "ddd,dd MMM yyyy" };
            if (isWithTime)
            {
                return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }

            return DateTime.ParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
        #endregion


        public static String ToConvertToWords(this decimal amount)
        {
            string numb = amount.ToString();
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents  
                        endStr = "Paisa " + endStr;//Cents  
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }



        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        private static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }

        private static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }
        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }

        public static string ReverseString(string Value)
        {
            char[] array = Value.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

    }
}
