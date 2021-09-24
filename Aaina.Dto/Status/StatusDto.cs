using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class StatusDto
    {
        public StatusDto()
        {
            this.GameList = new List<SelectedItemDto>();
            this.ModeList = new List<SelectedItemDto>();
            this.PlayerList = new List<SelectedItemDto>();
            this.StatusReminder = new List<StatusReminderDto>();
            this.AllRecord = new List<StatusDto>();
            this.GameByIds = new List<int>();
            this.TeamByIds = new List<int>();
            this.UserByIds = new List<int>();
            this.TeamForIds = new List<int>();
            this.UserForIds = new List<int>();
            this.GameList = new List<SelectedItemDto>();
            this.TeamList = new List<SelectedItemDto>();
            this.UserList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public int? GameId { get; set; }
        public string Game { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int? StatusModeId { get; set; }
        public bool IsActive { get; set; }
        public int? EstimatedTime { get; set; }
        public List<int> GameByIds { get; set; }
        public List<int> TeamByIds { get; set; }
        public List<int> UserByIds { get; set; }
        public List<int> TeamForIds { get; set; }
        public List<int> UserForIds { get; set; }
        public List<SelectedItemDto> GameList { get; set; }
        public List<SelectedItemDto> ModeList { get; set; }
        public List<SelectedItemDto> PlayerList { get; set; }

        public List<SelectedItemDto> TeamList { get; set; }
        public List<SelectedItemDto> UserList { get; set; }
        public List<SelectedItemDto> NotificationsList { get; set; }
        public List<SelectedItemDto> NotificationsUnitList { get; set; }

        public List<SelectedItemDto> ScheduleFrequencyList { get; set; }
        public List<SelectedItemDto> WeekDayList { get; set; }
        public List<SelectedItemDto> OccursEveryTimeUnitList { get; set; }
        public List<SelectedItemDto> DailyFrequencyList { get; set; }
        public List<SelectedItemDto> MonthlyOccurrenceList { get; set; }

        public StatusSchedulerDto StatusScheduler { get; set; }

        public List<StatusReminderDto> StatusReminder { get; set; }

        public List<StatusDto> AllRecord { get; set; }
    }

    public partial class StatusSchedulerDto
    {
        public StatusSchedulerDto()
        {
            this.DaysOfWeekList = new List<int>();
        }
        public string Venue { get; set; }
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
    public class StatusReminderDto
    {
        public int? Id { get; set; }
        public int? StatusId { get; set; }
        public int? TypeId { get; set; }
        public int? Every { get; set; }
        public int? Unit { get; set; }

    }

    public class StatusFeedbackDto
    {
        public StatusFeedbackDto()
        {
            GameList = new List<SelectedItemDto>();
            StatusList = new List<SelectedItemDto>();
            this.TeamList = new List<SelectedItemDto>();
            this.UserList = new List<SelectedItemDto>();
            this.ModeList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public string By { get; set; }
        public string For { get; set; }
        public string Name { get; set; }
        public int? ActualTime { get; set; }
        public int? EstimatedTime { get; set; }
        public NonStatusDto Nonstatus { get; set; }
        public List<SelectedItemDto> GameList { get; set; }

        public List<SelectedItemDto> TeamList { get; set; }
        public List<SelectedItemDto> ModeList { get; set; }
        public List<SelectedItemDto> UserList { get; set; }
        public List<SelectedItemDto> StatusList { get; set; }

    }

    public class NonStatusDto
    {
        public NonStatusDto()
        {
            this.TeamForIds = new List<int>();
            this.UserForIds = new List<int>();
        }
        public int? CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public int? StatusModeId { get; set; }
        public int? EstimatedTime { get; set; }
        public List<int> TeamForIds { get; set; }
        public List<int> UserForIds { get; set; }

    }

    public class StatusFeedbackPostDto
    {
        public StatusFeedbackPostDto()
        {
            this.Feedback = new List<StatusFeedbackDetailPostDto>();
        }
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int? ActualTime { get; set; }
        public int? EstimatedTime { get; set; }
        public NonStatusDto Nonstatus { get; set; }
        public List<StatusFeedbackDetailPostDto> Feedback { get; set; }

    }

    public class StatusFeedbackDetailPostDto
    {

        public int GameId { get; set; }
        public int SubGameId { get; set; }
        public DateTime FeedbackDate { get; set; }
        public string Feedback { get; set; }
        public int Status { get; set; }
        public int Progress { get; set; }
        public int Emotion { get; set; }

    }

    public class StatusFeedbackReminderDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string MeetingUrl { get; set; }
    }

    public class StatusFeedbackDisplayDto
    {
        public StatusFeedbackDisplayDto()
        {
            this.Feedback = new List<StatusFeedbackDetailDisplayDto>();
        }
        public string UserName { get; set; }
        public int? ActualTime { get; set; }
        public int? EstimatedTime { get; set; }
        public List<StatusFeedbackDetailDisplayDto> Feedback { get; set; }

    }

    public class StatusFeedbackDetailDisplayDto
    {

        public string Game { get; set; }
        public string SubGame { get; set; }
        public DateTime FeedbackDate { get; set; }
        public string Feedback { get; set; }
        public int Status { get; set; }
        public int Progress { get; set; }
        public int Emotion { get; set; }

    }

    public class StatusGridDto
    {
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string StatusModeId { get; set; }
        public string IsActive { get; set; }

    }

}
