using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data.Models;
using Aaina.Dto;
using Aaina.Service;
using Aaina.Web.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Wkhtmltopdf.NetCore;

namespace Aaina.Web.Controllers
{
    public class SharedController : BaseController
    {
        private readonly IUserLoginService userService;
        private readonly IPlayService playService;
        private readonly ISessionService sessionService;
        private readonly IAttributeService attributeService;
        private readonly IReportService reportService;
        private readonly IStatusService statusService;
        private readonly ILookService lookService;

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        private readonly IGeneratePdf generatePdf;

        public SharedController(IUserLoginService _userService, IWebHostEnvironment _hostingEnvironment, IConfiguration _configuration, IGeneratePdf _generatePdf, 
            IPlayService playService, IAttributeService attributeService, IReportService reportService, IStatusService statusService, ILookService lookService,
            ISessionService sessionService)
        {
            this.userService = _userService;
            this.hostingEnvironment = _hostingEnvironment;
            this.configuration = _configuration;
            this.generatePdf = _generatePdf;
            this.playService = playService;
            this.sessionService = sessionService;
            this.attributeService = attributeService;
            this.reportService = reportService;
            this.statusService = statusService;
            this.lookService = lookService;
            FileOutputUtil.OutputDir = new DirectoryInfo(hostingEnvironment.WebRootPath + "/TempExcel");
        }

        [HttpPost]
        public async Task<ActionResult> GetDataTable(string ControllerName, string ActionName, string ReportName)
        {
            var model = this.MapParameterModel(this.HttpContext);
            var setting = new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentCulture,

                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateFormatString = "dd/MM/yyyy"
            };

            var dataTableResult = new GridResult();
            dataTableResult = await GetPaggedData(model);
            return this.Json(dataTableResult);
        }

        [HttpPost]
        public async Task<ActionResult> ExporttoExcel(string uiParameters)
        {
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);
            var dt = await CreateDataTable(dataTableParameter);
            
            var result = EpPlusExcelExporterBase.ExportExcel(dataTableParameter.ReportName, dt);
            return File(System.IO.File.ReadAllBytes(result.FullName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dataTableParameter.ReportName + ".xlsx");
        }

        [HttpPost]
        public async Task<ActionResult> ExporttoPDF(string uiParameters)
        {
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);
            var exceldata = await createHtml(dataTableParameter);
            string wholehtml = exceldata.ToString();
            var result = generatePdf.GetPDF(wholehtml);
            return this.File(result, "application/pdf", dataTableParameter.ReportName + ".pdf");

        }


        [HttpPost]
        public async Task<ActionResult> GridReport(string uiParameters)
        {
            //setLetterHeadValues();
            HtmltoPdfDto model = new HtmltoPdfDto();
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);
            GridResult dataTableResult = new GridResult();

            dataTableResult = await GetPaggedData(dataTableParameter);
            StringBuilder exceldata = new StringBuilder();


            model.ReportName = dataTableParameter.ReportName;
            DataTable dataTableFieldName = new DataTable();
            dataTableFieldName.Columns.Add("fieldname");
            dataTableFieldName.Columns.Add("fieldtype");


            exceldata.Append("<table class='reporttable'> " +
                          "<tr>");

            foreach (var item in dataTableParameter.Columns)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    exceldata.Append("<th>" + item.Title + "</th>");

                    DataRow dr = dataTableFieldName.NewRow();
                    dr["fieldname"] = item.Data;
                    dataTableFieldName.Rows.Add(dr);
                }
            }

            exceldata.Append("</tr><tbody>");

            var json = JsonConvert.SerializeObject(dataTableResult.Data);

            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            string fldname;
            foreach (DataRow dr in table.Rows)
            {
                exceldata.Append("<tr>");
                for (int col = 0; col < dataTableFieldName.Rows.Count; col++)
                {
                    fldname = dataTableFieldName.Rows[col]["fieldname"].ToString();
                    exceldata.Append("<td>" + dr[fldname].ToString() + "</td>");
                }
                exceldata.Append("</tr>");

            }
            exceldata.Append("</tbody></table>");

            model.Wholehtml = exceldata.ToString();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GridPrint(string uiParameters)
        {
            //setLetterHeadValues();
            HtmltoPdfDto model = new HtmltoPdfDto();
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);
            model.ReportName = dataTableParameter.ReportName;
            var exceldata = await createHtml(dataTableParameter);
            model.Wholehtml = exceldata.ToString();
            return Json(model);
        }

        [HttpPost]
        public async Task<ActionResult> ExporttoCSV(string uiParameters)
        {
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);

            StringBuilder csvdata = new StringBuilder();
            DataTable dataTableFieldName = new DataTable();
            dataTableFieldName.Columns.Add("fieldname");

            foreach (var item in dataTableParameter.Columns)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    csvdata.Append(item.Title + ",");
                    DataRow dr = dataTableFieldName.NewRow();
                    dr["fieldname"] = item.Data;
                    dataTableFieldName.Rows.Add(dr);
                }
            }

            GridResult dataTableResult = new GridResult();
            dataTableResult = await GetPaggedData(dataTableParameter);
            var json = JsonConvert.SerializeObject(dataTableResult.Data);
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            csvdata.Append('\r');
            string fldname;
            foreach (DataRow dr in table.Rows)
            {
                for (int col = 0; col < dataTableFieldName.Rows.Count; col++)
                {
                    fldname = dataTableFieldName.Rows[col]["fieldname"].ToString();
                    csvdata.Append(dr[fldname].ToString() + ",");
                }
                csvdata.Append('\r');

            }

            return File(System.Text.Encoding.ASCII.GetBytes(csvdata.ToString()), "application/csv", dataTableParameter.ReportName + ".csv");
        }

        [HttpPost]
        public ActionResult ReportAnalytics(string uiParameters)
        {
            JsonResponse jsonResponse = new JsonResponse();
            HttpContext.Session.SetString("uiParameters", uiParameters);
            //return Content("qqq");
            Response.Redirect(@"\Report\Analytics");
            return Json("");
        }

        [HttpPost]
        public async Task<string> GetReportData(string uiParameters)
        {
            uiParameters = HttpContext.Session.GetString("uiParameters").ToString();

            JsonResponse jsonResponse = new JsonResponse();
            GridParameterModel dataTableParameter = MapParameterModel(uiParameters);

            StringBuilder csvdata = new StringBuilder();
            DataTable dataTableFieldName = new DataTable();
            dataTableFieldName.Columns.Add("fieldname");

            foreach (var item in dataTableParameter.Columns)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    csvdata.Append(item.Title + ",");
                    DataRow dr = dataTableFieldName.NewRow();
                    dr["fieldname"] = item.Data;
                    dataTableFieldName.Rows.Add(dr);
                }
            }

            GridResult dataTableResult = new GridResult();
            dataTableResult = await GetPaggedData(dataTableParameter);
            var json = JsonConvert.SerializeObject(dataTableResult.Data);
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            csvdata.Append('\r');
            string fldname;
            foreach (DataRow dr in table.Rows)
            {
                for (int col = 0; col < dataTableFieldName.Rows.Count; col++)
                {
                    fldname = dataTableFieldName.Rows[col]["fieldname"].ToString();
                    csvdata.Append(dr[fldname].ToString() + ",");
                }
                csvdata.Append('\r');

            }
            return csvdata.ToString();
        }

        [HttpPost]
        public async Task<ActionResult> ExporttoHtml(string uiParameters)
        {
            GridParameterModel DataTableParameter = JsonConvert.DeserializeObject<GridParameterModel>(uiParameters);
            var exceldata = await createHtml(DataTableParameter);
            return File(System.Text.Encoding.ASCII.GetBytes(exceldata.ToString()), "application/html", DataTableParameter.ReportName + ".html");

        }

        private async Task<string> createHtml(GridParameterModel DataTableParameter)
        {
            GridResult dataTableResult = new GridResult();
            List<WeightageDto> emojiList = new List<WeightageDto>();

            if (DataTableParameter.Columns.Any(a => a.Name == "Emotion"))
            {
                var menu = PermissionHelper.GetPermission(CurrentUser.UserId);
                LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);
                emojiList = leftMenu.EmojiList;
            }

            DataTableParameter.Start = 0;
            DataTableParameter.Length = -1;

            dataTableResult = await GetPaggedData(DataTableParameter);

            DataTable dataTableFieldName = new DataTable();
            dataTableFieldName.Columns.Add("fieldname");

            StringBuilder exceldata = new StringBuilder();
            exceldata.Append("<table cellpadding='5' cellspacing='0' style='border-collapse:collapse;padding:5px;' border = '1px solid #ccc;'> " +
                          "<tr>");

            foreach (var item in DataTableParameter.Columns)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    exceldata.Append("<th style='background-color:#B8DBFD;border:1pxsolid #ccc'>" + item.Title + "</th>");

                    DataRow dr = dataTableFieldName.NewRow();
                    dr["fieldname"] = item.Data;
                    dataTableFieldName.Rows.Add(dr);
                }
            }

            exceldata.Append("</tr><tbody>");
            string fldname;


            var json = JsonConvert.SerializeObject(dataTableResult.Data);
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);


            foreach (DataRow dr in table.Rows)
            {
                exceldata.Append("<tr>");
                for (int col = 0; col < dataTableFieldName.Rows.Count; col++)
                {
                    fldname = dataTableFieldName.Rows[col]["fieldname"].ToString();
                    if (fldname == "Emotion")
                    {
                        exceldata.Append($"<td><img src='{SiteKeys.Domain}DYF/{CurrentUser.CompanyId}/EmojiImages/{GetEmojiNameMini(emojiList, int.Parse(dr[fldname].ToString()))}' class='imgemoji'></td>");
                    }
                    else
                    {
                        exceldata.Append("<td>" + dr[fldname].ToString() + "</td>");
                    }
                    
                }
                exceldata.Append("</tr>");

            }
            exceldata.Append("</tbody></table>");
            return exceldata.ToString();

        }

        private async Task<DataTable> CreateDataTable(GridParameterModel DataTableParameter)
        {
            DataTableParameter.Start = 0;
            DataTableParameter.Length = -1;

            GridResult dataTableResult = await GetPaggedData(DataTableParameter);

            string folder = "";
            List<WeightageDto> emojiList = new List<WeightageDto>();

            if (DataTableParameter.Columns.Any(a => a.Name == "Emotion"))
            {
                folder = $"{hostingEnvironment.WebRootPath}/DYF/{CurrentUser.CompanyId}/EmojiImages/";
                var menu = PermissionHelper.GetPermission(CurrentUser.UserId);
                LeftMenuDto leftMenu = JsonConvert.DeserializeObject<LeftMenuDto>(menu);
                emojiList = leftMenu.EmojiList;
            }

            DataTable dataTableFieldName = new DataTable();
            foreach (var item in DataTableParameter.Columns)
            {
                if (!string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Data))
                {
                    dataTableFieldName.Columns.Add(item.Title);
                }
            }

            var json = JsonConvert.SerializeObject(dataTableResult.Data);
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);


            foreach (DataRow dr in table.Rows)
            {
                List<object> row = new List<object>();
                foreach (var col in DataTableParameter.Columns)
                {
                    if (!string.IsNullOrEmpty(col.Title) && !string.IsNullOrEmpty(col.Data))
                    {
                        if(col.Data== "Emotion")
                        {
                            if (!string.IsNullOrEmpty(folder) && emojiList.Any())
                            {
                                var feeback = @$"{folder}{GetEmojiNameMini(emojiList, int.Parse(dr[col.Data].ToString()))}";
                                row.Add(feeback);
                            }
                            else
                            {
                                row.Add(null);
                            }
                        }
                        else
                        {
                            row.Add(dr[col.Data]);
                        }
                        
                        
                    }
                }

                dataTableFieldName.Rows.Add(row.ToArray());
            }
            return dataTableFieldName;

        }

        private async Task<GridResult> GetPaggedData(GridParameterModel DataTableParameter)
        {
            GridResult dataTableResult = new GridResult();
            

            if (DataTableParameter.ControllerName.ToLower() == "user" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await userService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "play" && DataTableParameter.ActionName.ToLower() == "index")
            {
                DataTableParameter.UserId = CurrentUser.UserId;

                dataTableResult = await playService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "session" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await sessionService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "attribute" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await attributeService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "report" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await reportService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "status" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await statusService.GetPaggedListAsync(DataTableParameter);
            }
            else if (DataTableParameter.ControllerName.ToLower() == "look" && DataTableParameter.ActionName.ToLower() == "index")
            {
                dataTableResult = await lookService.GetPaggedListAsync(DataTableParameter);
            }
            return dataTableResult;
        }

        private GridParameterModel MapParameterModel(HttpContext httpContext)
        {

            var request = httpContext.Request.Form;

            int draw = Convert.ToInt32(request["draw"]);
            int start = Convert.ToInt32(request["start"]);
            int length = Convert.ToInt32(request["length"]);


            var search = new GridSearch
            {
                Value = request["search[value]"],
                Regex = Convert.ToBoolean(request["search[regex]"])
            };


            var o = 0;
            var order = new List<GridOrder>();

            while (request["order[" + o + "][column]"].Count > 0)
            {
                order.Add(new GridOrder()
                {
                    Column = Convert.ToInt32(request["order[0][column]"].ToList()[0]),
                    Dir = request["order[" + o + "][dir]"]
                });
                o++;
            }


            // Columns
            var c = 0;
            var columns = new List<GridColumn>();
            while (request["columns[" + c + "][name]"].Count > 0)
            {
                columns.Add(new GridColumn
                {
                    Data = !string.IsNullOrEmpty(request["columns[" + c + "][data]"][0]) ? (char.ToUpper(request["columns[" + c + "][data]"][0][0]) + request["columns[" + c + "][data]"][0].Substring(1)) : request["columns[" + c + "][data]"][0],
                    Name = request["columns[" + c + "][name]"][0],
                    Orderable = Convert.ToBoolean(request["columns[" + c + "][orderable]"][0]),
                    Search = new GridSearch
                    {
                        Value = request["columns[" + c + "][search][value]"][0],
                        Regex = Convert.ToBoolean(request["columns[" + c + "][search][regex]"][0])
                    }
                });
                c++;
            }

            int GameId = 0;

            if (!string.IsNullOrEmpty(request["tenant"]))
            {
                GameId = int.Parse(request["tenant"]);
            }

            var mapData = new GridParameterModel
            {
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                Order = order,
                Columns = columns,
                ControllerName = request["controllerName"],
                ActionName = request["actionName"],
                ReportName = request["reportName"],
                GameId = GameId,
                CompanyId = CurrentUser.CompanyId,
                UserId = CurrentUser.UserId,
                UserType = CurrentUser.RoleId
            };


            if (!string.IsNullOrEmpty(request["parm1"]))
            {
                mapData.Parm1 = request["parm1"];
            }

            if (!string.IsNullOrEmpty(request["parm2"]))
            {
                mapData.Parm2 = request["parm2"];
            }

            if (!string.IsNullOrEmpty(request["parm3"]))
            {
                mapData.Parm3 = request["parm3"];
            }

            if (!string.IsNullOrEmpty(request["parm4"]))
            {
                mapData.Parm4 = request["parm4"];
            }

            if (!string.IsNullOrEmpty(request["parm5"]))
            {
                mapData.Parm5 = request["parm5"];
            }

            return mapData;
        }

        private GridParameterModel MapParameterModel(string uiParameters)
        {
            GridParameterModel dataTableParameter = JsonConvert.DeserializeObject<GridParameterModel>(uiParameters);
            dataTableParameter.CompanyId = CurrentUser.CompanyId;
            dataTableParameter.UserId = CurrentUser.UserId;
            dataTableParameter.UserType = CurrentUser.RoleId;
            dataTableParameter.Start = 0;
            dataTableParameter.Length = -1;
            if (dataTableParameter.Columns != null && dataTableParameter.Columns.Any())
            {
                dataTableParameter.Columns.ToList().ForEach(x =>
                {

                    x.Data = !string.IsNullOrEmpty(x.Data) ? (char.ToUpper(x.Data[0]) + x.Data.Substring(1)) : x.Data;
                });
            }
            return dataTableParameter;
        }

        private static string GetEmojiName(List<WeightageDto> emojiList, double rating)
        {
            string emoji = "1.png";
            emoji = emojiList.Any(a => a.Rating == rating) ? emojiList.FirstOrDefault(a => a.Rating == rating).Emoji : emoji;
            return emoji;
        }
        private static string GetEmojiNameMini(List<WeightageDto> emojiList, double rating)
        {
            string emoji = "1-mini.png";
            if (emojiList.Any(a => a.Rating == rating))
            {
                emoji = emojiList.FirstOrDefault(a => a.Rating == rating).Emoji;
                emoji = $"{emoji.Split('.')[0]}-mini.{emoji.Split('.')[1]}";
            }

            return emoji;
        }
    }
}