using System;

namespace DIPS.AptSMS.ConfigClient.Common.Server.Extensions
{
    public static class DateTimeConverter
    {
        public static DateTime ConvertToUtc(DateTime localTime)
        {
            //return TimeZone.CurrentTimeZone.ToUniversalTime(localTime);
            return TimeZoneInfo.ConvertTimeToUtc(localTime);
        }
        public static DateTime ConvertToLocal(DateTime utcTime)
        {
            //return TimeZone.CurrentTimeZone.ToLocalTime(utcTime);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
        }
    }
}
