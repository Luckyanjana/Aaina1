using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class WeightageDto
    {
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public double? Rating { get; set; }
        public string Emoji { get; set; }
    }
}
