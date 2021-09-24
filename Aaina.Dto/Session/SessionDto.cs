using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class SessionDto
    {
        public SessionDto()
        {
            this.GameList = new List<SelectedItemDto>();
            this.TypeList = new List<SelectedItemDto>();
            this.ModeList = new List<SelectedItemDto>();
            this.PlayerList = new List<SelectedItemDto>();
            this.SessionParticipant = new List<SessionParticipantDto>();
            this.SessionReminder = new List<SessionReminderDto>();
            this.AllRecord = new List<SessionDto>();
        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedByStr { get; set; }
        public int? GameId { get; set; }
        public string Game { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int? TypeId { get; set; }
        public int? ModeId { get; set; }
        public bool IsActive { get; set; }
        public int? Deadline { get; set; }
        public List<SelectedItemDto> GameList { get; set; }
        public List<SelectedItemDto> TypeList { get; set; }
        public List<SelectedItemDto> ModeList { get; set; }
        public List<SelectedItemDto> PlayerList { get; set; }


        public List<SelectedItemDto> ParticipantTypeList { get; set; }
        public List<SelectedItemDto> NotificationsList { get; set; }
        public List<SelectedItemDto> NotificationsUnitList { get; set; }


        public List<SelectedItemDto> ScheduleFrequencyList { get; set; }
        public List<SelectedItemDto> WeekDayList { get; set; }
        public List<SelectedItemDto> OccursEveryTimeUnitList { get; set; }
        public List<SelectedItemDto> DailyFrequencyList { get; set; }
        public List<SelectedItemDto> MonthlyOccurrenceList { get; set; }

        public SessionSchedulerDto SessionScheduler { get; set; }
        public List<SessionParticipantDto> SessionParticipant { get; set; }
        public List<SessionReminderDto> SessionReminder { get; set; }

        public List<SessionDto> AllRecord { get; set; }

        public long NotificationId { get; set; }
        public int SessionId { get; set; }
        public string PendingMessage { get; set; }
        public int MessageStatus { get; set; }
    }

    public partial class SessionSchedulerDto
    {
        public SessionSchedulerDto()
        {
            this.DaysOfWeekList = new List<int>();
        }
        public string Venue { get; set; }
        public string MeetingUrl { get; set; }
        public byte Type { get; set; }
        public byte? Frequency { get; set; }
        public byte? RecurseEvery { get; set; }
        public string DaysOfWeek { get; set; }
        public List<int> DaysOfWeekList { get; set; }
        public byte? MonthlyOccurrence { get; set; }
        public byte? ExactDateOfMonth { get; set; }
        public byte? ExactWeekdayOfMonth { get; set; }
        public byte? ExactWeekdayOfMonthEvery { get; set; }
        public byte? DailyFrequency { get; set; }
        public TimeSpan? TimeStart { get; set; }
        public byte? OccursEveryValue { get; set; }
        public byte? OccursEveryTimeUnit { get; set; }
        public TimeSpan? TimeEnd { get; set; }
        public string ValidDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public string ColorCode { get; set; }
    }

    public class SessionParticipantDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int TypeId { get; set; }
        public int? ParticipantTyprId { get; set; }
        public bool IsAdded { get; set; }

    }

    public class SessionReminderDto
    {
        public int? Id { get; set; }
        public int? SessionId { get; set; }
        public int? TypeId { get; set; }
        public int? Every { get; set; }
        public int? Unit { get; set; }

    }

    public class SessionGridDto
    {
        
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CreatedDate { get; set; }
        public string GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string TypeId { get; set; }
        public string ModeId { get; set; }
        public string IsActive { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }


}
