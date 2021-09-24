
using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class GameDto
    {
        public GameDto()
        {
            this.GamePlayers = new List<GamePlayerDto>();
            this.ParentList = new List<SelectedItemDto>();
            this.RoleList = new List<SelectedItemDto>();
            this.UserList = new List<SelectedItemDto>();
            this.AllForChart = new List<GameMenuDto>();
            this.GameLocation = new List<string>();
            this.AllGame = new List<GameDto>();
        }
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public double? Weightage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool IsActive { get; set; }
        public string ClientName { get; set; }
        public bool ApplyForChild { get; set; }
        public string Location { get; set; }
        public List<string> GameLocation { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public List<SelectedItemDto> ParentList { get; set; }
        public List<SelectedItemDto> RoleList { get; set; }
        public List<GamePlayerDto> GamePlayers { get; set; }
        public List<SelectedItemDto> UserList { get; set; }
        public List<GameMenuDto> AllForChart { get; set; }
        public List<GameDto> AllGame { get; set; }
    }

    public class GamePlayerDto
    {
        public int? Id { get; set; }
        public int GameId { get; set; }
        public int TypeId { get; set; }
        public int? RoleId { get; set; }
        public int UserId { get; set; }
        public bool IsAdded { get; set; }
    }

    public class GameFeedbackGridDto
    {
        public GameFeedbackGridDto()
        {
            this.Groups = new List<GameFeedbackGroupGridDto>();
        }
        public int GameId { get; set; }
        public double Feebback { get; set; }
        public double FeebbackQuantity { get; set; }
        public bool IsQuantity { get; set; }
        public bool IsWeighted { get; set; }
        
        public double Percentage { get; set; }
        public List<GameFeedbackGroupGridDto> Groups { get; set; }
    }
    public class GameFeedbackGroupGridDto
    {
        public string GroupId { get; set; }
        public double Feebback { get; set; }
        public double FeebbackQuantity { get; set; }
        public double Percentage { get; set; }

    }

    public class GameGridFeedbackDto
    {
        public GameGridFeedbackDto()
        {
            this.Feedbacks = new List<GameFeedbackGridDto>();
            this.AllGames = new List<GameGridDto>();
            this.LookList = new List<SelectedItemDto>();
            this.LookGroupList = new List<SelectedItemDto>();
            this.AttributeList = new List<SelectedItemDto>();
            this.FilterList = new List<SelectedItemDto>();
            this.EmojiList = new List<WeightageDto>();
            this.AttributeNames = new List<string>();
        }
        public bool IsSelf { get; set; }
        public int? LookId { get; set; }
        public int? AttributeId { get; set; }
        public int? FilterId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public int? PresetId { get; set; }
        public DateTime? FilterToDate { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public List<string> AttributeNames { get; set; }
        public string FilterName { get; set; }
        public List<SelectedItemDto> LookList { get; set; }
        public List<SelectedItemDto> AttributeList { get; set; }
        public List<SelectedItemDto> LookGroupList { get; set; }
        public List<SelectedItemDto> FilterList { get; set; }
        public List<GameFeedbackGridDto> Feedbacks { get; set; }
        public List<GameGridDto> AllGames { get; set; }
        public List<SelectedItemDto> UsersList { get; set; }
        public List<WeightageDto> EmojiList { get; set; }
    }

    public class GameGridDto
    {
        public GameGridDto()
        {
            this.ChildGame = new List<GameGridDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }
        public double Weightage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }

      
        public  List<GameGridDto> ChildGame { get; set; }
    }



    public class GameMenuDto
    {
        public GameMenuDto()
        {
            this.ChildGame = new List<GameMenuDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public virtual List<GameMenuDto> ChildGame { get; set; }
    }
    public class LeftMenuDto
    {
        public LeftMenuDto()
        {
            this.LeftMenu = new List<GameMenuDto>();
            this.EmojiList = new List<WeightageDto>();
            this.LeftMenuStatic = new List<MenuPermissionListDto>();
            this.LeftUserMenuStatic = new List<MenuPermissionListDto>();
        }
        public List<WeightageDto> EmojiList { get; set; }
        public  List<GameMenuDto> LeftMenu { get; set; }
        public List<MenuPermissionListDto> LeftMenuStatic { get; set; }
        public List<MenuPermissionListDto> LeftUserMenuStatic { get; set; }
    }
}
