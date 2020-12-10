using System;

namespace DesignCrowd.Challenge.Models
{
    public class PublicHolidayRule
    {
        public string Name { get; set; }
        private DayOfWeek DayOfWeek { get; set; }
        private Occurence Occurence { get; set; }
        private int Day { get; set; }
        private int Month { get; set; }
        private bool CanAdjurn { get; set; }

        // Constructor for the first two rules, fixed day public holiday, and can adjurn public holiday which becomes monday if it falls on the weekend
        public PublicHolidayRule(int Month, int Day, bool CanAdjurn)
        {
            if (Day < 1 || Day > 31)
                throw new Exception("Invalid day");

            if (Month < 1 || Month > 12)
                throw new Exception("Invalid month");

            DateTime date;
            if (DateTime.TryParse($"2019-{Month}-{Day}", out date)) // Validate month and day with a non leap year
            {
                this.Day = Day;
                this.Month = Month;
            }
            else
            {
                throw new Exception("Invalid date");
            }

            this.CanAdjurn = CanAdjurn;
        }

        // Constructor for the third rule, occurence of a day of week in a specific month
        public PublicHolidayRule(DayOfWeek DayOfWeek, Occurence Occurence, int Month)
        {
            this.DayOfWeek = DayOfWeek;
            this.Occurence = Occurence;
            if (Month >= 1 && Month <= 12)
                this.Month = Month;
            else throw new Exception("Invalid month");
        }

        // Return the date of the occurence of a specific day of week
        private DateTime GetOccurenceOfDayInMonth(int year, int month, Occurence Occurrence, DayOfWeek Day)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var firstOccurenceOfDay = firstDayOfMonth.DayOfWeek == Day ? firstDayOfMonth : firstDayOfMonth.AddDays(Math.Abs(Day - firstDayOfMonth.DayOfWeek));
            return firstOccurenceOfDay.AddDays(7 * (int)Occurrence);
        }

        // Return the date of the public holiday in the providen year
        public DateTime GetYearPublicHolidayDate(int year)
        {
            // if day of week occurence rule
            if (this.Day == 0)
            {
                return this.GetOccurenceOfDayInMonth(year, this.Month, this.Occurence, this.DayOfWeek);
            }
            else // if fixed or can adjurn rule
            {
                var date = new DateTime(year, this.Month, this.Day);
                if (this.CanAdjurn)
                {
                    // if date falls on a weekend
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
                        date = date.AddDays(daysUntilMonday);
                    }
                }
                return date;
            }
        }
    }

    public enum Occurence
    {
        First = 0,
        Second = 1,
        Third = 2,
        Fourth = 3
    }
}
