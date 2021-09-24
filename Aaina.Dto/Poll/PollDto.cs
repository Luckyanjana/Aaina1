using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class PollDto
    {
        public PollDto()
        {
            this.PollParticipants = new List<PollParticipantsDto>();
            this.PollScheduler = new PollSchedulerDto();
            this.PollReminder = new List<PollReminderDto>();
            this.NotificationsList = new List<SelectedItemDto>();
            this.NotificationsUnitList = new List<SelectedItemDto>();
            this.ScheduleFrequencyList = new List<SelectedItemDto>();
            this.WeekDayList = new List<SelectedItemDto>();
            this.OccursEveryTimeUnitList = OccursEveryTimeUnitList;
            this.DailyFrequencyList = new List<SelectedItemDto>();
            this.MonthlyOccurrenceList = new List<SelectedItemDto>();
            this.AllRecord = new List<PollDto>();
            this.PollQuestion = new List<PollQuestionDto>();
            this.PollTypeList = new List<SelectedItemDto>();
            this.GameList = new List<SelectedItemDto>();
            this.SubGameList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int? GameId { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
       
        public bool IsActive { get; set; }
        public bool IsEditable { get; set; }
        public int CreatedBy { get; set; }
        public PollSchedulerDto PollScheduler { get; set; }
        public List<PollQuestionDto> PollQuestion { get; set; }
        public List<PollParticipantsDto> PollParticipants { get; set; }
        public List<PollReminderDto> PollReminder { get; set; }


        public List<GameUser> PlayerList { get; set; }
        public List<SelectedItemDto> GameList { get; set; }
        public List<SelectedItemDto> SubGameList { get; set; }
        public List<SelectedItemDto> NotificationsList { get; set; }
        public List<SelectedItemDto> NotificationsUnitList { get; set; }

        public List<SelectedItemDto> ScheduleFrequencyList { get; set; }
        public List<SelectedItemDto> WeekDayList { get; set; }
        public List<SelectedItemDto> OccursEveryTimeUnitList { get; set; }
        public List<SelectedItemDto> DailyFrequencyList { get; set; }
        public List<SelectedItemDto> PollTypeList { get; set; }
        public List<SelectedItemDto> MonthlyOccurrenceList { get; set; }
        public List<PollDto> AllRecord { get; set; }
    }


    public class PollSchedulerDto
    {
        public PollSchedulerDto()
        {
            this.DaysOfWeekList = new List<int>();
        }
        public int PollId { get; set; }
        public byte Type { get; set; }
        public byte? Frequency { get; set; }
        public byte? RecurseEvery { get; set; }
        public string DaysOfWeek { get; set; }
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
        public List<int> DaysOfWeekList { get; set; }
    }

    public class PollReminderDto
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int? TypeId { get; set; }
        public int? Every { get; set; }
        public int? Unit { get; set; }
    }

    public class PollParticipantsDto
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int? UserId { get; set; }
        public int TypeId { get; set; }
        public bool IsAdded { get; set; }
    }
    public class PollQuestionDto
    {
        public PollQuestionDto()
        {
            
            PollQuestionOption = new List<PollOptionDto>();
        }

        public int? Id { get; set; }
        public int QuestionTypeId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public List<PollOptionDto> PollQuestionOption { get; set; }
    }

    public class PollOptionDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public double Per { get; set; }
    }

    public class PollFeedbackDisplayDto
    {
        public PollFeedbackDisplayDto()
        {
            QuestionList = new List<PollQuestionDto>();
            this.ParticipantList = new List<SelectedItemDto>();
        }
        public int PollId { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Game { get; set; }
        public string SubGame { get; set; }        
        public List<PollQuestionDto> QuestionList { get; set; }
        public List<SelectedItemDto> ParticipantList { get; set; }
        public string Remark { get; set; }
    }

    public class PollFeedbackAddDto
    {
        public PollFeedbackAddDto()
        {
            QuestionList = new List<PollFeedbackAddQuestionDto>();
        }
        public int PollId { get; set; }
        public int UserId { get; set; }
        public List<PollFeedbackAddQuestionDto> QuestionList { get; set; }
        public string Remark { get; set; }
    }

    public class PollFeedbackAddQuestionDto
    {       
       
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string Remark { get; set; }
    }
}
