using System;
using System.Collections.Generic;
using DesignCrowd.Challenge.Models;
using DesignCrowd.Challenge.Services;
using Xunit;

namespace DesignCrowd.Challenge.Test
{
    public class BusinessDayCounterTests
    {
        private IBusinessDayCounter _businessDayCounter;
        private readonly List<DateTime> _publicHolidays;

        public BusinessDayCounterTests()
        {
            _businessDayCounter = new BusinessDayCounter();
            _publicHolidays = new List<DateTime> {
                new DateTime (2013, 12, 25),
                new DateTime (2013, 12, 26),
                new DateTime (2014, 1, 1)
            };
        }

        [Theory] // Test number of weekdays between two dates
        [InlineData("2013-10-7", "2013-10-9", 1)]
        [InlineData("2013-10-5", "2013-10-14", 5)]
        [InlineData("2013-10-7", "2014-1-1", 61)]
        [InlineData("2013-10-7", "2013-10-5", 0)]
        public void TestCountWeekdays(string firstDateString, string secondDateString, int weekDays)
        {
            var firstDate = DateTime.Parse(firstDateString);
            var secondDate = DateTime.Parse(secondDateString);

            int result = _businessDayCounter.WeekdaysBetweenTwoDates(firstDate, secondDate);

            Assert.Equal(weekDays, result);
        }

        [Theory] // Test number of business days between two dates
        [InlineData("2013-10-7", "2013-10-9", 1)]
        [InlineData("2013-12-24", "2013-12-27", 0)]
        [InlineData("2013-10-7", "2014-1-1", 59)]
        public void TestCountBusinessDays(string firstDateString, string secondDateString, int businessDays)
        {
            var firstDate = DateTime.Parse(firstDateString);
            var secondDate = DateTime.Parse(secondDateString);

            int result = _businessDayCounter.BusinessDaysBetweenTwoDates(firstDate, secondDate, _publicHolidays);

            Assert.Equal(businessDays, result);
        }

        [Theory]
        [InlineData("2020-6-15", "2020-6-28", 6, 20, false, 9)] // Test when fixed public holiday between dates and on weekend
        [InlineData("2013-10-7", "2013-10-9", 10, 8, false, 0)] // Test when fixed public holiday between dates and not on weekend
        [InlineData("2020-6-15", "2020-6-28", 6, 20, true, 8)] // Test when adjurnable public holiday between dates and on weekend
        [InlineData("2020-6-15", "2020-6-28", 6, 22, true, 8)] // Test when adjurnable public holiday between dates and not on weekend
        public void TestPublicHolidayFixedAndCanAdjurnRules(string firstDateString, string secondDateString, int month, int day, bool canAdjurn, int businessDays)
        {
            var firstDate = DateTime.Parse(firstDateString);
            var secondDate = DateTime.Parse(secondDateString);
            var rules = new List<PublicHolidayRule> { new PublicHolidayRule(month, day, canAdjurn) };

            int result = _businessDayCounter.BusinessDaysBetweenTwoDates(firstDate, secondDate, rules);

            Assert.Equal(businessDays, result);
        }

        [Theory]
        [InlineData("2020-6-15", "2020-6-28", DayOfWeek.Monday, Occurence.Second, 6, 9)] // Test when occurence public holiday not between dates
        [InlineData("2020-6-15", "2020-6-28", DayOfWeek.Monday, Occurence.Fourth, 6, 8)] // Test when occurence public holiday between dates
        public void TestPublicHolidayOccurenceRule(string firstDateString, string secondDateString, DayOfWeek dayOfWeek, Occurence occurence, int month, int businessDays)
        {
            var firstDate = DateTime.Parse(firstDateString);
            var secondDate = DateTime.Parse(secondDateString);
            var rules = new List<PublicHolidayRule> { new PublicHolidayRule(dayOfWeek, occurence, month) };

            int result = _businessDayCounter.BusinessDaysBetweenTwoDates(firstDate, secondDate, rules);

            Assert.Equal(businessDays, result);
        }

    }
}
