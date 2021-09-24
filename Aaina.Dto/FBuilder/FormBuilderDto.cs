using System;
using System.Collections.Generic;
using System.Text;


namespace Aaina.Dto
{
    public class FormBuilderDto
    {
        public FormBuilderDto()
        {
            this.FormBuilderAttribute = new List<FormBuilderAttributeDto>();
            this.DataTypeList = new List<SelectedItemDto>();
            this.DBColumnList = new List<SelectedItemDto>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsCoppied { get; set; }
        public int CopyId { get; set; }
        public List<SelectedItemDto> DataTypeList { get; set; }
        public List<SelectedItemDto> DBColumnList { get; set; }
        public List<FormBuilderAttributeDto> FormBuilderAttribute { get; set; }
    }

    public partial class FormBuilderAttributeDto
    {
        public FormBuilderAttributeDto()
        {
            this.FormBuilderAttributeLookUp = new List<FormBuilderAttributeLookUpDto>();
        }
        public int? Id { get; set; }
        public string AttributeName { get; set; }
        public int DataType { get; set; }
        public bool IsRequired { get; set; }
        public int OrderNo { get; set; }
        public List<FormBuilderAttributeLookUpDto> FormBuilderAttributeLookUp { get; set; }
        public string DbcolumnName { get; set; }
    }

    public class FormBuilderAttributeLookUpDto
    {
        public int? Id { get; set; }
        public string OptionName { get; set; }
    }
}
