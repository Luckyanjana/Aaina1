using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Data;
using Aaina.Data.Models;
using Aaina.Data.Repositories;
using Aaina.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aaina.Service
{
    public class LookService : ILookService
    {
        private IRepository<Look, int> repo;
        private IRepository<Game, int> gameRepo;
        private IRepository<LookAttribute, int> repoAttribute;
        private IRepository<LookGame, int> repoLookGame;
        private IRepository<LookPlayers, int> repoPlayers;
        private IRepository<LookSubAttribute, int> repoSubAttribute;
        private IRepository<SubAttribute, int> repoAtSubAttribute;
        private IRepository<Data.Models.Attribute, int> repoAtAttribute;
        private IRepository<LookScheduler, int> repoScheduler;
        private IRepository<LookGroup, int> repoGroup;
        private IRepository<LookGroupPlayer, int> repoGroupPlayer;
        private IRepository<LookTeam, int> repoTeam;
        private IRepository<LookUser, int> repoUser;
        private IRepository<UserLogin, int> repoUserLogin;
        private IRepository<Team, int> repo_Team;
        private IRepository<TeamPlayer, int> repo_TeamPlayer;
        private IRepository<GameFeedback, int> repoGameFeedback;
        private IRepository<GameFeedbackDetails, int> repoGameFeedbackDetails;
        private IRepository<TeamFeedback, int> repoTeamFeedback;
        private IRepository<TeamFeedbackDetails, int> repoTeamFeedbackDetails;

        private IRepository<UserFeedback, int> repoUserFeedback;
        private IRepository<UserFeedbackDetails, int> repoUserFeedbackDetails;

        private IRepository<WeightagePreset, int> repoWeightagePreset;
        private IRepository<WeightagePresetDetails, int> repoWeightagePresetDetails;
        private IRepository<Filter, int> repoFilter;
        private IRepository<FilterEmotionsFor, int> repoFilterEmotionsFor;
        private IRepository<FilterEmotionsFrom, int> repoFilterEmotionsFrom;
        private IRepository<FilterEmotionsFromP, int> repoFilterEmotionsFromP;
        private IRepository<FilterAttributes, int> repoFilterAttributes;
        private IRepository<FilterPlayers, int> repoFilterFilterPlayers;
        private IRepository<Role, int> repoRole;
        private IGameService gameService;
        private readonly IConfiguration configuration;
        private readonly string Connectionstring;
        public LookService(IConfiguration configuration, IRepository<Look, int> repo, IRepository<Game, int> gameRepo, IRepository<LookAttribute, int> repoAttribute,
            IRepository<LookGame, int> repoGame, IRepository<LookPlayers, int> repoPlayers,
            IRepository<LookSubAttribute, int> repoSubAttribute, IRepository<LookScheduler, int> repoScheduler, IRepository<LookGroup, int> repoGroup,
            IRepository<LookTeam, int> repoTeam, IRepository<LookUser, int> repoUser, IRepository<LookGroupPlayer, int> repoGroupPlayer, IRepository<GameFeedback, int> repoGameFeedback,
            IRepository<GameFeedbackDetails, int> repoGameFeedbackDetails, IRepository<WeightagePreset, int> repoWeightagePreset,
            IRepository<WeightagePresetDetails, int> repoWeightagePresetDetails, IRepository<Filter, int> repoFilter, IRepository<TeamFeedback, int> repoTeamFeedback,
            IRepository<TeamFeedbackDetails, int> repoTeamFeedbackDetails,
            IRepository<UserFeedback, int> repoUserFeedback, IRepository<UserFeedbackDetails, int> repoUserFeedbackDetails, IRepository<FilterEmotionsFor, int> repoFilterEmotionsFor,
            IRepository<FilterAttributes, int> repoFilterAttributes, IRepository<FilterEmotionsFrom, int> repoFilterEmotionsFrom,
            IRepository<FilterEmotionsFromP, int> repoFilterEmotionsFromP, IRepository<FilterPlayers, int> repoFilterFilterPlayers, IRepository<Team, int> repo_Team,
            IRepository<TeamPlayer, int> repo_TeamPlayer, IRepository<UserLogin, int> repoUserLogin, IRepository<Role, int> repoRole, IGameService gameService, IRepository<SubAttribute, int> repoAtSubAttribute, IRepository<Data.Models.Attribute, int> repoAtAttribute)
        {
            this.configuration = configuration;
            this.Connectionstring = this.configuration.GetConnectionString("AainaDb");

            this.repo = repo;
            this.gameRepo = gameRepo;
            this.repoAttribute = repoAttribute;
            this.repoLookGame = repoGame;
            this.repoPlayers = repoPlayers;
            this.repoSubAttribute = repoSubAttribute;
            this.repoScheduler = repoScheduler;
            this.repoGroup = repoGroup;
            this.repoTeam = repoTeam;
            this.repoUser = repoUser;
            this.repoGroupPlayer = repoGroupPlayer;
            this.repoGameFeedback = repoGameFeedback;
            this.repoGameFeedbackDetails = repoGameFeedbackDetails;
            this.repoWeightagePreset = repoWeightagePreset;
            this.repoWeightagePresetDetails = repoWeightagePresetDetails;
            this.repoFilter = repoFilter;
            this.repoFilterEmotionsFor = repoFilterEmotionsFor;
            this.repoFilterAttributes = repoFilterAttributes;
            this.repoTeamFeedback = repoTeamFeedback;
            this.repoTeamFeedbackDetails = repoTeamFeedbackDetails;
            this.repoUserFeedback = repoUserFeedback;
            this.repoUserFeedbackDetails = repoUserFeedbackDetails;
            this.repoFilterEmotionsFrom = repoFilterEmotionsFrom;
            this.repoFilterEmotionsFromP = repoFilterEmotionsFromP;
            this.repoFilterFilterPlayers = repoFilterFilterPlayers;
            this.repo_Team = repo_Team;
            this.repo_TeamPlayer = repo_TeamPlayer;
            this.repoUserLogin = repoUserLogin;
            this.repoRole = repoRole;
            this.gameService = gameService;
            this.repoAtSubAttribute = repoAtSubAttribute;
            this.repoAtAttribute = repoAtAttribute;
        }

        public async Task<Tuple<List<GameFeedbackGridDto>, List<string>, string>> GetGameFeedback(int gId, int? lookid, int? presetId, int? attributeId,
            int? filterId, DateTime filterFromDate, DateTime filterToDate)
        {

            List<string> AtterbuteNameList = new List<string>();
            string filterName = string.Empty;
            var x = this.repo.GetIncludingById(x => x.Id == lookid, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include("LookSubAttribute.SubAttribute").Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

            List<GameFeedback> feebbackList = new List<GameFeedback>();

            bool isTodayFeebback = false;

            List<Data.Models.Attribute> attrList = new List<Data.Models.Attribute>();
            Filter filter = new Filter();


            bool isLastFeebback = false;
            bool isQuantity = false;
            bool IsWeighted = false;
            if (filterId.HasValue)
            {
                filter = repoFilter.GetIncludingById(x => x.Id == filterId.Value, x => x.Include(m => m.FilterEmotionsFor));
                var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();
                filterName = filter.Name;
                isTodayFeebback = filter.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = filter.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = filter.CalculationType == (int)LookCalculation.WeightedAverage;

                List<int> filterGameid = filter.FilterEmotionsFor.Select(s => s.ForId).ToList();
                feebbackList = await repoGameFeedback.GetAllIncludingAsyn(f => (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate)) && !f.IsDraft && (f.GameFeedbackDetails.Any(a => filterGameid.Contains(a.GameId) && (attributeId.HasValue ? a.AttributeId == attributeId.Value : true)) && f.GameFeedbackDetails.Any(a => attrbutterList.Contains(a.AttributeId))), x => x.Include(m => m.GameFeedbackDetails));

                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Where(a => attributeId.HasValue ? a.Id == attributeId.Value : true).Select(x => x.Name).ToList();

            }
            else
            if (lookid.HasValue)
            {
                isTodayFeebback = x.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = x.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = x.CalculationType == (int)LookCalculation.WeightedAverage;
                filterName = x.Name;
                feebbackList = await repoGameFeedback.GetAllIncludingAsyn(f => f.LookId == lookid && (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate)) && !f.IsDraft && (attributeId.HasValue ? f.GameFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), f => f.Include(m => m.GameFeedbackDetails));


                var attrbutterList = repoAttribute.GetAll().Where(y => y.LookId == lookid && attributeId.HasValue ? y.AttributeId == attributeId.Value : true).Select(s => s.AttributeId).ToList();
                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Select(x => x.Name).ToList();
            }

            List<Role> roleList = repoRole.GetAll(x => x.CompanyId == x.CompanyId).ToList();
            var playerList = new List<GameUser>();
            if (lookid.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(x.CompanyId);
            }
            else if (filterId.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(filter.CompanyId);
            }


            if (lookid.HasValue && x.LookSubAttribute.Any(a => a.SubAttribute.IsQuantity))
            {
                isQuantity = true;
            }

            List<int> allUserId = new List<int>();
            List<int> FilterUserIds = new List<int>();
            List<int> overAllUserId = lookid.HasValue ? x.LookPlayers.Select(s => s.UserId).ToList() : new List<int>();
            allUserId.AddRange(overAllUserId);

            List<SubAttribute> subAttrList = attributeId.HasValue ?
               repoAtSubAttribute.GetAll(x => x.AttributeId == attributeId).ToList()
               : new List<SubAttribute>();



            if (!filterId.HasValue)
            {
                foreach (var item in x.LookGroup)
                {
                    allUserId.AddRange(item.LookGroupPlayer.Select(s => s.UserId).ToList());
                }
            }
            else
            {
                FilterUserIds = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamUserIdList = repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsCalculation).Select(s => s.UserId).ToList();
                var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                allUserId.AddRange(FilterUserIds);
                foreach (var item in teamList)
                {
                    allUserId.AddRange(item.TeamPlayer.Where(w => teamUserIdList.Contains(w.UserId)).Select(s => s.UserId).ToList());
                }
            }

            allUserId = allUserId.Distinct().ToList();

            List<WeightagePresetDetails> weightagePresetList = new List<WeightagePresetDetails>();
            List<LookGame> gameList = new List<LookGame>();
            if (lookid.HasValue)
            {
                gameList = x.LookGame.ToList();
            }
            else if (filterId.HasValue)
            {
                gameList = filter.FilterEmotionsFor.Select(s => new LookGame()
                {
                    GameId = s.ForId
                }).ToList();

            }


            //if (filterId.HasValue)
            //{
            //    var gamleList = repoFilterEmotionsFor.GetAll().Where(y => y.FilterId == filterId).Select(s => s.ForId).ToList();
            //    gameList = gameList.Where(s => gamleList.Contains(s.GameId)).ToList();
            //}





            if (!presetId.HasValue)
                weightagePresetList = repoWeightagePresetDetails.GetAll(x => x.WeightagePreset.IsDefault && x.WeightagePreset.GameId == gId).ToList();
            else
                weightagePresetList = repoWeightagePresetDetails.GetAll(x => x.WeightagePresetId == presetId && x.WeightagePreset.GameId == gId).ToList();

            List<GameFeedbackGridDto> feedbackGrid = new List<GameFeedbackGridDto>();
            foreach (var item in gameList)
            {
                List<WeightagePresetDetailsDto> gameFeed = new List<WeightagePresetDetailsDto>();
                if (filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var attr in subAttrList)
                    {
                        double Weightage = 0;
                        double Percentage = 0;
                        if (feebbackList.Any(l => l.LookId == lookid) && feebbackList.Any(l => l.LookId == lookid && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => l.LookId == lookid).GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                {
                                    Weightage = feebbackList.Where(l => l.LookId == lookid).OrderByDescending(o => o.AddedDate).FirstOrDefault().GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.Where(l => l.LookId == lookid).OrderByDescending(o => o.AddedDate).FirstOrDefault().GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(c.Percentage.Value.ToString()))));
                                }
                            }
                            else if (feebbackList.Any(l => l.LookId == lookid && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => l.LookId == lookid).Average(a => a.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (feebbackList.Any(l => l.LookId == lookid) && feebbackList.Any(l => l.LookId == lookid && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue)) ?
                           feebbackList.LastOrDefault(l => l.LookId == lookid).GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0);
                        }

                        var GameId = item.GameId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId,
                            Percentage= Percentage
                        });
                    }

                }
                else if (filterId.HasValue && filter != null && filter.IsSelf)
                {
                    foreach (var attr in attrList)
                    {
                        double Weightage = 0;
                        double Percentage = 0; 
                        int attrId = attr.Id;

                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId
                                && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                Weightage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                Percentage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                            }
                            else if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {

                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Average(a => a.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }

                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue))
                                && feebbackList.LastOrDefault(l => l.LookId == lookid).GameFeedbackDetails.Any(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue) ?
                               feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).GameFeedbackDetails.Where(w => w.GameId == item.GameId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0);
                        }

                        var GameId = item.GameId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attrId,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }
                }
                else
                {
                    foreach (var user in allUserId)
                    {
                        var defaultPreset = playerList.Any(x => x.UserId == user) ? roleList.FirstOrDefault(x => x.Id == playerList.FirstOrDefault(x => x.UserId == user).UserTypeId).Weightage : roleList.Min(w => w.Weightage);

                        double Weightage = 0;
                        double Percentage = 0;

                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).GameFeedbackDetails.Any(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                {
                                    Weightage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).GameFeedbackDetails.Where(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).GameFeedbackDetails.Where(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c =>c.Percentage.Value)));
                                }

                            }
                            else
                            {
                                if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).Average(a => a.GameFeedbackDetails.Where(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))));
                                }
                            }
                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (isQuantity && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.GameFeedbackDetails.Any(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true))) ? (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).GameFeedbackDetails.Where(w => w.GameId == item.GameId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0)) : 0);
                        }
                        var UserId = user;
                        var GameId = item.GameId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = UserId,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }
                }

                GameFeedbackGridDto gridDto = new GameFeedbackGridDto() { IsQuantity = isQuantity };
                gridDto.Feebback = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ?
                    gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0)
                    .Average(b => b.Weightage.Value) : 0;

                gridDto.Percentage = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ?
                    gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Percentage.HasValue && w.Weightage > 0)
                    .Average(b => b.Percentage.Value) : 0;

                gridDto.FeebbackQuantity = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ?
                   gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0)
                   .Average(b => b.WeightageQuantity.Value) : 0;
                if (isQuantity)
                {
                    gridDto.Feebback = gridDto.FeebbackQuantity;
                }

                gridDto.GameId = item.GameId;

                if (filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var group in subAttrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "subattr_" + group.Id,
                            Percentage= overAllpercent
                        });
                    }
                }
                else if (filterId.HasValue && filter.IsSelf)
                {
                    foreach (var group in attrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "attr_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else
                if (!filterId.HasValue)
                {
                    foreach (var group in x.LookGroup)
                    {
                        List<int> userList = group.LookGroupPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;


                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "group_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                    var userIdList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userIdList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView).Select(s => s.UserId).ToList());
                    userIdList = userIdList.Distinct().ToList();

                    foreach (var group in teamList)
                    {
                        List<int> userList = group.TeamPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ?
                            gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllpercent = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ?
                          gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;


                        var overAllQuantity = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ?
                           gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "team_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }



                    foreach (var group in userIdList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group && w.Weightage.HasValue && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group && w.Percentage.HasValue && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ? gameFeed.Where(w => w.UserId == group && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.Weightage.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "user_" + group,
                            Percentage = overAllpercent
                        });
                    }
                }
                gridDto.Feebback = gridDto.Groups.Any(a => a.Feebback > 0) ? gridDto.Groups.Where(x => x.Feebback > 0).Average(a => a.Feebback) : gridDto.Feebback;
                gridDto.Percentage = gridDto.Groups.Any(a => a.Percentage > 0) ? gridDto.Groups.Where(x => x.Percentage > 0).Average(a => a.Percentage) : gridDto.Percentage;
                gridDto.FeebbackQuantity = gridDto.Groups.Any(a => a.FeebbackQuantity > 0) ? gridDto.Groups.Where(x => x.FeebbackQuantity > 0).Average(a => a.FeebbackQuantity) : gridDto.Feebback;
                gridDto.IsWeighted = IsWeighted;
                feedbackGrid.Add(gridDto);
            }

            return new Tuple<List<GameFeedbackGridDto>, List<string>, string>(feedbackGrid, AtterbuteNameList, filterName);
        }


        public async Task<Tuple<List<GameFeedbackGridDto>, List<string>, string>> GetTeamFeedback(int tId, int? lookid, int? presetId, int? attributeId, int? filterId,
         DateTime filterFromDate, DateTime filterToDate)
        {

            List<string> AtterbuteNameList = new List<string>();
            string filterName = string.Empty;
            var x = this.repo.GetIncludingById(x => x.Id == lookid, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include("LookSubAttribute.SubAttribute").Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

            List<TeamFeedback> feebbackList = new List<TeamFeedback>();
            List<Data.Models.Attribute> attrList = new List<Data.Models.Attribute>();
            Filter filter = new Filter();
            bool isTodayFeebback = false;
            bool isLastFeebback = false;
            bool IsWeighted = false;
            if (filterId.HasValue)
            {
                filter = repoFilter.GetIncludingById(x => x.Id == filterId.Value, x => x.Include(m => m.FilterEmotionsFor));
                var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();
                isTodayFeebback = filter.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = filter.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = filter.CalculationType == (int)LookCalculation.WeightedAverage;
                filterName = filter.Name;
                List<int> filterGameid = filter.FilterEmotionsFor.Select(s => s.ForId).ToList();
                feebbackList = await repoTeamFeedback.GetAllIncludingAsyn(f => f.LookId == lookid && (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (x.AddedDate >= filterFromDate && f.AddedDate <= filterToDate)) && !f.IsDraft && (attributeId.HasValue ? f.TeamFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true) && f.TeamFeedbackDetails.Any(a => attrbutterList.Contains(a.AttributeId)), x => x.Include(m => m.TeamFeedbackDetails));

                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();


                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Where(a => attributeId.HasValue ? a.Id == attributeId.Value : true).Select(x => x.Name).ToList();
            }
            else
            if (lookid.HasValue)
            {
                isTodayFeebback = x.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = x.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = x.CalculationType == (int)LookCalculation.WeightedAverage;
                filterName = x.Name;
                feebbackList = repoTeamFeedback.GetAllIncluding(f => f.LookId == lookid && (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate)) && !f.IsDraft && (attributeId.HasValue ? f.TeamFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), x => x.Include(m => m.TeamFeedbackDetails)).ToList();


                var attrbutterList = repoAttribute.GetAll().Where(y => y.LookId == lookid && attributeId.HasValue ? y.AttributeId == attributeId.Value : true).Select(s => s.AttributeId).ToList();
                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Select(x => x.Name).ToList();
            }

            bool isQuantity = false;
            List<Role> roleList = repoRole.GetAll(x => x.CompanyId == x.CompanyId).ToList();
            var playerList = new List<GameUser>();
            if (lookid.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(x.CompanyId);
            }
            else if (filterId.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(filter.CompanyId);
            }




            List<int> FilterUserIds = new List<int>();
            List<int> allUserId = new List<int>();
            List<int> overAllUserId = lookid.HasValue ? x.LookPlayers.Select(s => s.UserId).ToList() : new List<int>();
            allUserId.AddRange(overAllUserId);
            if (!filterId.HasValue)
            {
                foreach (var item in x.LookGroup)
                {
                    allUserId.AddRange(item.LookGroupPlayer.Select(s => s.UserId).ToList());
                }
            }
            else
            {
                FilterUserIds = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamUserIdList = repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsCalculation).Select(s => s.UserId).ToList();
                var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                allUserId.AddRange(FilterUserIds);
                foreach (var item in teamList)
                {
                    allUserId.AddRange(item.TeamPlayer.Where(w => teamUserIdList.Contains(w.UserId)).Select(s => s.UserId).ToList());
                }
            }
            allUserId = allUserId.Distinct().ToList();

            // List<WeightagePresetDetails> weightagePresetList = new List<WeightagePresetDetails>();





            List<SubAttribute> subAttrList = attributeId.HasValue ?
               repoAtSubAttribute.GetAll(x => x.AttributeId == attributeId).ToList()
               : new List<SubAttribute>();


            List<LookTeam> gameList = new List<LookTeam>();
            if (lookid.HasValue)
            {
                gameList = x.LookTeam.ToList();
            }
            else if (filterId.HasValue)
            {
                gameList = filter.FilterEmotionsFor.Select(s => new LookTeam()
                {
                    TeamId = s.ForId
                }).ToList();
            }


            List<GameFeedbackGridDto> feedbackGrid = new List<GameFeedbackGridDto>();
            foreach (var item in gameList)
            {
                List<WeightagePresetDetailsDto> gameFeed = new List<WeightagePresetDetailsDto>();

                if (filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var attr in subAttrList)
                    {
                        double Weightage = 0;
                        double Percentage = 0;
                        if (feebbackList.Any(l => l.LookId == lookid) && feebbackList.Any(l => l.LookId == lookid && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => l.LookId == lookid).TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                {
                                    Weightage = feebbackList.Where(l => l.LookId == lookid).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.Where(l => l.LookId == lookid).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(c.Percentage.Value.ToString()))));
                                }
                            }
                            else if (feebbackList.Any(l => l.LookId == lookid && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => l.LookId == lookid).Average(a => a.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (feebbackList.Any(l => l.LookId == lookid) && feebbackList.Any(l => l.LookId == lookid && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue)) ?
                           feebbackList.LastOrDefault(l => l.LookId == lookid).TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0);
                        }

                        var GameId = item.TeamId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }

                }
                else if (filterId.HasValue && filter != null && filter.IsSelf)
                {
                    foreach (var attr in attrList)
                    {
                        var attrId = attr.Id;
                        double Weightage = 0;
                        double WeightageQuantity = 0;
                        double Percentage = 0;
                        try
                        {

                            if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.TeamId && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                if (isLastFeebback || isTodayFeebback)
                                {
                                    if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.TeamId).TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                    {
                                        Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.TeamId).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                        Percentage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.TeamId).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                                    }
                                }
                                else if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Average(a => a.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                                }
                            }

                            if (isQuantity)
                            {
                                WeightageQuantity = ((feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && w.AttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))) ?
                                   feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && w.AttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0));
                            }
                        }
                        catch
                        {

                            //throw;
                        }


                        var GameId = item.TeamId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }

                }
                else if (filterId.HasValue)
                {
                    foreach (var user in allUserId)
                    {
                        double Weightage = 0;
                        double Percentage = 0;

                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) && feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).TeamFeedbackDetails.LastOrDefault(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) != null)
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).OrderByDescending(o => o.AddedDate).FirstOrDefault().TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                                }

                            }
                            else if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).Average(a => a.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) && a.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }




                        double WeightageQuantity = 0;
                        if (isQuantity)
                        {
                            WeightageQuantity = ((feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                && feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).TeamFeedbackDetails.Any(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ?
                                feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).TeamFeedbackDetails.Where(w => w.TeamId == item.TeamId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0));
                        }
                        var UserId = user;
                        var GameId = item.TeamId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = UserId,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }
                }

                GameFeedbackGridDto gridDto = new GameFeedbackGridDto() { IsQuantity = isQuantity };
                gridDto.Feebback = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Weightage > 0) ? gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                gridDto.Percentage = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Percentage > 0) ? gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                gridDto.FeebbackQuantity = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity > 0) ?
                    gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                if (isQuantity)
                {
                    gridDto.Feebback = gridDto.FeebbackQuantity;
                }

                gridDto.GameId = item.TeamId;
                if (filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var group in subAttrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity.HasValue) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "subattr_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else if (filterId.HasValue && filter.IsSelf)
                {
                    foreach (var group in attrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "attr_" + group.Id,
                            Percentage = overAllpercent

                        });
                    }
                }
                else if (!filterId.HasValue)
                {
                    foreach (var group in x.LookGroup)
                    {
                        List<int> userList = group.LookGroupPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;


                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "group_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();

                    foreach (var group in teamList)
                    {
                        List<int> userList = group.TeamPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;


                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "team_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }

                    var userIdList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userIdList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView).Select(s => s.UserId).ToList());
                    userIdList = userIdList.Distinct().ToList();

                    foreach (var group in userIdList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group && w.WeightageQuantity.HasValue) ?
                            gameFeed.Where(w => w.UserId == group && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;


                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "user_" + group,
                            Percentage = overAllpercent
                        });
                    }
                }
                gridDto.Feebback = gridDto.Groups.Any() ? gridDto.Groups.Average(a => a.Feebback) : gridDto.Feebback;
                gridDto.Percentage = gridDto.Groups.Any() ? gridDto.Groups.Average(a => a.Percentage) : gridDto.Percentage;
                gridDto.FeebbackQuantity = gridDto.Groups.Any() ? gridDto.Groups.Average(a => a.FeebbackQuantity) : gridDto.Feebback;
                gridDto.IsWeighted = IsWeighted;
                feedbackGrid.Add(gridDto);
            }


            return new Tuple<List<GameFeedbackGridDto>, List<string>, string>(feedbackGrid, AtterbuteNameList, filterName);
        }

        public async Task<Tuple<List<GameFeedbackGridDto>, List<string>, string>> GetUserFeedback(int tId, int? lookid, int? presetId, int? attributeId, int? filterId,
DateTime filterFromDate, DateTime filterToDate)
        {

            List<string> AtterbuteNameList = new List<string>();
            string filterName = string.Empty;
            var x = this.repo.GetIncludingById(x => x.Id == lookid, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include("LookSubAttribute.SubAttribute").Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

            bool isLastFeebback = false;
            bool isTodayFeebback = false;
            bool IsWeighted = false;
            if (lookid.HasValue)
            {
                isLastFeebback = x.CalculationType ==(int)LookCalculation.LastUpdate;
            }

            List<UserFeedback> feebbackList = new List<UserFeedback>();
            List<Data.Models.Attribute> attrList = new List<Data.Models.Attribute>();
            Filter filter = new Filter();



            if (filterId.HasValue)
            {
                filter = repoFilter.GetIncludingById(x => x.Id  == filterId.Value, x => x.Include(m => m.FilterEmotionsFor));
                isTodayFeebback = filter.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = filter.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = filter.CalculationType ==  (int)LookCalculation.WeightedAverage;

                filterName = filter.Name;
                List<int> filterGameid = filter.FilterEmotionsFor.Select(s => s.ForId).ToList();
                feebbackList = repoUserFeedback.GetAllIncluding(f => (isTodayFeebback ? f.FeedbackDate == filterFromDate : f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate) && !f.IsDraft, x => x.Include(m => m.UserFeedbackDetails)).ToList();
            }
            else
            if (lookid.HasValue)
            {
                isTodayFeebback = x.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = x.CalculationType == (int)LookCalculation.LastUpdate;
                IsWeighted = x.CalculationType == (int)LookCalculation.WeightedAverage;
                filterName = x.Name;
                feebbackList = repoUserFeedback.GetAllIncluding(f => f.LookId == lookid && (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate)) && !f.IsDraft && (attributeId.HasValue ? f.UserFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), x => x.Include(m => m.UserFeedbackDetails)).ToList();


                var attrbutterList = repoAttribute.GetAll().Where(y => y.LookId == lookid && attributeId.HasValue ? y.AttributeId == attributeId.Value : true).Select(s => s.AttributeId).ToList();
                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Select(x => x.Name).ToList();
            }



            bool isQuantity = false;
            // List<Role> roleList = repoRole.GetAll(x => x.CompanyId == x.CompanyId).ToList();
            var playerList = new List<GameUser>();
            if (lookid.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(x.CompanyId);
            }
            else if (filterId.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(filter.CompanyId);
            }

            List<SubAttribute> subAttrList = attributeId.HasValue ?
                repoAtSubAttribute.GetAll(x => x.AttributeId == attributeId).ToList()
                : new List<SubAttribute>();

            List<int> allUserId = new List<int>();
            List<int> overAllUserId = lookid.HasValue ? x.LookPlayers.Select(s => s.UserId).ToList() : new List<int>();
            allUserId.AddRange(overAllUserId);
            if (!filterId.HasValue)
            {
                foreach (var item in x.LookGroup)
                {
                    allUserId.AddRange(item.LookGroupPlayer.Select(s => s.UserId).ToList());
                }
            }
            else
            {

                var teamUserIdList = repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsCalculation).Select(s => s.UserId).ToList();
                var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                allUserId.AddRange(repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList());
                foreach (var item in teamList)
                {
                    allUserId.AddRange(item.TeamPlayer.Where(w => teamUserIdList.Contains(w.UserId)).Select(s => s.UserId).ToList());
                }
            }
            allUserId = allUserId.Distinct().ToList();


            List<LookUser> gameList = lookid.HasValue ? x.LookUser.ToList() : new List<LookUser>();
            if (filterId.HasValue)
            {
                var gamleList = repoFilterEmotionsFor.GetAll().Where(y => y.FilterId == filterId).Select(s => s.ForId).ToList();
                gameList = gameList.Any() ? gameList.Where(s => gamleList.Contains(s.UserId)).ToList() : gamleList.Select(s => new LookUser() { UserId = s }).ToList();
                var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();
                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Where(a => attributeId.HasValue ? a.Id == attributeId.Value : true).Select(x => x.Name).ToList();

                feebbackList.ForEach(x =>
                {
                    x.UserFeedbackDetails = x.UserFeedbackDetails.Where(a => gameList.Select(u => u.UserId).ToList().Contains(a.UserId) && (attributeId.HasValue ? a.AttributeId == attributeId.Value : true) && attrbutterList.Contains(a.AttributeId)).ToList();
                });
            }
            else
            {

                feebbackList = await repoUserFeedback.GetAllIncludingAsyn(x => x.LookId == lookid && !x.IsDraft && (attributeId.HasValue ? x.UserFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), x => x.Include(m => m.UserFeedbackDetails));
            }



            List<GameFeedbackGridDto> feedbackGrid = new List<GameFeedbackGridDto>();
            foreach (var item in gameList)
            {
                List<WeightagePresetDetailsDto> gameFeed = new List<WeightagePresetDetailsDto>();
                if (!filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var attr in subAttrList)
                    {
                        double Weightage = 0;
                        double Percentage = 0;
                        if (feebbackList.Any() && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList != null && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) &&
                                  l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))) &&
                                feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId
                                && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))).UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId
                                && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))).OrderByDescending(o => o.AddedDate).FirstOrDefault()
                                .UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))
                                .GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))));

                                    Percentage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId
                                && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))).OrderByDescending(o => o.AddedDate).FirstOrDefault()
                                .UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))
                                .GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(c.Percentage.Value.ToString())));


                                }
                            }
                            else if (feebbackList != null && feebbackList.Any() && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))).Average(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue)) ?
                           feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0);
                        }

                        var GameId = item.UserId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }

                }
                else if (filterId.HasValue && filter != null && filter.IsSelf)
                {
                    foreach (var attr in attrList)
                    {
                        double Weightage = 0;
                        double Percentage = 0;

                        int attrId = attr.Id;

                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId
                                && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList != null && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId) && feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                {
                                    Weightage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                                }
                            }
                            else if (feebbackList != null && feebbackList.Any() && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {

                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Average(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }

                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue))
                                && feebbackList.LastOrDefault(l => l.LookId == lookid).UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue) ?
                               feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true) && w.Quantity.HasValue).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0);
                        }

                        var GameId = item.UserId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attrId,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }
                }
                else
                {
                    foreach (var user in allUserId)
                    {

                        double Weightage = 0;
                        double Percentage = 0;


                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (
                                    feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) &&
                                    feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).
                                    UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))
                                    )
                                {
                                    Weightage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))).UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    Percentage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))).UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                                    //Percentage = feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => (attributeId.HasValue ? w.AttributeId == attributeId : true))).UserFeedbackDetails.Where(w => (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => c.Percentage.Value)));
                                }

                            }
                            else
                            {
                                if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))).Average(a => a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))));
                                }
                            }
                        }

                        double WeightageQuantity = 0;

                        if (isQuantity)
                        {
                            WeightageQuantity = (isQuantity && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))) ? (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0)) : 0);
                        }
                        var UserId = user;
                        var GameId = item.UserId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = UserId,
                            GameId = GameId,
                            Percentage = Percentage
                        });
                    }
                }

                GameFeedbackGridDto gridDto = new GameFeedbackGridDto() { IsQuantity = isQuantity };
                gridDto.Feebback = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ?
                    gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0)
                    .Average(b => b.Weightage.Value) : 0;

                gridDto.Percentage = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ?
                gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0)
                .Average(b => b.Percentage.Value) : 0;

                gridDto.FeebbackQuantity = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ?
                   gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0)
                   .Average(b => b.WeightageQuantity.Value) : 0;
                if (isQuantity)
                {
                    gridDto.Feebback = gridDto.FeebbackQuantity;
                }

                gridDto.GameId = item.UserId;
                if (!filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var group in subAttrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "subattr_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else if (filterId.HasValue && filter.IsSelf)
                {
                    foreach (var group in attrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group.Id && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "attr_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else
                if (!filterId.HasValue)
                {
                    foreach (var group in x.LookGroup)
                    {
                        List<int> userList = group.LookGroupPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;


                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "group_" + group.Id,
                            Percentage = overAllpercent
                        });
                    }
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                    var userIdList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userIdList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView).Select(s => s.UserId).ToList());
                    userIdList = userIdList.Distinct().ToList();

                    foreach (var group in teamList)
                    {
                        List<int> userList = group.TeamPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0) ?
                            gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0) ?
    gameFeed.Where(w => userList.Contains(w.UserId) && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;


                        var overAllQuantity = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ?
                           gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "team_" + group.Id,
                            Percentage= overAllpercent
                        });
                    }



                    foreach (var group in userIdList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group && w.Weightage.HasValue && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group && w.Weightage.HasValue && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;
                        var overAllpercent = gameFeed.Any(w => w.UserId == group && w.Percentage.HasValue && w.Percentage > 0) ? gameFeed.Where(w => w.UserId == group && w.Percentage.HasValue && w.Percentage > 0).Average(b => b.Percentage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0) ? gameFeed.Where(w => w.UserId == group && w.WeightageQuantity.HasValue && w.WeightageQuantity > 0).Average(b => b.Weightage.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "user_" + group,
                            Percentage = overAllpercent
                        });
                    }
                }
                gridDto.Feebback = gridDto.Groups.Any(a => a.Feebback > 0) ? gridDto.Groups.Where(x => x.Feebback > 0).Average(a => a.Feebback) : gridDto.Feebback;
                gridDto.FeebbackQuantity = gridDto.Groups.Any(a => a.FeebbackQuantity > 0) ? gridDto.Groups.Where(x => x.FeebbackQuantity > 0).Average(a => a.FeebbackQuantity) : gridDto.Feebback;
                gridDto.IsWeighted = IsWeighted;
                gridDto.Percentage = gridDto.Groups.Any(a => a.Percentage > 0) ? gridDto.Groups.Where(x => x.Percentage > 0).Average(a => a.Percentage) : gridDto.Percentage;
                feedbackGrid.Add(gridDto);
            }



            return new Tuple<List<GameFeedbackGridDto>, List<string>, string>(feedbackGrid, AtterbuteNameList, filterName);
        }

        public async Task<Tuple<List<GameFeedbackGridDto>, List<string>, string>> GetUserFeedback1(int tId, int? lookid, int? presetId, int? attributeId, int? filterId,
DateTime filterFromDate, DateTime filterToDate)
        {

            List<string> AtterbuteNameList = new List<string>();
            string filterName = string.Empty;
            var x = this.repo.GetIncludingById(x => x.Id == lookid, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include("LookSubAttribute.SubAttribute").Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

            bool isLastFeebback = false;
            bool isTodayFeebback = false;
            if (lookid.HasValue)
            {
                isLastFeebback = x.CalculationType == (int)LookCalculation.LastUpdate;
            }

            List<UserFeedback> feebbackList = new List<UserFeedback>();
            List<Data.Models.Attribute> attrList = new List<Data.Models.Attribute>();
            Filter filter = new Filter();



            if (filterId.HasValue)
            {
                filter = repoFilter.GetIncludingById(x => x.Id == filterId.Value, x => x.Include(m => m.FilterEmotionsFor));
                isTodayFeebback = filter.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = filter.CalculationType == (int)LookCalculation.LastUpdate;
                var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();
                filterName = filter.Name;
                List<int> filterGameid = filter.FilterEmotionsFor.Select(s => s.ForId).ToList();
                feebbackList = repoUserFeedback.GetAllIncluding(f => (isTodayFeebback ? f.FeedbackDate == filterFromDate : f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate) && !f.IsDraft && f.UserFeedbackDetails.Any(a => filterGameid.Contains(a.UserId) && (attributeId.HasValue ? a.AttributeId == attributeId.Value : true)) && f.UserFeedbackDetails.Any(a => attrbutterList.Contains(a.AttributeId)), x => x.Include(m => m.UserFeedbackDetails)).ToList();


                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Where(a => attributeId.HasValue ? a.Id == attributeId.Value : true).Select(x => x.Name).ToList();


            }
            else
            if (lookid.HasValue)
            {
                isTodayFeebback = x.CalculationType == (int)LookCalculation.ForToday;
                isLastFeebback = x.CalculationType == (int)LookCalculation.LastUpdate;
                filterName = x.Name;
                feebbackList = repoUserFeedback.GetAllIncluding(f => f.LookId == lookid && (isTodayFeebback ? (f.FeedbackDate == filterFromDate) : (f.FeedbackDate >= filterFromDate && f.FeedbackDate <= filterToDate)) && !f.IsDraft && (attributeId.HasValue ? f.UserFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), x => x.Include(m => m.UserFeedbackDetails)).ToList();


                var attrbutterList = repoAttribute.GetAll().Where(y => y.LookId == lookid && attributeId.HasValue ? y.AttributeId == attributeId.Value : true).Select(s => s.AttributeId).ToList();
                attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                AtterbuteNameList = attrList.Select(x => x.Name).ToList();
            }



            bool isQuantity = false;
            // List<Role> roleList = repoRole.GetAll(x => x.CompanyId == x.CompanyId).ToList();
            var playerList = new List<GameUser>();
            if (lookid.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(x.CompanyId);
            }
            else if (filterId.HasValue)
            {
                playerList = await gameService.GetGamlePlayer(filter.CompanyId);
            }

            List<SubAttribute> subAttrList = attributeId.HasValue ?
                repoAtSubAttribute.GetAll(x => x.AttributeId == attributeId).ToList()
                : new List<SubAttribute>();

            List<int> allUserId = new List<int>();
            List<int> overAllUserId = lookid.HasValue ? x.LookPlayers.Select(s => s.UserId).ToList() : new List<int>();
            allUserId.AddRange(overAllUserId);
            if (!filterId.HasValue)
            {
                foreach (var item in x.LookGroup)
                {
                    allUserId.AddRange(item.LookGroupPlayer.Select(s => s.UserId).ToList());
                }
            }
            else
            {

                var teamUserIdList = repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsCalculation).Select(s => s.UserId).ToList();
                var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();
                allUserId.AddRange(repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList());
                foreach (var item in teamList)
                {
                    allUserId.AddRange(item.TeamPlayer.Where(w => teamUserIdList.Contains(w.UserId)).Select(s => s.UserId).ToList());
                }
            }
            allUserId = allUserId.Distinct().ToList();


            List<LookUser> gameList = lookid.HasValue ? x.LookUser.ToList() : new List<LookUser>();
            if (filterId.HasValue)
            {
                var gamleList = repoFilterEmotionsFor.GetAll().Where(y => y.FilterId == filterId).Select(s => s.ForId).ToList();
                gameList = gameList.Any() ? gameList.Where(s => gamleList.Contains(s.UserId)).ToList() : gamleList.Select(s => new LookUser() { UserId = s }).ToList();
            }
            else
            {

                feebbackList = await repoUserFeedback.GetAllIncludingAsyn(x => x.LookId == lookid && !x.IsDraft && (attributeId.HasValue ? x.UserFeedbackDetails.Any(a => a.AttributeId == attributeId.Value) : true), x => x.Include(m => m.UserFeedbackDetails));
            }



            List<GameFeedbackGridDto> feedbackGrid = new List<GameFeedbackGridDto>();
            foreach (var item in gameList)
            {
                List<WeightagePresetDetailsDto> gameFeed = new List<WeightagePresetDetailsDto>();
                if (!filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var attr in subAttrList)
                    {
                        double Weightage = 0;
                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Any())
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).OrderByDescending(o => o.AddedDate).FirstOrDefault().UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                }

                            }
                            else if (feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Any(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)) && a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Average(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ?
                                                          a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }

                        double WeightageQuantity = 0;
                        if (isQuantity)
                        {
                            WeightageQuantity = ((feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))) ?
                               feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.SubAttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0));
                        }

                        var GameId = item.UserId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId
                        });
                    }

                }
                else if (filterId.HasValue && filter != null && filter.IsSelf)
                {
                    foreach (var attr in attrList)
                    {
                        var attrId = attr.Id;
                        double Weightage = 0;
                        double WeightageQuantity = 0;
                        try
                        {

                            if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                if (isLastFeebback || isTodayFeebback)
                                {
                                    if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                    {
                                        Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == item.UserId).OrderByDescending(o => o.AddedDate).FirstOrDefault().UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                    }
                                }
                                else if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true)).Average(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attrId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                                }
                            }

                            if (isQuantity)
                            {
                                WeightageQuantity = ((feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true)) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && w.AttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true))) ?
                                   feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true)).UserFeedbackDetails.Where(w => w.UserId == item.UserId && w.AttributeId == attr.Id && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0));
                            }
                        }
                        catch
                        {

                            //throw;
                        }


                        var GameId = item.UserId;
                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = attr.Id,
                            GameId = GameId
                        });
                    }
                }
                else
                {
                    foreach (var user in allUserId)
                    {
                        double Weightage = 0;

                        if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                        {
                            if (isLastFeebback || isTodayFeebback)
                            {
                                if (feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) && feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).UserFeedbackDetails.LastOrDefault(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) != null)
                                {
                                    Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).OrderByDescending(o => o.AddedDate).FirstOrDefault().UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback)))));
                                }

                            }
                            else if (feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true))))
                            {
                                Weightage = feebbackList.Where(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).Average(a => a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) && a.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ? a.UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).GroupBy(a => a.AttributeId).Average(a => a.GroupBy(a => a.SubAttributeId).Average(b => b.Average(c => int.Parse(EncryptDecrypt.Decrypt(c.Feedback))))) : 0);
                            }
                        }


                        double WeightageQuantity = 0;
                        if (isQuantity)
                        {
                            WeightageQuantity = ((feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user) && feebbackList.Any(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user && l.UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)))
                                && feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).UserFeedbackDetails.Any(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)) ?
                                feebbackList.LastOrDefault(l => (lookid.HasValue ? l.LookId == lookid : true) && l.UserId == user).UserFeedbackDetails.Where(w => w.UserId == item.UserId && (attributeId.HasValue ? w.AttributeId == attributeId : true)).Average(a => a.Quantity.HasValue ? a.Quantity.Value : 0) : 0));
                        }
                        var UserId = user;
                        var GameId = item.UserId;

                        gameFeed.Add(new WeightagePresetDetailsDto()
                        {
                            Weightage = !isQuantity ? Weightage : WeightageQuantity,
                            WeightageQuantity = WeightageQuantity,
                            UserId = UserId,
                            GameId = GameId
                        });
                    }
                }

                GameFeedbackGridDto gridDto = new GameFeedbackGridDto() { IsQuantity = isQuantity };
                gridDto.Feebback = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.Weightage > 0) ? gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                gridDto.FeebbackQuantity = gameFeed.Any(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity > 0) ?
                    gameFeed.Where(w => overAllUserId.Contains(w.UserId) && w.WeightageQuantity > 0)
                    .Average(b => b.WeightageQuantity.Value) : 0;

                if (isQuantity)
                {
                    gridDto.Feebback = gridDto.FeebbackQuantity;
                }

                gridDto.GameId = item.UserId;
                if (!filterId.HasValue && attributeId.HasValue)
                {
                    foreach (var group in subAttrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "subattr_" + group.Id
                        });
                    }
                }
                else if (filterId.HasValue && filter.IsSelf)
                {
                    foreach (var group in attrList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group.Id && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group.Id && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group.Id && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group.Id && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "attr_" + group.Id
                        });
                    }
                }
                else
                if (!filterId.HasValue)
                {
                    foreach (var group in x.LookGroup)
                    {
                        List<int> userList = group.LookGroupPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage > 0) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue) ?
                            gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "group_" + group.Id
                        });
                    }
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    var teamList = repo_Team.GetAllIncluding(x => teamIdList.Contains(x.Id), x => x.Include(m => m.TeamPlayer)).ToList();

                    foreach (var group in teamList)
                    {
                        List<int> userList = group.TeamPlayer.Select(s => s.UserId).ToList();
                        var overAll1 = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.Weightage > 0) ?
                        gameFeed.Where(w => userList.Contains(w.UserId) && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllQuantity = userList.Any() && gameFeed.Any(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue) ? gameFeed.Where(w => userList.Contains(w.UserId) && w.WeightageQuantity.HasValue).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "team_" + group.Id
                        });
                    }

                    var userIdList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userIdList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView)
                        .Select(s => s.UserId).ToList());
                    userIdList = userIdList.Distinct().ToList();
                    foreach (var group in userIdList)
                    {
                        var overAll1 = gameFeed.Any(w => w.UserId == group && w.Weightage > 0) ? gameFeed.Where(w => w.UserId == group && w.Weightage > 0).Average(b => b.Weightage.Value) : 0;

                        var overAllQuantity = gameFeed.Any(w => w.UserId == group && w.WeightageQuantity > 0) ?
                            gameFeed.Where(w => w.UserId == group && w.WeightageQuantity > 0).Average(b => b.WeightageQuantity.Value) : 0;

                        gridDto.Groups.Add(new GameFeedbackGroupGridDto()
                        {
                            Feebback = !isQuantity ? overAll1 : overAllQuantity,
                            FeebbackQuantity = overAllQuantity,
                            GroupId = "user_" + group
                        });
                    }
                }
                gridDto.Feebback = gridDto.Groups.Any() ? gridDto.Groups.Average(a => a.Feebback) : gridDto.Feebback;
                gridDto.FeebbackQuantity = gridDto.Groups.Any() ? gridDto.Groups.Average(a => a.FeebbackQuantity) : gridDto.Feebback;

                feedbackGrid.Add(gridDto);
            }



            return new Tuple<List<GameFeedbackGridDto>, List<string>, string>(feedbackGrid, AtterbuteNameList, filterName);
        }

        public async Task<LookDto> GetById(int id)
        {
            var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));
            LookSchedulerDto lookScheduler = new LookSchedulerDto();
            if (x.IsSchedule)
            {
                LookScheduler sch = x.LookScheduler;
                lookScheduler = new LookSchedulerDto()
                {
                    DailyFrequency = sch.DailyFrequency,
                    DaysOfWeek = sch.DaysOfWeek,
                    EndDate = sch.EndDate,
                    ExactDateOfMonth = sch.ExactDateOfMonth,
                    ExactWeekdayOfMonth = sch.ExactWeekdayOfMonth,
                    ExactWeekdayOfMonthEvery = sch.ExactWeekdayOfMonthEvery,
                    Frequency = sch.Frequency,
                    MonthlyOccurrence = sch.MonthlyOccurrence,
                    Name = sch.Name,
                    OccursEveryTimeUnit = sch.OccursEveryTimeUnit,
                    OccursEveryValue = sch.OccursEveryValue,
                    RecurseEvery = sch.RecurseEvery,
                    StartDate = sch.StartDate,
                    TimeEnd = sch.TimeEnd,
                    TimeStart = sch.TimeStart,
                    Type = sch.Type,
                    ValidDays = sch.ValidDays
                };
            }

            return new LookDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                CalculationType = x.CalculationType,
                IsSchedule = x.IsSchedule,
                LookScheduler = lookScheduler,
                IsActive = x.IsActive,
                LookAttribute = x.LookAttribute.Select(s => new LookAttributeDto()
                {
                    Id = s.Id,
                    AttributeId = s.AttributeId
                }).ToList(),
                LookSubAttribute = x.LookSubAttribute.Select(s => new LookSubAttributeDto()
                {
                    Id = s.Id,
                    SubAttributeId = s.SubAttributeId
                }).ToList(),
                LookGame = x.LookGame.Select(s => new LookGameDto()
                {
                    Id = s.Id,
                    GameId = s.GameId
                }).ToList(),
                LookPlayers = x.LookPlayers.Select(s => new LookPlayersDto()
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    IsCalculation = s.IsCalculation,
                    IsView = s.IsView
                }).ToList(),
                LookGroup = x.LookGroup.Select(g => new LookGroupDto()
                {
                    Id = g.Id,
                    Name = g.Name,
                    LookGroupPlayer = g.LookGroupPlayer.Select(p => new LookGroupPlayerDto()
                    {
                        Id = p.Id,
                        UserId = p.UserId
                    }).ToList()
                }).ToList(),
                LookTeam = x.LookTeam.Select(t => new LookTeamDto()
                {
                    Id = t.Id,
                    TeamId = t.TeamId
                }).ToList(),
                LookUser = x.LookUser.Select(u => new LookUserDto()
                {
                    Id = u.Id,
                    UserId = u.UserId
                }).ToList()
            };
        }

        public async Task<LookDto> GetDetailsId(int id)
        {
            var x = await this.repo.GetAsync(id);
            return new LookDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                FromDate = x.FromDate,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                TypeId = x.TypeId,
                CalculationType = x.CalculationType,
                IsActive = x.IsActive
            };
        }
        public async Task<bool> IsExist(int companyId, string name, int? id)
        {
            var result = await this.repo.CountAsync(x => x.CompanyId == companyId && x.Id != id && x.Name == name);
            return result > 0;
        }

        public async Task<int> AddUpdateAsync(LookDto dto)
        {
            if (dto.Id.HasValue)
            {
                var look = this.repo.GetIncludingById(x => x.Id == dto.Id, x => x.Include(m => m.LookAttribute)
            .Include(m => m.LookSubAttribute).Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
            .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

                if (look.LookAttribute != null)
                {
                    var existingAttribute = look.LookAttribute.Where(x => dto.LookAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedAttribute = look.LookAttribute.Where(x => !dto.LookAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedAttribute = dto.LookAttribute.Where(x => !look.LookAttribute.Any(m => m.Id == x.Id)).ToList();


                    if (deletedAttribute.Any())
                    {
                        this.repoAttribute.DeleteRange(deletedAttribute);
                    }

                    if (existingAttribute.Any())
                    {
                        foreach (var e in existingAttribute)
                        {
                            var record = dto.LookAttribute.FirstOrDefault(a => a.Id == e.Id);
                            e.AttributeId = record.AttributeId;
                            repoAttribute.Update(e);
                        }
                    }
                    if (insertedAttribute.Any())
                    {
                        List<LookAttribute> addrecords = insertedAttribute.Select(a => new LookAttribute()
                        {
                            LookId = dto.Id.Value,
                            AttributeId = a.AttributeId,
                        }).ToList();

                        await repoAttribute.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.LookGame != null)
                {
                    var existingGame = look.LookGame.Where(x => dto.LookGame.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGame = look.LookGame.Where(x => !dto.LookGame.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGame = dto.LookGame.Where(x => !look.LookGame.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGame.Any())
                    {
                        this.repoLookGame.DeleteRange(deletedGame);
                    }

                    if (existingGame.Any())
                    {
                        foreach (var e in existingGame)
                        {
                            var record = dto.LookGame.FirstOrDefault(a => a.Id == e.Id);
                            e.GameId = record.GameId;
                            repoLookGame.Update(e);
                        }
                    }
                    if (insertedGame.Any())
                    {
                        List<LookGame> addrecords = insertedGame.Select(a => new LookGame()
                        {
                            LookId = dto.Id.Value,
                            GameId = a.GameId,
                        }).ToList();

                        await repoLookGame.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.LookPlayers != null)
                {
                    var existingPlayer = look.LookPlayers.Where(x => dto.LookPlayers.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedPlayer = look.LookPlayers.Where(x => !dto.LookPlayers.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedPlayer = dto.LookPlayers.Where(x => !look.LookPlayers.Any(m => m.Id == x.Id)).ToList();


                    if (deletedPlayer.Any())
                    {
                        this.repoPlayers.DeleteRange(deletedPlayer);
                    }

                    if (existingPlayer.Any())
                    {
                        foreach (var e in existingPlayer)
                        {
                            var record = dto.LookPlayers.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId;
                            e.IsView = record.IsView;
                            e.IsCalculation = record.IsCalculation;
                            repoPlayers.Update(e);
                        }
                    }
                    if (insertedPlayer.Any())
                    {
                        List<LookPlayers> addrecords = insertedPlayer.Select(a => new LookPlayers()
                        {
                            LookId = dto.Id.Value,
                            UserId = a.UserId,
                            IsCalculation = a.IsCalculation,
                            IsView = a.IsView
                        }).ToList();

                        await repoPlayers.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.LookSubAttribute != null)
                {
                    var existingSubAttribute = look.LookSubAttribute.Where(x => dto.LookSubAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedSubAttribute = look.LookSubAttribute.Where(x => !dto.LookSubAttribute.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedSubAttribute = dto.LookSubAttribute.Where(x => !look.LookSubAttribute.Any(m => m.Id == x.Id)).ToList();


                    if (deletedSubAttribute.Any())
                    {
                        this.repoSubAttribute.DeleteRange(deletedSubAttribute);
                    }

                    if (existingSubAttribute.Any())
                    {
                        foreach (var e in existingSubAttribute)
                        {
                            var record = dto.LookSubAttribute.FirstOrDefault(a => a.Id == e.Id);
                            e.SubAttributeId = record.SubAttributeId;
                            repoSubAttribute.Update(e);
                        }
                    }
                    if (insertedSubAttribute.Any())
                    {
                        List<LookSubAttribute> addrecords = insertedSubAttribute.Select(a => new LookSubAttribute()
                        {
                            LookId = dto.Id.Value,
                            SubAttributeId = a.SubAttributeId,
                        }).ToList();

                        await repoSubAttribute.InsertRangeAsyn(addrecords);
                    }
                }


                if (look.IsSchedule && look.LookScheduler != null)
                {
                    repoScheduler.Delete(look.LookScheduler);
                }

                if (dto.IsSchedule)
                {
                    repoScheduler.Insert(new LookScheduler()
                    {
                        DailyFrequency = dto.LookScheduler.DailyFrequency,
                        DaysOfWeek = dto.LookScheduler.DaysOfWeek,
                        EndDate = dto.LookScheduler.EndDate,
                        ExactDateOfMonth = dto.LookScheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.LookScheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.LookScheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.LookScheduler.Frequency,
                        MonthlyOccurrence = dto.LookScheduler.MonthlyOccurrence,
                        Name = dto.LookScheduler.Name,
                        OccursEveryTimeUnit = dto.LookScheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.LookScheduler.OccursEveryValue,
                        RecurseEvery = dto.LookScheduler.RecurseEvery,
                        StartDate = dto.LookScheduler.StartDate.Value,
                        TimeEnd = dto.LookScheduler.TimeEnd,
                        TimeStart = dto.LookScheduler.TimeStart.Value,
                        Type = dto.LookScheduler.Type,
                        ValidDays = dto.LookScheduler.ValidDays,
                        LookId = dto.Id.Value
                    });
                }


                if (look.LookTeam != null)
                {
                    var existingTeam = look.LookTeam.Where(x => dto.LookTeam.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedTeam = look.LookTeam.Where(x => !dto.LookTeam.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedTeam = dto.LookTeam.Where(x => !look.LookTeam.Any(m => m.Id == x.Id)).ToList();


                    if (deletedTeam.Any())
                    {
                        this.repoTeam.DeleteRange(deletedTeam);
                    }

                    if (existingTeam.Any())
                    {
                        foreach (var e in existingTeam)
                        {
                            var record = dto.LookTeam.FirstOrDefault(a => a.Id == e.Id);
                            e.TeamId = record.TeamId;
                            repoTeam.Update(e);
                        }
                    }
                    if (insertedTeam.Any())
                    {
                        List<LookTeam> addrecords = insertedTeam.Select(a => new LookTeam()
                        {
                            LookId = dto.Id.Value,
                            TeamId = a.TeamId,
                        }).ToList();

                        await repoTeam.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.LookUser != null)
                {
                    var existingUser = look.LookUser.Where(x => dto.LookUser.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedUser = look.LookUser.Where(x => !dto.LookUser.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedUser = dto.LookUser.Where(x => !look.LookUser.Any(m => m.Id == x.Id)).ToList();


                    if (deletedUser.Any())
                    {
                        this.repoUser.DeleteRange(deletedUser);
                    }

                    if (existingUser.Any())
                    {
                        foreach (var e in existingUser)
                        {
                            var record = dto.LookUser.FirstOrDefault(a => a.Id == e.Id);
                            e.UserId = record.UserId;
                            repoUser.Update(e);
                        }
                    }
                    if (insertedUser.Any())
                    {
                        List<LookUser> addrecords = insertedUser.Select(a => new LookUser()
                        {
                            LookId = dto.Id.Value,
                            UserId = a.UserId,
                        }).ToList();

                        await repoUser.InsertRangeAsyn(addrecords);
                    }
                }

                if (look.LookGroup != null)
                {
                    var existingGroup = look.LookGroup.Where(x => dto.LookGroup.Any(scdet => scdet.Id == x.Id)).ToList();
                    var deletedGroup = look.LookGroup.Where(x => !dto.LookGroup.Any(scdet => scdet.Id == x.Id)).ToList();
                    var insertedGroup = dto.LookGroup.Where(x => !look.LookGroup.Any(m => m.Id == x.Id)).ToList();


                    if (deletedGroup.Any())
                    {
                        foreach (var item in deletedGroup)
                        {
                            this.repoGroupPlayer.DeleteRange(item.LookGroupPlayer.ToList());
                        }

                        this.repoGroup.DeleteRange(deletedGroup);
                    }

                    if (existingGroup.Any())
                    {
                        foreach (var e in existingGroup)
                        {
                            this.repoGroupPlayer.DeleteRange(e.LookGroupPlayer.ToList());
                            var record = dto.LookGroup.FirstOrDefault(a => a.Id == e.Id);
                            e.Name = record.Name;
                            e.LookGroupPlayer = record.LookGroupPlayer.Select(p => new LookGroupPlayer()
                            {
                                UserId = p.UserId,
                                LookGroupId = e.Id
                            }).ToList();
                            repoGroup.Update(e);
                        }
                    }
                    if (insertedGroup.Any())
                    {
                        List<LookGroup> addrecords = insertedGroup.Select(a => new LookGroup()
                        {
                            LookId = dto.Id.Value,
                            Name = a.Name,
                            LookGroupPlayer = a.LookGroupPlayer.Select(p => new LookGroupPlayer()
                            {
                                UserId = p.UserId
                            }).ToList()
                        }).ToList();

                        await repoGroup.InsertRangeAsyn(addrecords);
                    }
                }

                look.Name = dto.Name;
                look.FromDate = dto.FromDate.Value;
                look.Desciption = dto.Desciption;
                look.GameId = dto.GameId;
                look.TypeId = dto.TypeId;
                look.CalculationType = dto.CalculationType;
                look.ToDate = dto.ToDate.Value;
                look.IsSchedule = dto.IsSchedule;
                look.IsActive = dto.IsActive;
                look.ModifiedDate = DateTime.Now;
                repo.Update(look);
                return dto.Id.Value;
            }
            else
            {
                var lookInfo = new Look()
                {
                    CompanyId = dto.CompanyId.Value,
                    Desciption = dto.Desciption,
                    Name = dto.Name,
                    FromDate = dto.FromDate.Value,
                    ToDate = dto.ToDate.Value,
                    TypeId = dto.TypeId,
                    CalculationType = dto.CalculationType,
                    ModifiedDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    GameId = dto.GameId,
                    IsSchedule = dto.IsSchedule,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    LookAttribute = dto.LookAttribute.Select(s => new LookAttribute()
                    {
                        AttributeId = s.AttributeId
                    }).ToList(),
                    LookGame = dto.LookGame.Select(s => new LookGame()
                    {
                        GameId = s.GameId
                    }).ToList(),
                    LookPlayers = dto.LookPlayers.Select(s => new LookPlayers()
                    {
                        UserId = s.UserId,
                        IsCalculation = s.IsCalculation,
                        IsView = s.IsView
                    }).ToList(),
                    LookSubAttribute = dto.LookSubAttribute.Select(s => new LookSubAttribute()
                    {
                        SubAttributeId = s.SubAttributeId
                    }).ToList(),
                    LookTeam = dto.LookTeam.Select(t => new LookTeam()
                    {
                        TeamId = t.TeamId
                    }).ToList(),
                    LookUser = dto.LookUser.Select(u => new LookUser()
                    {
                        UserId = u.UserId
                    }).ToList(),
                    LookGroup = dto.LookGroup.Select(g => new LookGroup()
                    {
                        Name = g.Name,
                        LookGroupPlayer = g.LookGroupPlayer.Select(p => new LookGroupPlayer()
                        {
                            UserId = p.UserId
                        }).ToList()
                    }).ToList()
                };
                if (dto.IsSchedule)
                {
                    lookInfo.LookScheduler = new LookScheduler()
                    {
                        DailyFrequency = dto.LookScheduler.DailyFrequency,
                        DaysOfWeek = dto.LookScheduler.DaysOfWeek,
                        EndDate = dto.LookScheduler.EndDate,
                        ExactDateOfMonth = dto.LookScheduler.ExactDateOfMonth,
                        ExactWeekdayOfMonth = dto.LookScheduler.ExactWeekdayOfMonth,
                        ExactWeekdayOfMonthEvery = dto.LookScheduler.ExactWeekdayOfMonthEvery,
                        Frequency = dto.LookScheduler.Frequency,
                        MonthlyOccurrence = dto.LookScheduler.MonthlyOccurrence,
                        Name = dto.LookScheduler.Name,
                        OccursEveryTimeUnit = dto.LookScheduler.OccursEveryTimeUnit,
                        OccursEveryValue = dto.LookScheduler.OccursEveryValue,
                        RecurseEvery = dto.LookScheduler.RecurseEvery,
                        StartDate = dto.LookScheduler.StartDate.Value,
                        TimeEnd = dto.LookScheduler.TimeEnd,
                        TimeStart = dto.LookScheduler.TimeStart.Value,
                        Type = dto.LookScheduler.Type,
                        ValidDays = dto.LookScheduler.ValidDays
                    };
                }

                await repo.InsertAsync(lookInfo);
                return lookInfo.Id;
            }

        }
        public List<LookDto> GetAll(int companyId)
        {
            var all = this.repo.GetAllIncluding(x => x.CompanyId == companyId, x => x.Include(m => m.LookScheduler));

            return all.Select(x => new LookDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                TypeId = x.TypeId,
                CalculationType = x.CalculationType,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive,
                CreatedDate = x.AddedDate,
                IsSchedule = x.IsSchedule,
                LookScheduler = x.IsSchedule && x.LookScheduler != null ?
                new LookSchedulerDto()
                {
                    DailyFrequency = x.LookScheduler.DailyFrequency,
                    DaysOfWeek = x.LookScheduler.DaysOfWeek,
                    Frequency = x.LookScheduler.Frequency,
                    Type = x.LookScheduler.Type
                }
                : new LookSchedulerDto()
            }).ToList();

        }
        public List<LookDto> GetAll(int companyId, int? userId, int gameId)
        {
            var all = this.repo.GetAllIncluding(x => x.CompanyId == companyId && x.GameId == gameId && (userId.HasValue ? x.CreatedBy == userId : true), x => x.Include(m => m.LookScheduler));

            return all.Select(x => new LookDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                TypeId = x.TypeId,
                CalculationType = x.CalculationType,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive,
                CreatedDate = x.AddedDate,
                IsSchedule = x.IsSchedule,
                CreatedBy = x.CreatedBy,
                LookScheduler = x.IsSchedule && x.LookScheduler != null ?
                new LookSchedulerDto()
                {
                    DailyFrequency = x.LookScheduler.DailyFrequency,
                    DaysOfWeek = x.LookScheduler.DaysOfWeek,
                    Frequency = x.LookScheduler.Frequency,
                    Type = x.LookScheduler.Type
                }
                : new LookSchedulerDto()
            }).ToList();

        }


        public List<LookDto> GetAllByGame(int companyId, int? userId, int gameId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.GameId == gameId && (userId.HasValue ? x.CreatedBy == userId : true) && x.IsActive).Select(x => new LookDto()
            {
                Name = x.Name,
                Desciption = x.Desciption,
                Id = x.Id,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                TypeId = x.TypeId,
                CalculationType = x.CalculationType,
                CompanyId = x.CompanyId,
                GameId = x.GameId,
                IsActive = x.IsActive
            }).ToList();

        }

        public List<SelectedItemDto> GetAllDrop(int gid, int companyId, int userId)
        {
            var currentDate = DateTime.Now.Date;
            return repo.GetAllList(x => x.CompanyId == companyId && x.IsActive && x.GameId == gid && (x.LookPlayers.Any(a => a.UserId == userId) || x.LookGroup.Any(a => a.LookGroupPlayer
            .Any(b => b.UserId == userId))) && x.FromDate <= currentDate && x.ToDate >= currentDate).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
        }

        public List<SelectedItemDto> GetAllDrop(int gid, int companyId)
        {

            return repo.GetAllList(x => x.CompanyId == companyId && x.IsActive && (x.GameId == gid || x.LookGame.Any(a => a.GameId == gid))).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
        }

        public List<SelectedItemDto> GetAllDropPrimission(int gid, int companyId, int userId)
        {
            DateTime currentdate = DateTime.Now.Date;

            return repo.GetAllList(x => x.CompanyId == companyId && x.IsActive && x.LookPlayers.Any(a => a.UserId == userId) && (x.GameId == gid || x.LookGame.Any(a => a.GameId == gid)) && x.FromDate <= currentdate && x.ToDate >= currentdate).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
        }

        public List<SelectedItemDto> GetAttributeDrop(int lookId)
        {

            return repoAttribute.GetAllIncluding(x => x.LookId == lookId, x => x.Include(m => m.Attribute)).Select(x => new SelectedItemDto()
            {
                Name = x.Attribute.Name,
                Id = x.AttributeId.ToString()
            }).ToList();
        }
        public List<SelectedItemDto> GetAttributeDropByfilter(int filterId)
        {
            List<int> attreIds = repoFilterAttributes.GetAllList(x => x.FilterId == filterId).Select(x => x.AttributeId).ToList();
            return repoAtAttribute.GetAllList(x => attreIds.Contains(x.Id)).Select(x => new SelectedItemDto()
            {
                Name = x.Name,
                Id = x.Id.ToString()
            }).ToList();
        }

        public List<SelectedItemDto> GetGroupAllDrop(int? lookId, int? filterId, int? attributeId)
        {
            List<SelectedItemDto> response = new List<SelectedItemDto>();
            if (!filterId.HasValue && attributeId.HasValue)
            {
                return repoAtSubAttribute.GetAllList(x => x.AttributeId == attributeId).Select(x => new SelectedItemDto()
                {
                    Name = x.Name,
                    Id = "subattr_" + x.Id
                }).ToList();
            }
            else
            if (!filterId.HasValue)
            {
                return repoGroup.GetAllList(x => x.LookId == lookId).Select(x => new SelectedItemDto()
                {
                    Name = x.Name,
                    Id = "group_" + x.Id
                }).ToList();
            }
            else if (lookId.HasValue)
            {

                var filter = repoFilter.Get(filterId.Value);
                if (filter.IsSelf)
                {
                    var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();

                    var attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                    response = attrList.Select(x => new SelectedItemDto()
                    {
                        Name = x.Name,
                        Id = "attr_" + x.Id
                    }).ToList();
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    response = repo_Team.GetAllList(x => teamIdList.Contains(x.Id)).Select(x => new SelectedItemDto()
                    {
                        Name = x.Name,
                        Id = "team_" + x.Id
                    }).ToList();

                    var userListList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userListList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView).Select(s => s.UserId).ToList());
                    userListList = userListList.Distinct().ToList();
                    response.AddRange(repoUserLogin.GetAllList(x => userListList.Contains(x.Id)).Select(x => new SelectedItemDto()
                    {
                        Name = x.Fname + " " + x.Lname,
                        Id = "user_" + x.Id
                    }).ToList());
                }

            }
            else if (filterId.HasValue)
            {
                var filter = repoFilter.Get(filterId.Value);
                if (filter.IsSelf)
                {
                    var attrbutterList = repoFilterAttributes.GetAll().Where(y => y.FilterId == filterId).Select(s => s.AttributeId).ToList();

                    var attrList = repoAtAttribute.GetAll().Where(y => attrbutterList.Contains(y.Id)).ToList();
                    response = attrList.Select(x => new SelectedItemDto()
                    {
                        Name = x.Name,
                        Id = "attr_" + x.Id
                    }).ToList();
                }
                else
                {
                    var teamIdList = repoFilterEmotionsFrom.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    response = repo_Team.GetAllList(x => teamIdList.Contains(x.Id)).Select(x => new SelectedItemDto()
                    {
                        Name = x.Name,
                        Id = "team_" + x.Id
                    }).ToList();

                    var userListList = repoFilterEmotionsFromP.GetAll().Where(y => y.FilterId == filterId).Select(s => s.FromId).ToList();
                    userListList.AddRange(repoFilterFilterPlayers.GetAll().Where(y => y.FilterId == filterId && y.IsView).Select(s => s.UserId).ToList());
                    userListList = userListList.Distinct().ToList();
                    response.AddRange(repoUserLogin.GetAllList(x => userListList.Contains(x.Id)).Select(x => new SelectedItemDto()
                    {
                        Name = x.Fname + " " + x.Lname,
                        Id = "user_" + x.Id
                    }).ToList());
                }
            }
            return response;

        }

        public LookFeebbackDto GetLookFeedbackByLookId(int lookId, int companyId, List<int> gameIds)
        {
            var look = repo.GetIncludingById(x => x.Id == lookId, x => x.Include(m => m.Game));
            LookFeebbackDto dto = new LookFeebbackDto()
            {
                CalculationType = look.CalculationType,
                Desciption = look.Desciption,
                FromDate = look.FromDate,
                GameId = look.GameId,
                Id = look.Id,
                Name = look.Name,
                ToDate = look.ToDate,
                TypeId = look.TypeId,
                Game = look.Game.Name
            };
            dto.AttributeList = repoAttribute.GetAll(x => x.LookId == lookId && x.Look.CompanyId == companyId).Select(s => new LookFeebbackAttributeDto()
            {
                AttributeId = s.AttributeId,
                Name = s.Attribute.Name,
                Desciption= s.Attribute.Desciption
            }).ToList();

            dto.SubAttributeList = repoSubAttribute.GetAllIncluding(x => x.LookId == lookId && x.Look.CompanyId == companyId, x => x.Include(m => m.SubAttribute)).Select(s => new LookFeebbackSubAttributeDto()
            {
                AttributeId = s.SubAttribute.AttributeId,
                Name = s.SubAttribute.Name,
                SubAttributeId = s.SubAttributeId,
                SubAttributeDesc = s.SubAttribute.Desciption,
                IsQuantity = s.SubAttribute.IsQuantity,
                Unit = s.SubAttribute.UnitId.HasValue ? ((UnitType)s.SubAttribute.UnitId.Value).GetEnumDescription() : ""
            }).ToList();


            if (look.TypeId == (int)LookType.Game)
            {
                List<int> gameId = repoLookGame.GetAll().Where(x => x.LookId == lookId && x.Look.CompanyId == companyId && (gameIds.Any() ? gameIds.Contains(x.GameId) : true)).Select(s => s.GameId).ToList();

                var list = gameRepo.GetAll().Where(x => x.CompanyId == companyId && gameId.Contains(x.Id)).OrderBy(o => !o.ParentId.HasValue ? 0 : o.ParentId.Value).Select(x => new LookFeebbackGameDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ParentId = x.ParentId
                }).ToList();


                List<LookFeebbackGameDto> response = list.Where(x => !list.Any(l => l.Id == x.ParentId)).Select(x => new LookFeebbackGameDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    ParentId = x.ParentId
                }).ToList();

                response.ForEach(x =>
                {
                    x.ChildGame = GetChild(list, x.Id);
                });
                dto.GameList = response;
            }
            else if (look.TypeId == (int)LookType.Team)
            {
                List<LookFeebbackGameDto> response = repoTeam.GetAll().Where(x => x.LookId == lookId && (gameIds.Any() ? gameIds.Contains(x.TeamId) : true)).Include(x => x.Team).Select(x => new LookFeebbackGameDto()
                {
                    Name = x.Team.Name,
                    Id = x.TeamId,
                }).ToList();
                dto.GameList = response;
            }
            else if (look.TypeId == (int)LookType.User)
            {
                List<LookFeebbackGameDto> response = repoUser.GetAll().Where(x => x.LookId == lookId && (gameIds.Any() ? gameIds.Contains(x.UserId) : true)).Include(x => x.User).Select(x => new LookFeebbackGameDto()
                {
                    Name = x.User.Fname + " " + x.User.Lname,
                    Id = x.UserId,
                }).ToList();
                dto.GameList = response;
            }
            return dto;

        }

        public List<GameGridDto> GetLookByLookId(int? lookId, List<int> teamIds)
        {

            List<GameGridDto> response = repoTeam.GetAll().Where(x => (lookId.HasValue ? x.LookId == lookId : true) && (teamIds.Any() ? teamIds.Contains(x.TeamId) : true)).Include(x => x.Team).Select(x => new GameGridDto()
            {
                Name = x.Team.Name,
                Id = x.TeamId,
            }).Distinct().ToList();

            return response;

        }
        public List<GameGridDto> GetLookByLookId(int? lookId)
        {

            List<GameGridDto> response = repoTeam.GetAll().Where(x => (lookId.HasValue ? x.LookId == lookId : true)).Include(x => x.Team).Select(x => new GameGridDto()
            {
                Name = x.Team.Name,
                Id = x.TeamId,
            }).Distinct().ToList();

            return response;

        }

        public List<GameGridDto> GetUserByLookId(int? lookId, List<int> userIds)
        {

            List<GameGridDto> response = repoUser.GetAll().Where(x => (lookId.HasValue ? x.LookId == lookId : true) && (userIds.Any() ? userIds.Contains(x.UserId) : true)).Include(x => x.User).Select(x => new GameGridDto()
            {
                Name = x.User.Fname + " " + x.User.Lname,
                Id = x.UserId,
            }).Distinct().ToList();

            return response;

        }
        public List<GameGridDto> GetUserByLookId(int? lookId)
        {

            List<GameGridDto> response = repoUser.GetAll().Where(x => (lookId.HasValue ? x.LookId == lookId : true)).Include(x => x.User).Select(x => new GameGridDto()
            {
                Name = x.User.Fname + " " + x.User.Lname,
                Id = x.UserId,
            }).Distinct().ToList();

            return response;

        }
        private List<LookFeebbackGameDto> GetChild(List<LookFeebbackGameDto> list, int parentId)
        {
            List<LookFeebbackGameDto> response = list.Where(x => x.ParentId == parentId).ToList();
            response.ForEach(x =>
            {
                x.ChildGame = GetChild(list, x.Id);
            });

            return response;

        }

        public void DeleteBy(int id)
        {
            var context = this.repo.GetContext();
            try
            {
                context.Database.BeginTransaction();
                var x = this.repo.GetIncludingById(x => x.Id == id, x => x.Include(m => m.LookAttribute)
           .Include(m => m.LookSubAttribute).Include(m => m.LookGame).Include(m => m.LookPlayers).Include(m => m.LookScheduler)
           .Include(m => m.LookGroup).Include("LookGroup.LookGroupPlayer").Include(m => m.LookTeam).Include(m => m.LookUser));

                repoAttribute.DeleteRange(x.LookAttribute.ToList());
                repoSubAttribute.DeleteRange(x.LookSubAttribute.ToList());
                repoLookGame.DeleteRange(x.LookGame.ToList());
                repoPlayers.DeleteRange(x.LookPlayers.ToList());
                repoScheduler.Delete(x.LookScheduler);
                foreach (var item in x.LookGroup)
                {
                    repoGroupPlayer.DeleteRange(item.LookGroupPlayer.ToList());

                }
                repoGroup.DeleteRange(x.LookGroup.ToList());
                repoGroup.DeleteRange(x.LookGroup.ToList());
                repoTeam.DeleteRange(x.LookTeam.ToList());
                repoUser.DeleteRange(x.LookUser.ToList());
                repo.Delete(x);

                context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.Database.RollbackTransaction();
                throw;
            }
        }

        public DataTable GetCompanyLookNotification(DateTime currentDateTime)
        {

            DataSet ds = SqlHelper.ExecuteDataset(this.Connectionstring, "[dbo].[GetCompanyLookNotification]",
                          new SqlParameter("@currentDateTime", currentDateTime));

            DataTable dt = null;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        public List<LookPlayersDto> GetCompanyLookParticipantNotification(DateTime currentDateTime)
        {
            DataTable dt = GetCompanyLookNotification(currentDateTime);
            List<int> sessionId = new List<int>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    sessionId.Add(int.Parse(row[0].ToString()));
                }

                return repoPlayers.GetAllList(x => sessionId.Contains(x.LookId)).Select(x => new LookPlayersDto()
                {
                    Id = x.LookId,
                    UserId = x.UserId
                }).ToList();
            }
            else
            {
                return new List<LookPlayersDto>();
            }

        }

        public async Task<GridResult> GetPaggedListAsync(GridParameterModel parameters)
        {


            IQueryable<Look> query = this.repo.GetAllIncluding(x => x.CompanyId == parameters.CompanyId && x.GameId == parameters.GameId && (parameters.UserType == (int)UserType.User ? x.CreatedBy == parameters.UserId : true), x => x.Include(m => m.LookScheduler).Include(m => m.CreatedByNavigation));

            var result = await CustomPredicate.ToPaggedListAsync(query, parameters);
            List<LookGridDto> data = new List<LookGridDto>();
            foreach (Look x in result.Data)
            {
                data.Add(new LookGridDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    IsActive = x.IsActive ? "Active" : "In active",
                    CreatedDate = x.AddedDate.ToString("dd/MM/yyyy"),
                    CreatedBy = x.CreatedBy.HasValue ? x.CreatedByNavigation.Fname + " " + x.CreatedByNavigation.Lname : string.Empty,
                    Frequency = x.IsSchedule && x.LookScheduler != null && x.LookScheduler.Frequency != null ? ((ScheduleFrequency)x.LookScheduler.Frequency).ToString() : string.Empty

                });

            }
            result.Data = data.ToList<object>();
            return result;
        }
    }
}
