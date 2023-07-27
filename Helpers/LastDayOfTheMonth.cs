using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Helpers
{

    public static class LastDayOfTheMonthExtension
    {
        public static bool IsLastDayOftheMonth(this string? dateTime, DateTime lastDayOfTheMonth) => DateTime.ParseExact(dateTime, "yyyyMMdd", null) <= lastDayOfTheMonth ? true : false;
        public static bool IsFutureDate(this string? dateTime) => DateTime.ParseExact(dateTime, "yyyyMMdd", null) > DateTime.Now.Date ? true : false;
        public static bool IsPastDate(this string? dateTime) => DateTime.ParseExact(dateTime, "yyyyMMdd", null) < DateTime.Now.Date ? true : false;

    }

}
