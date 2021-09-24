using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class AttributeDto
    {
        public AttributeDto()
        {
            SubAttribute = new List<SubAttributeDto>();
            this.UnitList = new List<SelectListDto>();
        }

        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListDto> UnitList { get; set; }
        public List<SubAttributeDto> SubAttribute { get; set; }
    }

    public class AttributeGridDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public string IsActive { get; set; }
    }
}
