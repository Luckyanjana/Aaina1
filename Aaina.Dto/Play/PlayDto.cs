using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
  public  class PlayDto
    {
        public PlayDto()
        {
            GameList = new List<SelectedItemDto>();
            SubGameList = new List<SelectedItemDto>();
            AccountableList = new List<SelectedItemDto>();
            DependancyList = new List<SelectedItemDto>();
            PriorityList = new List<SelectListDto>();
            StatusList = new List<SelectListDto>();
            FeedbackList = new List<SelectListDto>();
            this.PersonInvolved = new List<int>();
            this.ParentList = new List<SelectedItemDto>();
            this.ChildList = new List<PlayDto>();
            this.PlayFeedBack = new List<PlayFeedbackDto>();
            this.EmojiList = new List<WeightageDto>();
           // this.Dependancy = new List<DayOfWeek>;
        }
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }

        public int UserId { get; set; }
        public int Type { get; set; }
        public int? GameId { get; set; }
        public string Game { get; set; }
        public string SubGame { get; set; }
        public int? SubGameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AccountableId { get; set; }
        public string Accountable { get; set; }
        public int? DependancyId { get; set; }
        public string Dependancy { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public DateTime? AddedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public double? Emotion { get; set; }
        public double? Phoemotion { get; set; }
        public int? FeedbackType { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Comments { get; set; }
        public bool IsToday { get; set; }
        public bool IsTActive { get; set; }
        public bool IsAgenda { get; set; }
        public List<int> PersonInvolved { get; set; }
        public string PersonInvolvedStr { get; set; }
        public int GameType { get; set; }
        public bool IsRequested { get; set; }


        public List<SelectedItemDto> GameList { get; set; }
        public List<SelectedItemDto> SubGameList { get; set; }
        public List<SelectedItemDto> AccountableList { get; set; }
        public List<SelectedItemDto> ParentList { get; set; }
        public List<SelectedItemDto> DependancyList { get; set; }
        public List<SelectListDto> FeedbackList { get; set; }
        public List<SelectListDto> PriorityList { get; set; }
        public List<SelectListDto> StatusList { get; set; }
        public List<PlayDto> ChildList { get; set; }
        public List<PlayFeedbackDto> PlayFeedBack { get; set; }
        public List<WeightageDto> EmojiList { get; set; }


    }

  public partial class PlayDelegateDto
    {
        public int PlayId { get; set; }
        public int StatusId { get; set; }
        public int AccountableId { get; set; }
        public int DelegateId { get; set; }
        public string Description { get; set; }

    }

    public partial class PlayMainDto
    {
        public PlayMainDto()
        {
            this.Result = new List<PlayDto>();
            this.GameList = new List<SelectListDto>();
            this.AccountableList = new List<SelectListDto>();
            this.PriorityList = new List<SelectListDto>();
            this.StatusList = new List<SelectListDto>();
            this.PlayGridDtoList = new List<PlayGridDto>();
        }
        public int? GameId { get; set; }
        public int? UserId { get; set; }
        public List<PlayDto> Result { get; set; }
        public List<SelectListDto> GameList { get; set; }
        public string ViewType { get; set; }

        public string ValueType { get; set; }
        public List<SelectListDto> AccountableList { get; set; }
        public List<SelectListDto> PriorityList { get; set; }
        public List<SelectListDto> StatusList { get; set; }
        public List<PlayGridDto> PlayGridDtoList { get; set; }
        
    }

    public class PlayGridDto
    {
        public int Id { get; set; }
        public string ParentId { get; set; }
        public string GameId { get; set; }
        public string SubGameId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AccountableId { get; set; }
        public int Accountable { get; set; }

        public string DependancyId { get; set; }
        
        public string Priority { get; set; }
        public string Status { get; set; }
        
        public string StartDate { get; set; }
        public string DeadlineDate { get; set; }
        public string Emotion { get; set; }
        public string Phoemotion { get; set; }
        
        public string ActualStartDate { get; set; }
        public string ActualEndDate { get; set; }

        public bool IsRequested { get; set; }

        public int Role { get; set; }
        public int GameType { get; set; }

        public string StatusPlay { get; set; }

    }

    public partial class PlayFeedbackDto
    {
        public int? PlayId { get; set; }
        public int? FeedbackStatus { get; set; }
        public int? FeedbackPriority { get; set; }
        public string Description { get; set; }
        public int? emoji { get; set; }
        public double? Percentage { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string Date { get; set; }

    }

}
