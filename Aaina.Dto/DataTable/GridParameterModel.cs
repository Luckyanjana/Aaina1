using System;
using System.Collections.Generic;

public class GridParameterModel
{
   
    public int Draw { get; set; }

    public int Start { get; set; }

   
    public int Length { get; set; }

   
    public GridSearch Search { get; set; }

   
    public IEnumerable<GridOrder> Order { get; set; }

   
    public IEnumerable<GridColumn> Columns { get; set; }
       
  
    public int CompanyId { get; set; }
    public int? GameId { get; set; }
    public int UserId { get; set; }

    public int UserType { get; set; }

    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string ReportName { get; set; }
    public string Parm1 { get; set; }
    public string Parm2 { get; set; }
    public string Parm3 { get; set; }
    public string Parm4 { get; set; }
    public string Parm5 { get; set; }

}