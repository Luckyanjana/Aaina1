using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class PreSessionDto
    {
        public PreSessionDto()
        {
            this.PreSessionAgenda = new List<PreSessionAgendaDto>();
            this.PreSessionParticipant = new List<PreSessionParticipantDto>();
            this.PreSessionAgendaList = new List<PreSessionAgendaDto>();


        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedByDate { get; set; }
        public List<PreSessionAgendaDto> PreSessionAgenda { get; set; }
        public List<PreSessionAgendaDto> PreSessionAgendaList { get; set; }
        public List<PreSessionParticipantDto> PreSessionParticipant { get; set; }
    }
    public class PreSessionAgendaDto
    {
        public PreSessionAgendaDto()
        {
            this.PreSessionAgendaDoc = new List<PreSessionAgendaDocDto>();
        }
        public int Id { get; set; }
        public int PlayId { get; set; }
        public int PreSessionId { get; set; }        
        public int Type { get; set; }
        public string TypeStr { get; set; }
        public int GameId { get; set; }
        public string Game { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccountableId { get; set; }
        public int? DependancyId { get; set; }
        public string Dependancy { get; set; }
        public int Priority { get; set; }
        public string Prioritystr { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int? FeedbackType { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public List<PreSessionAgendaDocDto> PreSessionAgendaDoc { get; set; }

    }

    public class PreSessionParticipantDto
    {
        public int Id { get; set; }
        public int PreSessionId { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int ParticipantTyprId { get; set; }
        public string ParticipantType { get; set; }
        public int Status { get; set; }
        public string StatusStr { get; set; }
        public string Remarks { get; set; }
    }

    public class PreSessionAgendaDocDto
    {
        public int Id { get; set; }
        public int PreSessionId { get; set; }
        public string FileName { get; set; }
    }

    public class PreSessionAgendaListDto
    {
        public PreSessionAgendaListDto()
        {
            this.PreSessionAgendaDoc = new List<PreSessionAgendaDocDto>();
        }
        public int Id { get; set; }
        public int PreSessionId { get; set; }
        public int SessionId { get; set; }
        public int GameId { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string SessionName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Dependancy { get; set; }
        public int Status { get; set; }
        public bool IsCoordinator { get; set; }
        public bool IsApproved { get; set; }
        public List<PreSessionAgendaDocDto> PreSessionAgendaDoc { get; set; }

    }
}
