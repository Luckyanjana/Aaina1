using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class TeamDto
    {
        public TeamDto()
        {
            this.TeamPlayer = new List<TeamPlayerDto>();
            this.RoleList = new List<SelectedItemDto>();
            this.UserList = new List<SelectedItemDto>();
            this.AllForChart = new List<GameMenuDto>();
            this.GameList = new List<SelectedItemDto>();
            this.AllRecord = new List<TeamDto>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int? GameId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public double? Weightage { get; set; }
        public List<TeamPlayerDto> TeamPlayer { get; set; }
        public List<SelectedItemDto> GameList { get; set; }
        public List<SelectedItemDto> RoleList { get; set; }
        public List<SelectedItemDto> UserList { get; set; }
        public List<GameMenuDto> AllForChart { get; set; }
        public List<TeamDto> AllRecord { get; set; }
    }
    public partial class TeamPlayerDto
    {
        public int? Id { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public int TypeId { get; set; }
        public int? RoleId { get; set; }
        public bool IsAdded { get; set; }
    }
}
