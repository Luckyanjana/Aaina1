using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class GameFeedbackDto
    {
        public GameFeedbackDto()
        {
            this.GameFeedbackDetails = new List<GameFeedbackDetailsDto>();
        }
        public int? Id { get; set; }
        public int GameId { get; set; }
        public int TypeId { get; set; }
        public int CompanyId { get; set; }
        public int LookId { get; set; }
        public int UserId { get; set; }
        public bool IsDraft { get; set; }

        public virtual List<GameFeedbackDetailsDto> GameFeedbackDetails { get; set; }
    }

    public partial class GameFeedbackDetailsDto
    {
        public int? Id { get; set; }        
        public int? GameId { get; set; }
        public int? AttributeId { get; set; }
        public int? SubAttributeId { get; set; }
        public int? Feedback { get; set; }
        public double? Quantity { get; set; }
        public double? Percentage { get; set; }
    }
    }
