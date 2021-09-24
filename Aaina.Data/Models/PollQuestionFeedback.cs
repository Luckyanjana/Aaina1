using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PollQuestionFeedback
    {
        public int Id { get; set; }
        public int PollFeedbackId { get; set; }
        public int PollQuestionId { get; set; }
        public int PollQuestionOptionId { get; set; }
        public string Remark { get; set; }

        public virtual PollFeedback PollFeedback { get; set; }
        public virtual PollQuestion PollQuestion { get; set; }
        public virtual PollQuestionOption PollQuestionOption { get; set; }
    }
}
