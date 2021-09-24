using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public  class SubAttributeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double Weightage { get; set; }
        public string Desciption { get; set; }
        public bool IsQuantity { get; set; }
        public int? UnitId { get; set; }
    }
}
