using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Aaina.Dto
{
    public class LookFeebbackDto
    {
        public LookFeebbackDto()
        {
            GameList = new List<LookFeebbackGameDto>();
            AttributeList = new List<LookFeebbackAttributeDto>();
            SubAttributeList = new List<LookFeebbackSubAttributeDto>();
            EmojiList = new List<SelectedItemDto>();
            LookList = new List<SelectedItemDto>();
            GameFeedbackDetails = new List<GameFeedbackDetailsDto>();
        }
        public int? Id { get; set; }
        public int? feedbackId { get; set; }
        public int? GameId { get; set; }
        public string Game { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CalculationType { get; set; }
        public List<LookFeebbackGameDto> GameList { get; set; }
        public List<LookFeebbackAttributeDto> AttributeList { get; set; }
        public List<LookFeebbackSubAttributeDto> SubAttributeList { get; set; }
        public List<SelectedItemDto> EmojiList { get; set; }
        public List<SelectedItemDto> LookList { get; set; }
        public List<GameFeedbackDetailsDto> GameFeedbackDetails { get; set; }
    }

    public class LookFeebbackGameDto
    {
        public LookFeebbackGameDto()
        {
            ChildGame = new List<LookFeebbackGameDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public List<LookFeebbackGameDto> ChildGame { get; set; }
    }

    public class LookFeebbackAttributeDto
    {
        public int AttributeId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }

    }

    public class LookFeebbackSubAttributeDto
    {
        public int AttributeId { get; set; }
        public int? Feedback { get; set; }
        public int SubAttributeId { get; set; }
        public string Name { get; set; }
        public string SubAttributeDesc { get; set; }
        public bool IsQuantity { get; set; }
        public string Unit { get; set; }
    }
}
