using System;

namespace DIPS.AptSMS.ConfigClient.Common.FormatFunction
{
    public static class TagFormater
    {
        /// <summary>
        /// Convert to any format from given value 
        /// </summary>
        /// <param name="format">format which asking to convert</param>
        /// <param name="Value">values should convert accordingly</param>
        /// <returns>formatted String will return</returns>
        public static string GetFormatString(string format, string Value)
        {
            try
            {
                var specialFormat = "";
                if (format.Trim().Length > 3)
                {
                    specialFormat = format.Trim().Substring(3);
                    format = format.Trim().Substring(0, 2);
                }
                var requestFormat = GetFormat(format);
                return GetFormatString(Value, requestFormat, specialFormat);
            }
            catch (Exception ex)
            {
                //log error
                return Value;
            }
        }
        public static string GetFormatString(string value , TagFormat format, string specialFormat)
        {
            switch (format)
            {
                case TagFormat.U:
                    return value.Trim().ToUpper();
                case TagFormat.CC:
                    return value;
                case TagFormat.PN:
                    return PersonHelper.NameFix(value);
                case TagFormat.PA:
                    return PersonHelper.AddressFix(value);
                case TagFormat.T1:
                    return TimeFormater.GetFormat_T1(value);
                case TagFormat.T2:
                    return TimeFormater.GetFormat_T2(value);
                case TagFormat.T3:
                    return TimeFormater.GetFormat_T3(value);
                case TagFormat.TX:
                    return TimeFormater.GetFormat_TX(value, specialFormat);
                default:
                    return value;
            }
        }
        private static TagFormat GetFormat(string value)
        {
            switch (value.Trim().ToUpper())
            {
                case "U":
                    return TagFormat.U;
                case "CC":
                    return TagFormat.CC;
                case "PN":
                    return TagFormat.PN;
                case "PA":
                    return TagFormat.PA;
                case "T1":
                    return TagFormat.T1;
                case "T2":
                    return TagFormat.T2;
                case "T3":
                    return TagFormat.T3;
                case "TX":
                    return TagFormat.TX;
                default:
                    return TagFormat.N;
            }
        }

    }
}
