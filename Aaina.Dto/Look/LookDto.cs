using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Aaina.Dto
{
    public class LookDto
    {
        public LookDto()
        {
            this.LookAttribute = new List<LookAttributeDto>();
            this.LookGame = new List<LookGameDto>();
            this.LookPlayers = new List<LookPlayersDto>();
            this.LookSubAttribute = new List<LookSubAttributeDto>();
            this.TypeList = new List<SelectedItemDto>();
            this.GameList = new List<GameGridDto>();
            this.CalculatiotTypeList = new List<SelectedItemDto>();
            this.AttributeList = new List<AttributeDto>();
            this.AllRecord = new List<LookDto>();
            this.PlayerList = new List<GameUser>();
            this.ScheduleFrequencyList = new List<SelectedItemDto>();
            this.WeekDayList = new List<SelectedItemDto>();
            this.DailyFrequencyList = new List<SelectedItemDto>();
            this.MonthlyOccurrenceList = new List<SelectedItemDto>();
            this.OccursEveryTimeUnitList = new List<SelectedItemDto>();
            this.LookGroup = new List<LookGroupDto>();
            this.LookTeam = new List<LookTeamDto>();
            this.LookUser = new List<LookUserDto>();
            this.GroupPlayerList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Game { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CalculationType { get; set; }
        public bool IsSchedule { get; set; }
        public LookSchedulerDto LookScheduler { get; set; }
        public List<LookDto> AllRecord { get; set; }
        public List<SelectedItemDto> TypeList { get; set; }
        public List<SelectedItemDto> CalculatiotTypeList { get; set; }
        public List<SelectedItemDto> ScheduleFrequencyList { get; set; }
        public List<SelectedItemDto> WeekDayList { get; set; }
        public List<SelectedItemDto> OccursEveryTimeUnitList { get; set; }
        public List<SelectedItemDto> DailyFrequencyList { get; set; }
        public List<SelectedItemDto> MonthlyOccurrenceList { get; set; }
        public List<GameGridDto> GameList { get; set; }

        public List<SelectedItemDto> TeamList { get; set; }

        public List<GameGridDto> UserList { get; set; }
        public List<AttributeDto> AttributeList { get; set; }
        public List<LookAttributeDto> LookAttribute { get; set; }
        public List<LookGameDto> LookGame { get; set; }
        public List<LookPlayersDto> LookPlayers { get; set; }
        public List<LookSubAttributeDto> LookSubAttribute { get; set; }
        public List<GameUser> PlayerList { get; set; }
        public List<SelectedItemDto> GroupPlayerList { get; set; }
        public List<LookGroupDto> LookGroup { get; set; }
        public virtual List<LookTeamDto> LookTeam { get; set; }
        public virtual List<LookUserDto> LookUser { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class LookAttributeDto
    {
        public int? Id { get; set; }
        public int AttributeId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class LookGameDto
    {
        public int? Id { get; set; }
        public int GameId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class LookTeamDto
    {
        public int? Id { get; set; }
        public int TeamId { get; set; }
        public bool IsAdded { get; set; }
    }



    public class LookUserDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class LookGroupDto
    {
        public LookGroupDto()
        {
            this.LookGroupPlayer = new List<LookGroupPlayerDto>();
        }
        public int? Id { get; set; }
        public string Name { get; set; }

        public List<LookGroupPlayerDto> LookGroupPlayer { get; set; }
    }

    public class LookGroupPlayerDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class LookPlayersDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public bool IsView { get; set; }
        public bool IsCalculation { get; set; }
        public bool IsAdded { get; set; }
    }
    public partial class LookSubAttributeDto
    {
        public int? Id { get; set; }
        public int SubAttributeId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class GameUser
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public int UserTypeId { get; set; }
    }

    public class LookSchedulerDto
    {
        public LookSchedulerDto()
        {
            this.DaysOfWeekList = new List<int>();
        }
        public string Name { get; set; }
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
    }

    public class LookGridDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string IsActive { get; set; }
        public string Frequency { get; set; }
        public string CreatedBy { get; set; }
    }
}
