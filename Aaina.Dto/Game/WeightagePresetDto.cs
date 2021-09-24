using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Aaina.Dto
{
    public class WeightagePresetDto
    {
        public WeightagePresetDto()
        {
            this.WeightagePresetDetails = new List<WeightagePresetDetailsDto>();
            this.GameList = new List<GameGridDto>();
            this.PlayerList = new List<SelectedItemDto>();
            this.PresetList = new List<SelectedItemDto>();
            this.RoleList = new List<RoleDto>();
        }
        public int GameId { get; set; }
        public int PresetId { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public bool IsDefault { get; set; }
        public List<RoleDto> RoleList { get; set; }
        public List<GameGridDto> GameList { get; set; }
        public List<SelectedItemDto> PlayerList { get; set; }
        public List<SelectedItemDto> PresetList { get; set; }
        public virtual List<WeightagePresetDetailsDto> WeightagePresetDetails { get; set; }
    }
    public class WeightagePresetDetailsDto
    {
        public int? GameId { get; set; }
        public int? RoleId { get; set; }
        public int UserId { get; set; }
        public double? Weightage { get; set; }
        public double? WeightageQuantity { get; set; }
        public string Role { get; set; }
        public string ColorCode { get; set; }
        public double? Percentage { get; set; }
    }
}
