using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Aaina.Data.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attribute> Attribute { get; set; }
        public virtual DbSet<ChatGroup> ChatGroup { get; set; }
        public virtual DbSet<ChatGroupUser> ChatGroupUser { get; set; }
        public virtual DbSet<ChatMessage> ChatMessage { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Filter> Filter { get; set; }
        public virtual DbSet<FilterAttributes> FilterAttributes { get; set; }
        public virtual DbSet<FilterEmotionsFor> FilterEmotionsFor { get; set; }
        public virtual DbSet<FilterEmotionsFrom> FilterEmotionsFrom { get; set; }
        public virtual DbSet<FilterEmotionsFromP> FilterEmotionsFromP { get; set; }
        public virtual DbSet<FilterPlayers> FilterPlayers { get; set; }
        public virtual DbSet<FormBuilder> FormBuilder { get; set; }
        public virtual DbSet<FormBuilderAttribute> FormBuilderAttribute { get; set; }
        public virtual DbSet<FormBuilderAttributeLookUp> FormBuilderAttributeLookUp { get; set; }
        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<GameFeedback> GameFeedback { get; set; }
        public virtual DbSet<GameFeedbackDetails> GameFeedbackDetails { get; set; }
        public virtual DbSet<GameLocation> GameLocation { get; set; }
        public virtual DbSet<GamePlayer> GamePlayer { get; set; }
        public virtual DbSet<Look> Look { get; set; }
        public virtual DbSet<LookAttribute> LookAttribute { get; set; }
        public virtual DbSet<LookGame> LookGame { get; set; }
        public virtual DbSet<LookGroup> LookGroup { get; set; }
        public virtual DbSet<LookGroupPlayer> LookGroupPlayer { get; set; }
        public virtual DbSet<LookPlayers> LookPlayers { get; set; }
        public virtual DbSet<LookScheduler> LookScheduler { get; set; }
        public virtual DbSet<LookSubAttribute> LookSubAttribute { get; set; }
        public virtual DbSet<LookTeam> LookTeam { get; set; }
        public virtual DbSet<LookUser> LookUser { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<NotificationReminder> NotificationReminder { get; set; }
        public virtual DbSet<Play> Play { get; set; }
        public virtual DbSet<PlayDelegate> PlayDelegate { get; set; }
        public virtual DbSet<PlayFeedback> PlayFeedback { get; set; }
        public virtual DbSet<PlayPersonInvolved> PlayPersonInvolved { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<PollFeedback> PollFeedback { get; set; }
        public virtual DbSet<PollParticipants> PollParticipants { get; set; }
        public virtual DbSet<PollQuestion> PollQuestion { get; set; }
        public virtual DbSet<PollQuestionFeedback> PollQuestionFeedback { get; set; }
        public virtual DbSet<PollQuestionOption> PollQuestionOption { get; set; }
        public virtual DbSet<PollReminder> PollReminder { get; set; }
        public virtual DbSet<PollScheduler> PollScheduler { get; set; }
        public virtual DbSet<PostSession> PostSession { get; set; }
        public virtual DbSet<PostSessionAgenda> PostSessionAgenda { get; set; }
        public virtual DbSet<PreSession> PreSession { get; set; }
        public virtual DbSet<PreSessionAgenda> PreSessionAgenda { get; set; }
        public virtual DbSet<PreSessionAgendaDoc> PreSessionAgendaDoc { get; set; }
        public virtual DbSet<PreSessionDelegate> PreSessionDelegate { get; set; }
        public virtual DbSet<PreSessionGroup> PreSessionGroup { get; set; }
        public virtual DbSet<PreSessionGroupDetails> PreSessionGroupDetails { get; set; }
        public virtual DbSet<PreSessionParticipant> PreSessionParticipant { get; set; }
        public virtual DbSet<PreSessionStatus> PreSessionStatus { get; set; }
        public virtual DbSet<PushNotificationToken> PushNotificationToken { get; set; }
        public virtual DbSet<ReportGive> ReportGive { get; set; }
        public virtual DbSet<ReportGiveAttributeValue> ReportGiveAttributeValue { get; set; }
        public virtual DbSet<ReportGiveDetails> ReportGiveDetails { get; set; }
        public virtual DbSet<ReportTemplate> ReportTemplate { get; set; }
        public virtual DbSet<ReportTemplateEntity> ReportTemplateEntity { get; set; }
        public virtual DbSet<ReportTemplateGame> ReportTemplateGame { get; set; }
        public virtual DbSet<ReportTemplateReminder> ReportTemplateReminder { get; set; }
        public virtual DbSet<ReportTemplateScheduler> ReportTemplateScheduler { get; set; }
        public virtual DbSet<ReportTemplateUser> ReportTemplateUser { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleMenuPermission> RoleMenuPermission { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<SessionParticipant> SessionParticipant { get; set; }
        public virtual DbSet<SessionReminder> SessionReminder { get; set; }
        public virtual DbSet<SessionScheduler> SessionScheduler { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<StatusFeedback> StatusFeedback { get; set; }
        public virtual DbSet<StatusFeedbackDetail> StatusFeedbackDetail { get; set; }
        public virtual DbSet<StatusGameBy> StatusGameBy { get; set; }
        public virtual DbSet<StatusReminder> StatusReminder { get; set; }
        public virtual DbSet<StatusScheduler> StatusScheduler { get; set; }
        public virtual DbSet<StatusSnooze> StatusSnooze { get; set; }
        public virtual DbSet<StatusTeamBy> StatusTeamBy { get; set; }
        public virtual DbSet<StatusTeamFor> StatusTeamFor { get; set; }
        public virtual DbSet<StatusUserBy> StatusUserBy { get; set; }
        public virtual DbSet<StatusUserFor> StatusUserFor { get; set; }
        public virtual DbSet<SubAttribute> SubAttribute { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamFeedback> TeamFeedback { get; set; }
        public virtual DbSet<TeamFeedbackDetails> TeamFeedbackDetails { get; set; }
        public virtual DbSet<TeamPlayer> TeamPlayer { get; set; }
        public virtual DbSet<UserFeedback> UserFeedback { get; set; }
        public virtual DbSet<UserFeedbackDetails> UserFeedbackDetails { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserMenuPermission> UserMenuPermission { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<Weightage> Weightage { get; set; }
        public virtual DbSet<WeightagePreset> WeightagePreset { get; set; }
        public virtual DbSet<WeightagePresetDetails> WeightagePresetDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=88.99.153.130,1533;Database=new_aaiina;User ID=sa;Password=LWMu(9j%HcZ64;Integrated Security=false;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Attribute)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Attribute)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<ChatGroup>(entity =>
            {
                entity.ToTable("Chat_Group");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DeletedDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ChatGroupUser>(entity =>
            {
                entity.ToTable("Chat_Group_User");

                entity.Property(e => e.DeletedDate).HasColumnType("date");

                entity.Property(e => e.JoinDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("Chat_Message");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.SendDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Desciption).HasMaxLength(50);

                entity.Property(e => e.DriveId).HasMaxLength(250);

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Filter>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Filter)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Filter)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Filter)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FilterAttributes>(entity =>
            {
                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterAttributes)
                    .HasForeignKey(d => d.FilterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FilterEmotionsFor>(entity =>
            {
                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterEmotionsFor)
                    .HasForeignKey(d => d.FilterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FilterEmotionsFrom>(entity =>
            {
                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterEmotionsFrom)
                    .HasForeignKey(d => d.FilterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FilterEmotionsFromP>(entity =>
            {
                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterEmotionsFromP)
                    .HasForeignKey(d => d.FilterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FilterPlayers>(entity =>
            {
                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterPlayers)
                    .HasForeignKey(d => d.FilterId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FormBuilder>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Footer).HasMaxLength(4000);

                entity.Property(e => e.Header).HasMaxLength(4000);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.FormBuilder)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.FormBuilder)
                    .HasForeignKey(d => d.CreatedBy);
            });

            modelBuilder.Entity<FormBuilderAttribute>(entity =>
            {
                entity.Property(e => e.AttributeName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.DbcolumnName)
                    .HasColumnName("DBColumnName")
                    .HasMaxLength(100);

                entity.HasOne(d => d.FormBuilder)
                    .WithMany(p => p.FormBuilderAttribute)
                    .HasForeignKey(d => d.FormBuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FormBuilderAttributeLookUp>(entity =>
            {
                entity.Property(e => e.OptionName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.FormBuilderAttribute)
                    .WithMany(p => p.FormBuilderAttributeLookUp)
                    .HasForeignKey(d => d.FormBuilderAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ClientName).HasMaxLength(250);

                entity.Property(e => e.ContactNumber).HasMaxLength(12);

                entity.Property(e => e.ContactPerson).HasMaxLength(250);

                entity.Property(e => e.Desciption).HasMaxLength(50);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.Location).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Todate)
                    .HasColumnName("TODate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Game)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Game)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<GameFeedback>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedbackDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.GameFeedback)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.GameFeedback)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GameFeedback)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<GameFeedbackDetails>(entity =>
            {
                entity.Property(e => e.Feedback)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Percentage).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.GameFeedbackDetails)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.GameFeedback)
                    .WithMany(p => p.GameFeedbackDetails)
                    .HasForeignKey(d => d.GameFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GameFeedbackDetails)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubAttribute)
                    .WithMany(p => p.GameFeedbackDetails)
                    .HasForeignKey(d => d.SubAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<GameLocation>(entity =>
            {
                entity.Property(e => e.Location).HasMaxLength(1000);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GameLocation)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<GamePlayer>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.GamePlayer)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GamePlayer)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.GamePlayer)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GamePlayer)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Look>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Look)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Look)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Look)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<LookAttribute>(entity =>
            {
                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.LookAttribute)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookAttribute)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookGame>(entity =>
            {
                entity.HasOne(d => d.Game)
                    .WithMany(p => p.LookGame)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookGame)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookGroup>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookGroup)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookGroupPlayer>(entity =>
            {
                entity.HasOne(d => d.LookGroup)
                    .WithMany(p => p.LookGroupPlayer)
                    .HasForeignKey(d => d.LookGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LookGroupPlayer)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookPlayers>(entity =>
            {
                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookPlayers)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LookPlayers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookScheduler>(entity =>
            {
                entity.HasKey(e => e.LookId)
                    .HasName("PK__LookSche__F090066C67E14355");

                entity.Property(e => e.LookId).ValueGeneratedNever();

                entity.Property(e => e.DaysOfWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.ValidDays)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Look)
                    .WithOne(p => p.LookScheduler)
                    .HasForeignKey<LookScheduler>(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookSubAttribute>(entity =>
            {
                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookSubAttribute)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubAttribute)
                    .WithMany(p => p.LookSubAttribute)
                    .HasForeignKey(d => d.SubAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookTeam>(entity =>
            {
                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookTeam)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.LookTeam)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LookUser>(entity =>
            {
                entity.HasOne(d => d.Look)
                    .WithMany(p => p.LookUser)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LookUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<NotificationReminder>(entity =>
            {
                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.ReadDate).HasColumnType("datetime");

                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.HasOne(d => d.SendByNavigation)
                    .WithMany(p => p.NotificationReminderSendByNavigation)
                    .HasForeignKey(d => d.SendBy);

                entity.HasOne(d => d.SendToNavigation)
                    .WithMany(p => p.NotificationReminderSendToNavigation)
                    .HasForeignKey(d => d.SendTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NotificationReminder_UserLogin_UserId");
            });

            modelBuilder.Entity<Play>(entity =>
            {
                entity.Property(e => e.ActualEndDate).HasColumnType("datetime");

                entity.Property(e => e.ActualStartDate).HasColumnType("datetime");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.AddedOn).HasColumnType("datetime");

                entity.Property(e => e.Comments).HasMaxLength(2000);

                entity.Property(e => e.CoordinateEmotion).HasMaxLength(500);

                entity.Property(e => e.DeadlineDate).HasColumnType("datetime");

                entity.Property(e => e.DecisionMakerEmotion).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Emotion).HasMaxLength(250);

                entity.Property(e => e.GameType).HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Phoemotion)
                    .HasColumnName("PHOEmotion")
                    .HasMaxLength(250);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Accountable)
                    .WithMany(p => p.PlayAccountable)
                    .HasForeignKey(d => d.AccountableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Play)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Dependancy)
                    .WithMany(p => p.PlayDependancy)
                    .HasForeignKey(d => d.DependancyId);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.PlayGame)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId);

                entity.HasOne(d => d.SubGame)
                    .WithMany(p => p.PlaySubGame)
                    .HasForeignKey(d => d.SubGameId)
                    .HasConstraintName("FK_Play_SubGame_SubGameId");
            });

            modelBuilder.Entity<PlayDelegate>(entity =>
            {
                entity.Property(e => e.DelegateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.HasOne(d => d.Accountable)
                    .WithMany(p => p.PlayDelegateAccountable)
                    .HasForeignKey(d => d.AccountableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Delegate)
                    .WithMany(p => p.PlayDelegateDelegate)
                    .HasForeignKey(d => d.DelegateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Play)
                    .WithMany(p => p.PlayDelegate)
                    .HasForeignKey(d => d.PlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PlayFeedback>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(4000);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Play)
                    .WithMany(p => p.PlayFeedback)
                    .HasForeignKey(d => d.PlayId)
                    .HasConstraintName("FK_PlayFeedback_Play");
            });

            modelBuilder.Entity<PlayPersonInvolved>(entity =>
            {
                entity.HasOne(d => d.Play)
                    .WithMany(p => p.PlayPersonInvolved)
                    .HasForeignKey(d => d.PlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PlayPersonInvolved)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Poll)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Poll)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.PollGame)
                    .HasForeignKey(d => d.GameId);

                entity.HasOne(d => d.SubGame)
                    .WithMany(p => p.PollSubGame)
                    .HasForeignKey(d => d.SubGameId);
            });

            modelBuilder.Entity<PollFeedback>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(500);

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollFeedback)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PollFeedback)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollParticipants>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollParticipants)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PollParticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollQuestion>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollQuestion)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollQuestionFeedback>(entity =>
            {
                entity.Property(e => e.Remark).HasMaxLength(500);

                entity.HasOne(d => d.PollFeedback)
                    .WithMany(p => p.PollQuestionFeedback)
                    .HasForeignKey(d => d.PollFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PollQuestion)
                    .WithMany(p => p.PollQuestionFeedback)
                    .HasForeignKey(d => d.PollQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PollQuestionOption)
                    .WithMany(p => p.PollQuestionFeedback)
                    .HasForeignKey(d => d.PollQuestionOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollQuestionOption>(entity =>
            {
                entity.Property(e => e.FilePath).HasMaxLength(250);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.HasOne(d => d.PollQuestion)
                    .WithMany(p => p.PollQuestionOption)
                    .HasForeignKey(d => d.PollQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollReminder>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.PollReminder)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PollScheduler>(entity =>
            {
                entity.HasKey(e => e.PollId)
                    .HasName("PK__PollSche__E1949E6A8D96AC2F");

                entity.Property(e => e.PollId).ValueGeneratedNever();

                entity.Property(e => e.DaysOfWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.ValidDays)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Poll)
                    .WithOne(p => p.PollScheduler)
                    .HasForeignKey<PollScheduler>(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PostSession>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.PostSession)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PostSessionAgenda>(entity =>
            {
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(d => d.Play)
                    .WithMany(p => p.PostSessionAgenda)
                    .HasForeignKey(d => d.PlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PostSession)
                    .WithMany(p => p.PostSessionAgenda)
                    .HasForeignKey(d => d.PostSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSession>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.PreSession)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionAgenda>(entity =>
            {
                entity.HasOne(d => d.Play)
                    .WithMany(p => p.PreSessionAgenda)
                    .HasForeignKey(d => d.PlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PreSession)
                    .WithMany(p => p.PreSessionAgenda)
                    .HasForeignKey(d => d.PreSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionAgendaDoc>(entity =>
            {
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.PreSessionAgenda)
                    .WithMany(p => p.PreSessionAgendaDoc)
                    .HasForeignKey(d => d.PreSessionAgendaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PreSessionAgendaDoc_PreSessionAgenda_PreSessionId");
            });

            modelBuilder.Entity<PreSessionDelegate>(entity =>
            {
                entity.Property(e => e.DelegateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.HasOne(d => d.DecisionMaker)
                    .WithMany(p => p.PreSessionDelegateDecisionMaker)
                    .HasForeignKey(d => d.DecisionMakerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Delegate)
                    .WithMany(p => p.PreSessionDelegateDelegate)
                    .HasForeignKey(d => d.DelegateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.PreSessionDelegate)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionGroup>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.PreSessionGroup)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionGroupDetails>(entity =>
            {
                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.HasOne(d => d.PreSessionGroup)
                    .WithMany(p => p.PreSessionGroupDetails)
                    .HasForeignKey(d => d.PreSessionGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PreSessionGroupDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionParticipant>(entity =>
            {
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(d => d.PreSession)
                    .WithMany(p => p.PreSessionParticipant)
                    .HasForeignKey(d => d.PreSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PreSessionParticipant)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PreSessionStatus>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ReDateTime)
                    .HasColumnName("Re-DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.DecisionMaker)
                    .WithMany(p => p.PreSessionStatus)
                    .HasForeignKey(d => d.DecisionMakerId);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.PreSessionStatus)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PushNotificationToken>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.PushNotificationToken)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<ReportGive>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReportGiveCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RemarkedByNavigation)
                    .WithMany(p => p.ReportGiveRemarkedByNavigation)
                    .HasForeignKey(d => d.RemarkedBy);

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ReportGive)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGive_Report_ReportId");
            });

            modelBuilder.Entity<ReportGiveAttributeValue>(entity =>
            {
                entity.Property(e => e.AttributeValue).HasMaxLength(1000);

                entity.HasOne(d => d.FormBuilderAttribute)
                    .WithMany(p => p.ReportGiveAttributeValue)
                    .HasForeignKey(d => d.FormBuilderAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LookUp)
                    .WithMany(p => p.ReportGiveAttributeValue)
                    .HasForeignKey(d => d.LookUpId);

                entity.HasOne(d => d.ReportGiveDetail)
                    .WithMany(p => p.ReportGiveAttributeValue)
                    .HasForeignKey(d => d.ReportGiveDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReportGiveDetails>(entity =>
            {
                entity.HasOne(d => d.RemarkedByNavigation)
                    .WithMany(p => p.ReportGiveDetails)
                    .HasForeignKey(d => d.RemarkedBy);

                entity.HasOne(d => d.ReportGive)
                    .WithMany(p => p.ReportGiveDetails)
                    .HasForeignKey(d => d.ReportGiveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportGive_Report_ReportGiveId");
            });

            modelBuilder.Entity<ReportTemplate>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.ReportTemplate)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReportTemplate)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.FormBuilder)
                    .WithMany(p => p.ReportTemplate)
                    .HasForeignKey(d => d.FormBuilderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.ReportTemplate)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<ReportTemplateEntity>(entity =>
            {
                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ReportTemplateEntity)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilterEmotionsFor_ReportTemplate_ReportId");
            });

            modelBuilder.Entity<ReportTemplateGame>(entity =>
            {
                entity.HasOne(d => d.ReportTemplate)
                    .WithMany(p => p.ReportTemplateGame)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReportTemplateReminder>(entity =>
            {
                entity.HasOne(d => d.ReportTemplate)
                    .WithMany(p => p.ReportTemplateReminder)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReportTemplateScheduler>(entity =>
            {
                entity.HasKey(e => e.ReportTemplateId)
                    .HasName("PK__ReportTe__C7EA28068B61602C");

                entity.Property(e => e.ReportTemplateId).ValueGeneratedNever();

                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.DaysOfWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.ValidDays)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Venue)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.ReportTemplate)
                    .WithOne(p => p.ReportTemplateScheduler)
                    .HasForeignKey<ReportTemplateScheduler>(d => d.ReportTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ReportTemplateUser>(entity =>
            {
                entity.HasOne(d => d.ReportTemplate)
                    .WithMany(p => p.ReportTemplateUser)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReportTemplateUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ColorCode).HasMaxLength(50);

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PlayerType).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Role)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<SessionParticipant>(entity =>
            {
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.SessionParticipant)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SessionParticipant)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<SessionReminder>(entity =>
            {
                entity.HasOne(d => d.Session)
                    .WithMany(p => p.SessionReminder)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SessionScheduler>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__SessionS__C9F49290BCAFA240");

                entity.Property(e => e.SessionId).ValueGeneratedNever();

                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.DaysOfWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.MeetingUrl)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.ValidDays)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Venue)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Session)
                    .WithOne(p => p.SessionScheduler)
                    .HasForeignKey<SessionScheduler>(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Status)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Status)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<StatusFeedback>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusFeedback)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StatusFeedback)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusFeedbackDetail>(entity =>
            {
                entity.Property(e => e.Emotion)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Feedback).IsRequired();

                entity.Property(e => e.FeedbackDate).HasColumnType("datetime");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.StatusFeedbackDetailGame)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.StatusFeedback)
                    .WithMany(p => p.StatusFeedbackDetail)
                    .HasForeignKey(d => d.StatusFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusFeedbackDetail_Play_PlayId");

                entity.HasOne(d => d.SubGame)
                    .WithMany(p => p.StatusFeedbackDetailSubGame)
                    .HasForeignKey(d => d.SubGameId);
            });

            modelBuilder.Entity<StatusGameBy>(entity =>
            {
                entity.HasOne(d => d.Game)
                    .WithMany(p => p.StatusGameBy)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusGameBy)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusReminder>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusReminder)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusScheduler>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__StatusSc__C8EE2063BD5ABC08");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.DaysOfWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.ValidDays)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Venue)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Status)
                    .WithOne(p => p.StatusScheduler)
                    .HasForeignKey<StatusScheduler>(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusSnooze>(entity =>
            {
                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusSnooze)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StatusSnooze)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusTeamBy>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusTeamBy)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.StatusTeamBy)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusTeamFor>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusTeamFor)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.StatusTeamFor)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusUserBy>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusUserBy)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StatusUserBy)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StatusUserFor>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusUserFor)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StatusUserFor)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SubAttribute>(entity =>
            {
                entity.Property(e => e.Desciption).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Weightage).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.SubAttribute)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Desciption).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.CreatedBy);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.GameId);
            });

            modelBuilder.Entity<TeamFeedback>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedbackDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TeamFeedback)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.TeamFeedback)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TeamFeedback)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TeamFeedbackDetails>(entity =>
            {
                entity.Property(e => e.Feedback)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Percentage).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.TeamFeedbackDetails)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubAttribute)
                    .WithMany(p => p.TeamFeedbackDetails)
                    .HasForeignKey(d => d.SubAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TeamFeedback)
                    .WithMany(p => p.TeamFeedbackDetails)
                    .HasForeignKey(d => d.TeamFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamFeedbackDetails)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TeamPlayer>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TeamPlayer)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TeamPlayer)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamPlayer)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TeamPlayer)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserFeedback>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedbackDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.UserFeedback)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Look)
                    .WithMany(p => p.UserFeedback)
                    .HasForeignKey(d => d.LookId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFeedback)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserFeedbackDetails>(entity =>
            {
                entity.Property(e => e.Feedback)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Percentage).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.UserFeedbackDetails)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SubAttribute)
                    .WithMany(p => p.UserFeedbackDetails)
                    .HasForeignKey(d => d.SubAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.UserFeedback)
                    .WithMany(p => p.UserFeedbackDetails)
                    .HasForeignKey(d => d.UserFeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFeedbackDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeedbackDetails_User_UserId");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.AvatarUrl).HasMaxLength(250);

                entity.Property(e => e.City).HasMaxLength(250);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.DriveId).HasMaxLength(250);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasColumnName("FName")
                    .HasMaxLength(250);

                entity.Property(e => e.LinkExpiredDate).HasColumnType("datetime");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Mname).HasMaxLength(250);

                entity.Property(e => e.MobileNo).HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PlayerType).HasDefaultValueSql("((1))");

                entity.Property(e => e.SaltKey)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.State).HasMaxLength(250);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserProf__1788CCAC0962665A");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DriveIdEduCert)
                    .HasColumnName("DriveId_EduCert")
                    .HasMaxLength(250);

                entity.Property(e => e.DriveIdExpCert)
                    .HasColumnName("DriveId_ExpCert")
                    .HasMaxLength(250);

                entity.Property(e => e.DriveIdIdProffFile)
                    .HasColumnName("DriveId_IdProffFile")
                    .HasMaxLength(250);

                entity.Property(e => e.DriveIdOther)
                    .HasColumnName("DriveId_Other")
                    .HasMaxLength(250);

                entity.Property(e => e.DriveIdPoliceVerification)
                    .HasColumnName("DriveId_PoliceVerification")
                    .HasMaxLength(250);

                entity.Property(e => e.EduCert).HasMaxLength(250);

                entity.Property(e => e.ExpCert).HasMaxLength(250);

                entity.Property(e => e.FatherMobileNo).HasMaxLength(12);

                entity.Property(e => e.FatherName).HasMaxLength(250);

                entity.Property(e => e.GuardianMobileNo).HasMaxLength(12);

                entity.Property(e => e.GuardianName).HasMaxLength(250);

                entity.Property(e => e.IdProffFile).HasMaxLength(250);

                entity.Property(e => e.Joining).HasColumnType("date");

                entity.Property(e => e.MotherMobileNo).HasMaxLength(12);

                entity.Property(e => e.MotherName).HasMaxLength(250);

                entity.Property(e => e.Other).HasMaxLength(250);

                entity.Property(e => e.PoliceVerification).HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserProfile)
                    .HasForeignKey<UserProfile>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProfile_Company_UserID");
            });

            modelBuilder.Entity<Weightage>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Emoji)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Weightage)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WeightagePreset>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.WeightagePreset)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WeightagePresetDetails>(entity =>
            {
                entity.HasOne(d => d.Game)
                    .WithMany(p => p.WeightagePresetDetails)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WeightagePresetDetails)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WeightagePresetDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WeightagePreset)
                    .WithMany(p => p.WeightagePresetDetails)
                    .HasForeignKey(d => d.WeightagePresetId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
