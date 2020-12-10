using System;
using System.Collections.Generic;
using System.Linq;
using DesignCrowd.Challenge.Models;

namespace DesignCrowd.Challenge.Services
{
    public class BusinessDayCounter : IBusinessDayCounter
    {
        // Return if day is weekday
        private bool IsWeekday(DateTime day)
        {
            // Check if day is between monday and friday
            if ((int)day.DayOfWeek >= 1 && (int)day.DayOfWeek <= 5)
                return true;
            return false;
        }

        // Return list of weekday dates between two dates exclusive of the given dates
        private List<DateTime> GetWeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            List<DateTime> weekDays = new List<DateTime>();
            firstDate = firstDate.AddDays(1);

            while (firstDate < secondDate)
            {
                if (IsWeekday(firstDate))
                    weekDays.Add(firstDate);
                firstDate = firstDate.AddDays(1);
            }

            return weekDays;
        }

        // Return number of weekdays between two dates exclusive of the given dates
        public int WeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            var weekdays = GetWeekdaysBetweenTwoDates(firstDate, secondDate);
            return weekdays.Count();
        }

        // Return number of business days between two dates exclusive of the given dates and public holidays
        public int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays)
        {
            var weekdays = GetWeekdaysBetweenTwoDates(firstDate, secondDate);
            return weekdays.Count(x => !publicHolidays.Select(y => y.Date).Contains(x.Date));
        }

        // Return number of business days between two dates exclusive of the given dates following rules of public holidays
        public int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<PublicHolidayRule> publicHolidayRules)
        {
            var weekdays = GetWeekdaysBetweenTwoDates(firstDate, secondDate);
            IEnumerable<DateTime> publicHolidays = new List<DateTime>();
            while (firstDate.Year <= secondDate.Year)
            {
                // Get firstDate year list of public holidays
                var yearPublicHolidays = publicHolidayRules.Select(x => x.GetYearPublicHolidayDate(firstDate.Year)).ToList();
                // Create a list of all public hoidays
                publicHolidays = publicHolidays.Concat(yearPublicHolidays);
                firstDate = firstDate.AddYears(1);
            }
            return weekdays.Count(x => !publicHolidays.Select(y => y.Date).Contains(x.Date));
        }
    }

    public interface IBusinessDayCounter
    {
        // Return number of weekdays between two dates exclusive of the given dates
        int WeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate);
        // Return number of business days between two dates exclusive of the given dates and public holidays
        int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays);
        // Return number of business days between two dates exclusive of the given dates following rules of public holidays
        int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<PublicHolidayRule> publicHolidayRules);
    }
}
