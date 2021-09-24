using System.Collections.Generic;


public class GridResult
{

    public int Draw { get; set; }

    public int RecordsTotal { get; set; }

    public int RecordsFiltered { get; set; }

    public string Error { get; set; }

    public List<object> Data { get; set; }

    public string CustomData { get; set; }
}