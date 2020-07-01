using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.FormatFunction
{
    public static class TimeFormater
    {
       // T1 = 4,//DateTme format 01 :Friday, 29 May 2015 05:50
       // T2 = 5,//DateTme format 02 :05/29/2015 05:50
       // T3 = 6,//DateTme format 03 :May 29

        public static string GetFormat_T1(DateTime dateTime)
        {
            //DateTme format 01 :Friday, 29 May 2015 05:50            
            return dateTime.ToString("dddd, dd MMMM yyyy");
        }
        public static string GetFormat_T2(DateTime dateTime)
        {
            //DateTme format 02 :05/29/2015 05:50       
            return dateTime.ToString("MM/dd/yyyy HH:mm");
        }
        public static string GetFormat_T3(DateTime dateTime)
        {
            //DateTme format format 03 :May 29    
            return dateTime.ToString("MMMM dd");
        }
        public static string GetFormat_TX(DateTime dateTime,string format)
        {
            //DateTme format format 03 :May 29    
            return dateTime.ToString(format);
        }
        public static string GetFormat_T1(string dateTime)
        {
            //DateTme format 01 :Friday, 29 May 2015 05:50
            DateTime selectValue = GetTimeFromString(dateTime);
            return GetFormat_T1(selectValue);
        }
        public static string GetFormat_T2(string dateTime)
        {
            //DateTme format 01 :Friday, 29 May 2015 05:50
            DateTime selectValue = GetTimeFromString(dateTime);
            return GetFormat_T2(selectValue);
        }
        public static string GetFormat_T3(string dateTime)
        {
            //DateTme format 01 :Friday, 29 May 2015 05:50
            DateTime selectValue = GetTimeFromString(dateTime);
            return GetFormat_T3(selectValue);
        }
        public static string GetFormat_TX(string dateTime, string format)
        {
            //DateTme format 01 :Friday, 29 May 2015 05:50
            DateTime selectValue = GetTimeFromString(dateTime);
            return GetFormat_TX(selectValue, format);
        }
        private static DateTime GetTimeFromString(string timeValue)
        {
            // 2020 - 02 - 10T08: 00:00
            // 2020 - 02 - 17T09: 14:19
            DateTimeOffset dto = DateTimeOffset.Parse(timeValue);
            //Get the date object from the string. 
            return dto.DateTime;
        }
    }
}
