using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aaina.Dto
{
  public  class FilterDto
    {
        public FilterDto()
        {
            AttributeIds = new List<int>();
            ForIds = new List<int>();
            this.FromIds = new List<int>();
            this.EmotionsForList = new List<SelectListDto>();
            this.AttributeList = new List<SelectListDto>();
            this.EmotionsFromList = new List<SelectListDto>();
            this.Players = new List<FilterPlayersDto>();
            this.AllRecord = new List<FilterDto>();
            this.CalculatiotTypeList = new List<SelectListDto>();
            this.EmotionsFromTypeList = new List<SelectListDto>();
            this.EmotionsForTypeList = new List<SelectListDto>();
            this.FromPIds = new List<int>();
            this.FromIds = new List<int>();
            this.ForIds = new List<int>();
            this.AttributeIds = new List<int>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? EmotionsFor { get; set; }
        public int? EmotionsFrom { get; set; }
        public int? EmotionsFromP { get; set; }
        public int? CalculationType { get; set; }
        public bool IsSelf { get; set; }
        public List<int> AttributeIds { get; set; }
        public List<int> ForIds { get; set; }
        public List<int> FromIds { get; set; }
        public List<int> FromPIds { get; set; }
        public List<FilterPlayersDto> Players { get; set; }
        public List<SelectListDto> EmotionsForList { get; set; }
        public List<SelectListDto> EmotionsFromList { get; set; }
        public List<SelectListDto> EmotionsFromPList { get; set; }
        public List<SelectListDto> AttributeList { get; set; }
        public List<SelectListDto> PlayerListList { get; set; }
        public List<FilterDto> AllRecord { get; set; }
        public List<SelectListDto> CalculatiotTypeList { get; set; }
        public List<SelectListDto> EmotionsFromTypeList { get; set; }
        public List<SelectListDto> EmotionsForTypeList { get; set; }
        public int GameId { get; set; }
    }

    public class FilterPlayersDto
    {        
        public int UserId { get; set; }
        public bool IsView { get; set; }
        public bool IsCalculation { get; set; }
    }

}
