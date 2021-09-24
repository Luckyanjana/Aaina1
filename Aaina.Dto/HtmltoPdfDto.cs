using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class HtmltoPdfDto
    {

        public string Wholehtml { get; set; }
        public string ReportName { get; set; }
        public bool IsLandscape { get; set; }
    }
}
