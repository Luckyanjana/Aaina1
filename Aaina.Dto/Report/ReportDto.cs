using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Aaina.Dto
{
    public class ReportDto
    {
        public ReportDto()
        {
            this.Entity = new List<ReportGameDto>();
            this.Scheduler = new ReportSchedulerDto();
            this.ReportJourney = new List<ReportParticipantDto>();
            this.Reminder = new List<ReportReminderDto>();
            this.ParticipantTypeList = new List<SelectedItemDto>();
            this.NotificationsList = new List<SelectedItemDto>();
            this.NotificationsUnitList = new List<SelectedItemDto>();
            this.ScheduleFrequencyList = new List<SelectedItemDto>();
            this.WeekDayList = new List<SelectedItemDto>();
            this.OccursEveryTimeUnitList = OccursEveryTimeUnitList;
            this.DailyFrequencyList = new List<SelectedItemDto>();
            this.MonthlyOccurrenceList = new List<SelectedItemDto>();
            this.AllRecord = new List<ReportDto>();
            this.TypeList = new List<SelectedItemDto>();
            this.AccountAbilityList = new List<SelectedItemDto>();
            this.FormBuilderList = new List<SelectedItemDto>();
            this.EntityIds = new List<int>();
            this.EntityTypeList = new List<SelectListDto>();
            this.EntityIdList = new List<SelectListDto>();
        }

        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int FormBuilderId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int TypeId { get; set; }
        public bool IsActive { get; set; }
        public int? EntityType { get; set; }
        public List<int> EntityIds { get; set; }
        public List<ReportGameDto> Entity { get; set; }
        public ReportSchedulerDto Scheduler { get; set; }
        public List<ReportParticipantDto> ReportJourney { get; set; }

        public List<ReportReminderDto> Reminder { get; set; }
        public List<GameUser> PlayerList { get; set; }


        public List<GameGridDto> GameList { get; set; }
        public List<SelectedItemDto> AccountAbilityList { get; set; }
        public List<SelectedItemDto> TypeList { get; set; }
        public List<SelectedItemDto> ParticipantTypeList { get; set; }
        public List<SelectedItemDto> NotificationsList { get; set; }
        public List<SelectedItemDto> NotificationsUnitList { get; set; }


        public List<SelectedItemDto> ScheduleFrequencyList { get; set; }
        public List<SelectedItemDto> WeekDayList { get; set; }
        public List<SelectedItemDto> OccursEveryTimeUnitList { get; set; }
        public List<SelectedItemDto> DailyFrequencyList { get; set; }
        public List<SelectedItemDto> FormBuilderList { get; set; }
        public List<SelectedItemDto> MonthlyOccurrenceList { get; set; }
        public List<ReportDto> AllRecord { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsCreator { get; set; }
        public List<SelectListDto> EntityTypeList { get; set; }
        public List<SelectListDto> EntityIdList { get; set; }
        public bool IsUpdateGive { get; set; }
        public bool IsView { get; set; }
    }

    public partial class ReportSchedulerDto
    {
        public ReportSchedulerDto()
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

    public class ReportGameDto
    {
        public int? Id { get; set; }
        public int GameId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class ReportParticipantDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int TypeId { get; set; }
        public int? AccountAbilityId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class ReportReminderDto
    {
        public int? Id { get; set; }
        public int? ReportId { get; set; }
        public int? TypeId { get; set; }
        public int? Every { get; set; }
        public int? Unit { get; set; }

    }

    public class ReportGiveDto
    {
        public ReportGiveDto()
        {
            this.Attribute = new List<FormBuilderAttributeValueDto>();
            this.UserList = new List<SelectedItemDto>();
        }

        public int? Id { get; set; }
        public int ReportId { get; set; }
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string Game { get; set; }
        public int TypeId { get; set; }
        public FormBuilderDto FormBuilder { get; set; }
        public List<SelectedItemDto> UserList { get; set; }
        public List<FormBuilderAttributeValueDto> Attribute { get; set; }
    }

    public class ReportGiveSaveDto
    {
        public ReportGiveSaveDto()
        {
            this.Details = new List<ReportGiveSaveDetailsDto>();
            this.FormBuilderAttribute = new List<FormBuilderAttributeDto>();
            this.Accountability = new List<SelectedItemDto>();
        }

        public int? Id { get; set; }
        public int ReportId { get; set; }

        public bool IsDuplicate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsManual { get; set; }
        public int UserId { get; set; }
        public List<ReportGiveSaveDetailsDto> Details { get; set; }
        public List<FormBuilderAttributeDto> FormBuilderAttribute { get; set; }
        public List<SelectedItemDto> Accountability { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public DateTime GiveDate { get; set; }
        public string Game { get; set; }
        public bool IsReject { get; set; }
    }
    public class ReportGiveSaveDetailsDto
    {
        public ReportGiveSaveDetailsDto()
        {
            this.Attribute = new List<FormBuilderAttributeValueDto>();
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int? EntityId { get; set; }
        public List<FormBuilderAttributeValueDto> Attribute { get; set; }
        public string Remark { get; set; }
    }
    public class ReportGiveResultDto
    {
        public ReportGiveResultDto()
        {
            Result = new List<ReportGiveDetailsDto>();
        }

        public string Name { get; set; }
        public string Desciption { get; set; }
        public string Game { get; set; }

        public List<ReportGiveDetailsDto> Result { get; set; }
    }
    public class ReportGiveDetailsDto
    {
        public string UserName { get; set; }
        public List<ReportGiveAttributeDto> Attribute { get; set; }
    }

    public class FormBuilderAttributeValueDto
    {
        public int Id { get; set; }
        public int ReportGiveId { get; set; }
        public int FormBuilderAttributeId { get; set; }
        public string FormBuilderAttribute { get; set; }
        public string AttributeValue { get; set; }
        public int? LookUpId { get; set; }
        public string LookUp { get; set; }
    }

    public class ReportGiveAttributeDto
    {
        public string Attribute { get; set; }
        public string AttributeValue { get; set; }
    }

    public class ReportGridDto
    {       

        public int Id { get; set; }        
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string TypeId { get; set; }
        public string IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsCreator { get; set; }
        public bool IsUpdateGive { get; set; }
        public bool IsView { get; set; }
    }
}
