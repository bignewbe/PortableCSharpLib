using System;
using System.Collections.Generic;

namespace PortableCSharpLib.Facility
{
    public static class DateTimeOperation
    {
        static DateTimeOperation() { PortableCSharpLib.General.CheckDateTime(); }

        public static List<DateTime> ListMarketHolidays = new List<DateTime>(new DateTime[] {
            DateTime.Parse("01/01/2015"),
            DateTime.Parse("01/19/2015"),
            DateTime.Parse("02/16/2015"),
            DateTime.Parse("04/03/2015"),
            DateTime.Parse("05/25/2015"),
            DateTime.Parse("07/03/2015"),
            DateTime.Parse("09/07/2015"),
            DateTime.Parse("11/26/2015"),
            DateTime.Parse("12/25/2015"),
        });

        //input date: 20131129, input time: 20:30:40
        public static DateTime ConvertStringToDataTime(string date, string time)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));
            int day = Convert.ToInt32(date.Substring(6, 2));

            int hours = Convert.ToInt32(time.Substring(0, 2));
            int mins = Convert.ToInt32(time.Substring(2, 2));
            int sec = Convert.ToInt32(time.Substring(4, 2));

            DateTime dateTime = new DateTime(year, month, day, hours, mins, sec, DateTimeKind.Utc);
            return dateTime;
        }
        public static string ConvertDateTimeToString(DateTime dateTime)
        {
            return string.Format("{0,4}{1,2}{2,2}{3,2}{4,2}{5,2}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
        public static DateTime GetCurrentEasternTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }

        public static DateTime GetNextTradingDay(DateTime date)
        {
            var nextTradingDay = date.AddDays(1);
            while (!IsTradingDay(nextTradingDay))
                nextTradingDay = nextTradingDay.AddDays(1);

            return nextTradingDay;
        }
        //return the previous latest trading day
        public static DateTime GetLatestTradingDay()
        {
            var latestTradeDay = GetCurrentEasternTime().Date;
            while (!IsTradingDay(latestTradeDay))
                latestTradeDay = latestTradeDay.AddDays(-1);

            return latestTradeDay;
        }
        //check if the given date is a trading day
        public static bool IsTradingDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday ||
                ListMarketHolidays.Contains(date.Date))
                return false;
            else
                return true;
        }

        /// <summary>
        /// check if two trading days are consecutive. the precondition is that the given days must be trading days. 
        /// </summary>
        /// <param name="lastTradeDate"></param>
        /// <param name="currentTradeDate"></param>
        /// <returns></returns>
        public static int CheckConsecutiveTradingDays(DateTime lastTradeDate, DateTime currentTradeDate)
        {
            if (lastTradeDate.Date == currentTradeDate.Date)   //same day
                return 0;

            if ((currentTradeDate.Date - lastTradeDate.Date).TotalDays == 1)
                return 1;                           //consecutive trading day

            if ((currentTradeDate.Date - lastTradeDate.Date).TotalDays == 3 &&
                lastTradeDate.DayOfWeek == DayOfWeek.Friday &&
                currentTradeDate.DayOfWeek == DayOfWeek.Monday)
                return 1;                           //consecutive trading day with weekend in between

            if ((currentTradeDate.Date - lastTradeDate.Date).TotalDays == 2 &&
                ListMarketHolidays.Contains(lastTradeDate.Date.AddDays(-1)))
                return 1;                           //consecutive trading day with one holiday day in between

            if ((currentTradeDate.Date - lastTradeDate.Date).TotalDays == 4) //with weekend and one close day in between
            {
                if (lastTradeDate.DayOfWeek == DayOfWeek.Thursday && ListMarketHolidays.Contains(lastTradeDate.Date.AddDays(1)) ||      //Fri Sat Sun
                    currentTradeDate.DayOfWeek == DayOfWeek.Tuesday && ListMarketHolidays.Contains(currentTradeDate.Date.AddDays(-1)))  //Sat Sun Mon 
                    return 1;                       //consecutive trading day with weekend and one close day in between
            }

            return 2;                               //there are gaps between two trading days
        }

    }
}
