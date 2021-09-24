using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class StatusFeedbackDetail
    {
        public int Id { get; set; }
        public int StatusFeedbackId { get; set; }
        public int GameId { get; set; }
        public int? SubGameId { get; set; }
        public DateTime FeedbackDate { get; set; }
        public string Feedback { get; set; }
        public int Status { get; set; }
        public int Progress { get; set; }
        public string Emotion { get; set; }

        public virtual Game Game { get; set; }
        public virtual StatusFeedback StatusFeedback { get; set; }
        public virtual Game SubGame { get; set; }
    }
}
