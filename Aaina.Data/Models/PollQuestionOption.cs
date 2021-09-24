using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class PollQuestionOption
    {
        public PollQuestionOption()
        {
            PollQuestionFeedback = new HashSet<PollQuestionFeedback>();
        }

        public int Id { get; set; }
        public int PollQuestionId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }

        public virtual PollQuestion PollQuestion { get; set; }
        public virtual ICollection<PollQuestionFeedback> PollQuestionFeedback { get; set; }
    }
}
