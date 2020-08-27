using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccessLayer.ExtensionMetod
{
    public static class Helpers
    {

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }
    }
}
