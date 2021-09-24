using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
   public class RoleDto
    {
        public RoleDto()
        {
            PlayerTypeList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public double? Weightage { get; set; }
        public string ColorCode { get; set; }
        public string Name { get; set; }        
        public string Desciption { get; set; }
        public bool IsActive { get; set; }
        public int PlayerType { get; set; }
        public List<SelectedItemDto> PlayerTypeList { get; set; }
    }
}
