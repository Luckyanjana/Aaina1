using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class StatusScheduler
    {
        public int StatusId { get; set; }
        public string Venue { get; set; }
        public byte Type { get; set; }
        public byte? Frequency { get; set; }
        public byte? RecurseEvery { get; set; }
        public string DaysOfWeek { get; set; }
        public byte? MonthlyOccurrence { get; set; }
        public byte? ExactDateOfMonth { get; set; }
        public byte? ExactWeekdayOfMonth { get; set; }
        public byte? ExactWeekdayOfMonthEvery { get; set; }
        public byte? DailyFrequency { get; set; }
        public TimeSpan TimeStart { get; set; }
        public byte? OccursEveryValue { get; set; }
        public byte? OccursEveryTimeUnit { get; set; }
        public TimeSpan? TimeEnd { get; set; }
        public string ValidDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public string ColorCode { get; set; }

        public virtual Status Status { get; set; }
    }
}
