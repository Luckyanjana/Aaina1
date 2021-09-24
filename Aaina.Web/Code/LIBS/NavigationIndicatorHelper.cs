using Aaina.Common;
using Aaina.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aaina.Web.Code.LIBS
{
    public static class NavigationIndicatorHelper
    {
        public static string MakeActiveClass(this IUrlHelper urlHelper, string controller, string action)
        {
            try
            {
                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                {
                    if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static string MakeActiveClass(this IUrlHelper urlHelper, string controller, string[] action)
        {
            try
            {
                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                {
                    if (action.Contains(methodName.ToLower()))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static string MakeActiveClass(this IUrlHelper urlHelper, string[] controller)
        {
            try
            {
                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controller.Contains(controllerName.ToLower()))
                {
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static string BindSubRow(this IUrlHelper urlHelper, List<GameGridDto> dtoRow, GameGridFeedbackDto model, string extraSpace, string style, int index, int companyId)
        {

            int index1 = index;
            string extraSpace1 = "";
            string style1 = "style='display: none;'";
            StringBuilder sb = new StringBuilder();
            foreach (var row in dtoRow)
            {
                index++;
                var rowData = model.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);

                sb.Append($"<tr class='{row.ParentId}' {style} ><td><span class='serialnumber'></span></td><td>{row.Id} </td><td> {extraSpace} {row.Name} {(row.ChildGame.Any() ? " <a data-toggle='collapse' data-parent='accordion'  href='javascript:void(0)' id='" + row.Id + "'><span class='fa fa-plus-square mtx'></span> </a>" : "")} </td>");

                if (!model.IsSelf)
                {
                    var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='/DYF/{companyId}/EmojiImages/{GetEmojiName(model.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}' class='imgemoji'>") : "-";

                    var percetage = "";
                    if (rowData != null)
                    {
                        if (!rowData.IsWeighted)
                        {
                            percetage = "0";
                            if (rowData != null && rowData.Percentage > 0)
                            {
                                percetage = Math.Round(rowData.Percentage, MidpointRounding.AwayFromZero) + "%";
                            }

                        }
                    }
                    sb.Append($"<td class='text-center'>{feeback} {percetage}</td>");
                }

                foreach (var item in model.LookGroupList)
                {
                    var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                         (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='/DYF/{companyId}/EmojiImages/{GetEmojiName(model.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}'  class='imgemoji'>") : "-";
                    var percetage = "";
                    if (rowData != null)
                    {
                        if (!rowData.IsWeighted)
                        {
                            percetage = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Percentage > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Percentage, MidpointRounding.AwayFromZero) > 0 ? rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Percentage.ToString() : "0";
                            if (percetage != "0")
                            {
                                percetage = Math.Round(Convert.ToDecimal(percetage), MidpointRounding.AwayFromZero).ToString() + "%";
                            }
                        }
                    }
                    sb.Append($"<td class='text-center'>{feeback1} {percetage}</td>");
                }

                sb.Append($"</tr>");

                if (row.ChildGame.Any())
                {

                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                index1 = index;
                sb.Append(BindSubRow(urlHelper, row.ChildGame, model, extraSpace1, style1, index1, companyId));
            }
            return sb.ToString();


        }

        public static string BindSubRowWithPlus(this IUrlHelper urlHelper, List<GameGridDto> dtoRow, GameGridFeedbackDto model, string extraSpace, string style, int index, int companyId)
        {

            int index1 = index;
            string extraSpace1 = "";
            string style1 = "";
            StringBuilder sb = new StringBuilder();
            foreach (var row in dtoRow)
            {
                index++;
                var rowData = model.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);

                sb.Append($"<tr class='{row.ParentId}' {style} ><td><span class='serialnumber'></span></td><td>{row.Id} </td><td> {extraSpace} {row.Name} </td>");
                if (!model.IsSelf)
                {
                    var feeback = rowData != null && rowData.Feebback > 0 && Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero) > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='{SiteKeys.Domain}DYF/{companyId}/EmojiImages/{GetEmojiNameMini(model.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}' class='imgemoji'>") : "-";
                    var percetage = "";
                    if (rowData != null)
                    {
                        if (!rowData.IsWeighted)
                        {
                            percetage = "0";
                            if (rowData != null && rowData.Percentage > 0)
                            {
                                percetage = Math.Round(rowData.Percentage, MidpointRounding.AwayFromZero) + "%";
                            }

                        }
                    }
                    sb.Append($"<td class='text-center'>{feeback} {percetage}</td>");
                }
                foreach (var item in model.LookGroupList)
                {
                    var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero) > 0 ?
                         (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='{SiteKeys.Domain}/DYF/{companyId}/EmojiImages/{GetEmojiNameMini(model.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}'  class='imgemoji'>") : "-";
                    var percetage = "";
                    if (rowData != null)
                    {
                        if (!rowData.IsWeighted)
                        {
                            percetage = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Percentage > 0) && Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Percentage, MidpointRounding.AwayFromZero) > 0 ? rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Percentage.ToString() : "0";
                            if (percetage != "0")
                            {
                                percetage = Math.Round(Convert.ToDecimal(percetage), MidpointRounding.AwayFromZero).ToString() + "%";
                            }
                        }
                    }
                    sb.Append($"<td class='text-center'>{feeback1} {percetage}</td>");
                }

                sb.Append($"</tr>");

                if (row.ChildGame.Any())
                {

                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                index1 = index;
                sb.Append(BindSubRowWithPlus(urlHelper, row.ChildGame, model, extraSpace1, style1, index1, companyId));
            }
            return sb.ToString();


        }

        public static string BindSubRowUser(this IUrlHelper urlHelper, List<GameGridDto> dtoRow, GameGridFeedbackDto model, string extraSpace, string style, int index, int companyId)
        {

            int index1 = index;
            string extraSpace1 = "";
            string style1 = "style='display: none;'";
            StringBuilder sb = new StringBuilder();
            foreach (var row in dtoRow)
            {
                index++;
                var rowData = model.Feedbacks.FirstOrDefault(x => x.GameId == row.Id);
                sb.Append($"<tr class='{row.ParentId}' {style} > <td><input type='checkbox' class='game_check parent_{row.ParentId}' value='{row.Id}' /></td><td> {extraSpace} {row.Name} {(row.ChildGame.Any() ? " <a data-toggle='collapse' data-parent='accordion'  href='javascript:void(0)' id='" + row.Id + "'><span class='fa fa-plus-square mtx'></span> </a>" : "")} </td>");

                if (!model.IsSelf)
                {
                    var feeback = rowData != null && rowData.Feebback > 0 ? (rowData.IsQuantity ? $"{Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='/DYF/{companyId}/EmojiImages/{GetEmojiName(model.EmojiList, Math.Round(rowData.Feebback, MidpointRounding.AwayFromZero))}' class='imgemoji'>") : "-";

                    sb.Append($"<td class='text-center'>{feeback}</td>");
                }

                foreach (var item in model.LookGroupList)
                {
                    var feeback1 = rowData != null && rowData.Groups.Any() && rowData.Groups.Any(x => x.GroupId == item.Id && x.Feebback > 0) ?
                         (rowData.IsQuantity ? $"{Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero)}" : $"<img src='/DYF/{companyId}/EmojiImages/{GetEmojiName(model.EmojiList, Math.Round(rowData.Groups.FirstOrDefault(x => x.GroupId == item.Id).Feebback, MidpointRounding.AwayFromZero))}'  class='imgemoji'>") : "-";
                    sb.Append($"<td class='text-center'>{feeback1}</td>");
                }

                sb.Append($"</tr>");

                if (row.ChildGame.Any())
                {

                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                index1 = index;
                sb.Append(BindSubRowUser(urlHelper, row.ChildGame, model, extraSpace1, style1, index1, companyId));
            }
            return sb.ToString();


        }

        public static string BindLeftMenu(this IUrlHelper urlHelper, List<GameMenuDto> dtoRow, string controlleName, string actionName)
        {


            StringBuilder sb = new StringBuilder();
            foreach (var row in dtoRow)
            {
                if (row.ChildGame.Any())
                {
                    sb.Append($"<li><b class='caret' data-toggle='collapse' data-target='#submenu{row.Id}'></b><a href='/{row.Id}/{controlleName}/{actionName}' class='accordion-heading'><span class='nav-header-primary'>{row.Name}</span></a>");
                    sb.Append($"<ul class='nav nav-list collapse' id='submenu{row.Id}'>");
                    sb.Append(BindLeftMenuChild(urlHelper, row.ChildGame, controlleName, actionName));
                    sb.Append("</ul></li>");
                }
                else
                {
                    sb.Append($"<li><a href='/{row.Id}/{controlleName}/{actionName}'>{row.Name}</a></li>");
                }

            }
            return sb.ToString();


        }

        public static string BindLeftMenuChild(this IUrlHelper urlHelper, List<GameMenuDto> dtoRow, string controlleName, string actionName)
        {


            StringBuilder sb = new StringBuilder();
            foreach (var row in dtoRow)
            {
                if (row.ChildGame.Any())
                {
                    sb.Append($"<li><i class='fa fa-plus' data-toggle='collapse' data-target='#oneinner{row.Id}'></i> <a href='/{row.Id}/{controlleName}/{actionName}' class='accordion-heading'>{row.Name} </a>");
                    sb.Append($"<ul class='nav nav-list collapse' id='oneinner{row.Id}'>");
                    sb.Append(BindLeftMenuChild(urlHelper, row.ChildGame, controlleName, actionName));
                    sb.Append("</ul></li>");
                }
                else
                {
                    sb.Append($"<li><a href='/{row.Id}/{controlleName}/{actionName}'>{row.Name}</a></li>");
                }

            }
            return sb.ToString();


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
