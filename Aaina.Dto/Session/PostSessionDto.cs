using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
  public  class PostSessionDto
    {
        public PostSessionDto()
        {
            this.PostSessionAgenda = new List<PostSessionAgendaDto>();
            this.PriorityList = new List<SelectListDto>();
            this.StatusList = new List<SelectListDto>();
            this.AccountableList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int? CompanyId { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int tenant { get; set; }
        public DateTime? ModifiedDate { get; set; }
        //public int tenant { get; set; }
        public List<PostSessionAgendaDto> PostSessionAgenda { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsCoordinator { get; set; }
        public bool IsDecisionMaker { get; set; }
        public List<SelectListDto> PriorityList { get; set; }
        public List<SelectListDto> StatusList { get; set; }
        public List<SelectedItemDto> AccountableList { get; set; }
    }

    public class PostSessionAgendaDto
    {
       
        public int Id { get; set; }
        public int PostSessionId { get; set; }
        public int Type { get; set; }
        public string TypeStr { get; set; }
        public int GameId { get; set; }
        public string Game { get; set; }
        public string SubGame { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccountableId { get; set; }
        
        public int? DependancyId { get; set; }
        public string Dependancy { get; set; }
        public string Accountable { get; set; }
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
        public string Remarks { get; set; }
        public int? Emotions { get; set; }
        public int? CoordinateEmotion { get; set; }
        public int? DecisionMakerEmotion { get; set; }
        public int PlayId { get; set; }

    }
}
