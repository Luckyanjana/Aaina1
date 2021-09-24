using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PollQuestion
    {
        public PollQuestion()
        {
            PollQuestionFeedback = new HashSet<PollQuestionFeedback>();
            PollQuestionOption = new HashSet<PollQuestionOption>();
        }

        public int Id { get; set; }
        public int PollId { get; set; }
        public int QuestionTypeId { get; set; }
        public string Name { get; set; }

        public virtual Poll Poll { get; set; }
        public virtual ICollection<PollQuestionFeedback> PollQuestionFeedback { get; set; }
        public virtual ICollection<PollQuestionOption> PollQuestionOption { get; set; }
    }
}
