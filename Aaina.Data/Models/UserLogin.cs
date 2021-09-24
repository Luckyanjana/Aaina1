using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            Filter = new HashSet<Filter>();
            FormBuilder = new HashSet<FormBuilder>();
            Game = new HashSet<Game>();
            GameFeedback = new HashSet<GameFeedback>();
            GamePlayer = new HashSet<GamePlayer>();
            Look = new HashSet<Look>();
            LookGroupPlayer = new HashSet<LookGroupPlayer>();
            LookPlayers = new HashSet<LookPlayers>();
            LookUser = new HashSet<LookUser>();
            NotificationReminderSendByNavigation = new HashSet<NotificationReminder>();
            NotificationReminderSendToNavigation = new HashSet<NotificationReminder>();
            PlayAccountable = new HashSet<Play>();
            PlayDelegateAccountable = new HashSet<PlayDelegate>();
            PlayDelegateDelegate = new HashSet<PlayDelegate>();
            PlayDependancy = new HashSet<Play>();
            PlayPersonInvolved = new HashSet<PlayPersonInvolved>();
            Poll = new HashSet<Poll>();
            PollFeedback = new HashSet<PollFeedback>();
            PollParticipants = new HashSet<PollParticipants>();
            PreSessionDelegateDecisionMaker = new HashSet<PreSessionDelegate>();
            PreSessionDelegateDelegate = new HashSet<PreSessionDelegate>();
            PreSessionGroupDetails = new HashSet<PreSessionGroupDetails>();
            PreSessionParticipant = new HashSet<PreSessionParticipant>();
            PreSessionStatus = new HashSet<PreSessionStatus>();
            PushNotificationToken = new HashSet<PushNotificationToken>();
            ReportGiveCreatedByNavigation = new HashSet<ReportGive>();
            ReportGiveDetails = new HashSet<ReportGiveDetails>();
            ReportGiveRemarkedByNavigation = new HashSet<ReportGive>();
            ReportTemplate = new HashSet<ReportTemplate>();
            ReportTemplateUser = new HashSet<ReportTemplateUser>();
            SessionParticipant = new HashSet<SessionParticipant>();
            StatusFeedback = new HashSet<StatusFeedback>();
            StatusSnooze = new HashSet<StatusSnooze>();
            StatusUserBy = new HashSet<StatusUserBy>();
            StatusUserFor = new HashSet<StatusUserFor>();
            Team = new HashSet<Team>();
            TeamFeedback = new HashSet<TeamFeedback>();
            TeamPlayer = new HashSet<TeamPlayer>();
            UserFeedback = new HashSet<UserFeedback>();
            UserFeedbackDetails = new HashSet<UserFeedbackDetails>();
            WeightagePresetDetails = new HashSet<WeightagePresetDetails>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserType { get; set; }
        public string Email { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsEmailVerify { get; set; }
        public bool IsActive { get; set; }
        public string PasswordResetLink { get; set; }
        public DateTime? LinkExpiredDate { get; set; }
        public bool IsForgotVerified { get; set; }
        public DateTime? Dob { get; set; }
        public int? Gender { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string UserName { get; set; }
        public int PlayerType { get; set; }
        public string DriveId { get; set; }

        public virtual Company Company { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Filter> Filter { get; set; }
        public virtual ICollection<FormBuilder> FormBuilder { get; set; }
        public virtual ICollection<Game> Game { get; set; }
        public virtual ICollection<GameFeedback> GameFeedback { get; set; }
        public virtual ICollection<GamePlayer> GamePlayer { get; set; }
        public virtual ICollection<Look> Look { get; set; }
        public virtual ICollection<LookGroupPlayer> LookGroupPlayer { get; set; }
        public virtual ICollection<LookPlayers> LookPlayers { get; set; }
        public virtual ICollection<LookUser> LookUser { get; set; }
        public virtual ICollection<NotificationReminder> NotificationReminderSendByNavigation { get; set; }
        public virtual ICollection<NotificationReminder> NotificationReminderSendToNavigation { get; set; }
        public virtual ICollection<Play> PlayAccountable { get; set; }
        public virtual ICollection<PlayDelegate> PlayDelegateAccountable { get; set; }
        public virtual ICollection<PlayDelegate> PlayDelegateDelegate { get; set; }
        public virtual ICollection<Play> PlayDependancy { get; set; }
        public virtual ICollection<PlayPersonInvolved> PlayPersonInvolved { get; set; }
        public virtual ICollection<Poll> Poll { get; set; }
        public virtual ICollection<PollFeedback> PollFeedback { get; set; }
        public virtual ICollection<PollParticipants> PollParticipants { get; set; }
        public virtual ICollection<PreSessionDelegate> PreSessionDelegateDecisionMaker { get; set; }
        public virtual ICollection<PreSessionDelegate> PreSessionDelegateDelegate { get; set; }
        public virtual ICollection<PreSessionGroupDetails> PreSessionGroupDetails { get; set; }
        public virtual ICollection<PreSessionParticipant> PreSessionParticipant { get; set; }
        public virtual ICollection<PreSessionStatus> PreSessionStatus { get; set; }
        public virtual ICollection<PushNotificationToken> PushNotificationToken { get; set; }
        public virtual ICollection<ReportGive> ReportGiveCreatedByNavigation { get; set; }
        public virtual ICollection<ReportGiveDetails> ReportGiveDetails { get; set; }
        public virtual ICollection<ReportGive> ReportGiveRemarkedByNavigation { get; set; }
        public virtual ICollection<ReportTemplate> ReportTemplate { get; set; }
        public virtual ICollection<ReportTemplateUser> ReportTemplateUser { get; set; }
        public virtual ICollection<SessionParticipant> SessionParticipant { get; set; }
        public virtual ICollection<StatusFeedback> StatusFeedback { get; set; }
        public virtual ICollection<StatusSnooze> StatusSnooze { get; set; }
        public virtual ICollection<StatusUserBy> StatusUserBy { get; set; }
        public virtual ICollection<StatusUserFor> StatusUserFor { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<TeamFeedback> TeamFeedback { get; set; }
        public virtual ICollection<TeamPlayer> TeamPlayer { get; set; }
        public virtual ICollection<UserFeedback> UserFeedback { get; set; }
        public virtual ICollection<UserFeedbackDetails> UserFeedbackDetails { get; set; }
        public virtual ICollection<WeightagePresetDetails> WeightagePresetDetails { get; set; }
    }
}
