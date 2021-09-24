

	CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,
	[Location] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	)

	CREATE TABLE [dbo].[UserLogin](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,
	[UserType] [int] NOT NULL,
	[UserName] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](250) NOT NULL,
	[FName] [nvarchar](250) NOT NULL,
	[Mname] [nvarchar](250) NULL,
	[Lname] [nvarchar](250) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[SaltKey] [nvarchar](250) NOT NULL,
	[AvatarUrl] [nvarchar](250) NULL,
	IsEmailVerify BIT NOT NULL,
	[IsActive] [bit] NOT NULL,
	[PasswordResetLink] [nvarchar](max) NULL,
	[LinkExpiredDate] [datetime] NULL,
	[IsForgotVerified] [bit] NOT NULL,
	[DOB] DATETIME,
	[Gender] int,
	[MobileNo] [nvarchar](250) NULL,
	[Address] [nvarchar](250) NULL,
	[City] [nvarchar](250) NULL,
	[State] [nvarchar](250) NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,	
	CONSTRAINT [FK_UserLogin_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id])
	)

	INSERT INTO Company(Name,IsActive,AddedDate,ModifiedDate) VALUES('Kandira Soft',1,GETDATE(),GETDATE())
	INSERT INTO UserLogin(CompanyId,UserType,UserName,Email,FName,Lname,Password,SaltKey,IsEmailVerify,IsActive,IsForgotVerified,AddedDate,ModifiedDate)
	VALUES(1,1,'super.admin@yopmail.com','super.admin@yopmail.com','Surendra','Kandira','X2hzaXJg8NSEkJy04/TYCQoluyHGiJ/hSTsD84I80LQ=','8XrrjOPoQtOOvpY6VfvvPw==',1,1,0,GETDATE(),GETDATE())
	-- password is 123456
	--alter table UserLogin add UserName [nvarchar](250)
	--update UserLogin set UserName=Email
	--alter table UserLogin alter column UserName [nvarchar](250) NOT NULL

	CREATE TABLE [dbo].[UserProfile](
	UserID INT PRIMARY KEY NOT NULL,
	Joining Date NULL,
	FatherName [nvarchar](250) NULL,
	[FatherMobileNo] [nvarchar](12) NULL,
	MotherName [nvarchar](250) NULL,
	[MotherMobileNo] [nvarchar](12) NULL,
	GuardianName [nvarchar](250) NULL,
	[GuardianMobileNo] [nvarchar](12) NULL,
	IdProofType INT,
	IdProffFile [nvarchar](250) NULL,
	EduCert [nvarchar](250) NULL,
	ExpCert [nvarchar](250) NULL,
	PoliceVerification [nvarchar](250) NULL,
	Other [nvarchar](250) NULL,
	[PlayerType] [int] DEFAULT(1) NOT NULL,
	CONSTRAINT [FK_UserProfile_Company_UserID] FOREIGN KEY([UserID]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	CREATE TABLE [dbo].[Weightage](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Rating] float NOT NULL,
	[Emoji] [nvarchar](250) NOT NULL,
	IsActive BIT NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Weightage_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id])
	)

	
	CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Weightage] float NOT NULL,
	[Desciption] [nvarchar](500) NULL,
	[ColorCode] [nvarchar](50) NULL,
	IsActive BIT NOT NULL,
	[PlayerType] [int] DEFAULT(1) NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Role_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id])	
	)

	

	CREATE TABLE [dbo].[Game](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[ParentId] [int]  NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[Weightage] float NOT NULL,
	FromDate DATETIME NULL,
	TODate DATETIME NULL,
	ClientName nvarchar(250),
	ApplyForChild BIT NOT NULL,
	Location nvarchar(250),
	ContactPerson nvarchar(250),
	ContactNumber nvarchar(12),
	IsActive BIT NOT NULL,
	CreatedBy INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Game_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Game_Game_ParentId] FOREIGN KEY([ParentId]) REFERENCES [dbo].[Game] ([Id])	,
	CONSTRAINT [FK_Game_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	
	CREATE TABLE [dbo].[GameLocation](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[GameId] INT NOT NULL,	
	[Location] NVARCHAR(1000) NULL,	
	CONSTRAINT [FK_GameLocation_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])
	)

	CREATE TABLE [dbo].[GamePlayer](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[UserId] [int] NOT NULL,	
	[GameId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,	
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_GamePlayer_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_GamePlayer_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	,
	CONSTRAINT [FK_GamePlayer_Role_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Role] ([Id])	,
	CONSTRAINT [FK_GamePlayer_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)


	CREATE TABLE [dbo].[Team](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[GameId] [int]  NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[Weightage] float NOT NULL,
	IsActive BIT NOT NULL,
	CreatedBy INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Team_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Team_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_Team_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)


	CREATE TABLE [dbo].[TeamPlayer](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,		
	[TeamId] [int] NOT NULL,
	[UserId] [int] NOT NULL,	
	[TypeId] [int] NOT NULL,	
	[RoleId] [int] NOT NULL,	
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_TeamPlayer_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_TeamPlayer_Team_TeamId] FOREIGN KEY([TeamId]) REFERENCES [dbo].[Team] ([Id])	,
	CONSTRAINT [FK_TeamPlayer_Role_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Role] ([Id])	,
	CONSTRAINT [FK_TeamPlayer_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE [dbo].[Attribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[GameId] [int]  NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,
	IsActive BIT NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Attribute_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Attribute_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	
	)

	CREATE TABLE [dbo].[SubAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,	
	[AttributeId] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,	
	[Weightage] [FLOAT] DEFAULT(1) NOT NULL,
	[Desciption] [nvarchar](500) NULL,
	IsQuantity BIT DEFAULT(0) NOT NULL,
	UnitId INT NULL,
	CONSTRAINT [FK_SubAttribute_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES [dbo].[Attribute] ([Id])	
	)

	CREATE TABLE [dbo].[Look](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[GameId] [int]  NULL,	
	[TypeId] [int] NOT NULL,	
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[FromDate] DATETIME NOT NULL,
	[ToDate] DATETIME NOT NULL,
	[CalculationType] INT NOT NULL,
	[IsSchedule] BIT NOT NULL,
	IsActive BIT NOT NULL,
	CreatedBy INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Look_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Look_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_Look_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	CREATE TABLE [dbo].[LookAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[AttributeId] INT NOT NULL,
	CONSTRAINT [FK_LookAttribute_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookAttribute_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES [dbo].[Attribute] ([Id])	
	)

	CREATE TABLE [dbo].[LookSubAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[SubAttributeId] INT NOT NULL,
	CONSTRAINT [FK_LookSubAttribute_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookSubAttribute_SubAttribute_SubAttributeId] FOREIGN KEY([SubAttributeId]) REFERENCES [dbo].[SubAttribute] ([Id])	
	)

	CREATE TABLE [dbo].[LookGame](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[GameId] INT NOT NULL,
	CONSTRAINT [FK_LookGame_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookGame_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	
	)

	CREATE TABLE [dbo].[LookPlayers](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[UserId] INT NOT NULL,
	IsView BIT NOT NULL,
	IsCalculation BIT NOT NULL,
	CONSTRAINT [FK_LookPlayers_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookPlayers_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	--------------------- 14Th Feb 2021----------------------------------------------------------
	CREATE TABLE [dbo].[LookGroup](	
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[Name] [varchar](250) NULL,	
	CONSTRAINT [FK_LookGroup_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id])
	)

	CREATE TABLE [dbo].[LookGroupPlayer](	
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookGroupId] [int] NOT NULL,
	[UserId] INT NOT NULL,
	CONSTRAINT [FK_LookGroupPlayer_LookGroup_LookGroupId] FOREIGN KEY([LookGroupId]) REFERENCES [dbo].[LookGroup] ([Id]),	
	CONSTRAINT [FK_LookGroupPlayer_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE [dbo].[LookTeam](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[TeamId] INT NOT NULL,
	CONSTRAINT [FK_LookTeam_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookTeam_Team_TeamId] FOREIGN KEY([TeamId]) REFERENCES [dbo].[Team] ([Id])	
	)

	CREATE TABLE [dbo].[LookUser](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[LookId] [int] NOT NULL,
	[UserId] INT NOT NULL,
	CONSTRAINT [FK_LookUser_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),	
	CONSTRAINT [FK_LookUser_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	

	CREATE TABLE [dbo].[LookScheduler](	
	[LookId] [int] PRIMARY KEY NOT NULL,
	[Name] [varchar](250) NULL,
	[Type] [tinyint] NOT NULL, --1= one time ,2= recurring
	[Frequency] [tinyint] NULL, --1= daily, 2=weekly,3= monthly
	[RecurseEvery] [tinyint] NULL, -- scheduler recurs every X days, weeks, months
	[DaysOfWeek] [varchar](20) NULL, -- Mo-Th-Fr
	[MonthlyOccurrence] [tinyint] NULL, -- 1=days (12th day of month), 2= Week (Second Sunday), 3=Months (Thrird month)
	[ExactDateOfMonth] [tinyint] NULL, -- 12th day
	[ExactWeekdayOfMonth] [tinyint] NULL,-- 1=sun,2=mon,3=tue ....
	[ExactWeekdayOfMonthEvery] [tinyint] NULL,--1=once per day/week/month,2= every second day/week/month, 3=every third day/week/month
	[DailyFrequency] [tinyint] NULL, -- 1=once per day,2= every x hours/day		
	[TimeStart] [time] NOT NULL,
	[OccursEveryValue] [tinyint] NULL,--
	[OccursEveryTimeUnit] [tinyint] NULL, --1=Hours,2=Min,3=Sec
	[TimeEnd] [time] NULL,
	[ValidDays] [varchar](20) NULL, -- Mo-Tu-We-Th-Fr
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	CONSTRAINT [FK_LookScheduler_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id])
	)

	CREATE TABLE [dbo].[PushNotificationToken](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[UserId] INT  NULL,	
	[TokenId] NVARCHAR(MAX)		
	CONSTRAINT [FK_PushNotificationToken_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)
	
	

	CREATE TABLE [dbo].[GameFeedback](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[LookId] [int] NOT  NULL,	
	[UserId] [int] NOT NULL,	
	[IsDraft] [BIT] NOT NULL,	
	[FeedbackDate] DATETIME NOT NULL,	
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_GameFeedback_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_GameFeedback_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),
	CONSTRAINT [FK_GameFeedback_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	

	CREATE TABLE [dbo].[GameFeedbackDetails](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[GameFeedbackId] [int] NOT NULL,	
	[GameId] [int] NOT NULL,	
	[AttributeId] [INT] NOT NULL,
	[SubAttributeId] [int] NOT NULL,	
	[Feedback] [nvarchar](250) NOT NULL,
	 [Quantity] FLOAT NULL,
	CONSTRAINT [FK_GameFeedbackDetails_GameFeedback_GameFeedbackId] FOREIGN KEY(GameFeedbackId) REFERENCES [dbo].[GameFeedback] ([Id]),	
	CONSTRAINT [FK_GameFeedbackDetails_Game_GameId] FOREIGN KEY(GameId) REFERENCES [dbo].[Game] ([Id]),	
	CONSTRAINT [FK_GameFeedbackDetails_SubAttribute_SubAttributeId] FOREIGN KEY(SubAttributeId) REFERENCES [dbo].[SubAttribute] ([Id]),
	CONSTRAINT [FK_GameFeedbackDetails_Attribute_AttributeId] FOREIGN KEY(AttributeId) REFERENCES [dbo].[Attribute] ([Id])	
	)


	CREATE TABLE [dbo].[TeamFeedback](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[LookId] [int] NOT  NULL,	
	[UserId] [int] NOT NULL,	
	[IsDraft] [BIT] NOT NULL,	
	[FeedbackDate] DATETIME NOT NULL,	
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_TeamFeedback_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_TeamFeedback_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),
	CONSTRAINT [FK_TeamFeedback_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	

	CREATE TABLE [dbo].[TeamFeedbackDetails](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[TeamFeedbackId] [int] NOT NULL,	
	[TeamId] [int] NOT NULL,	
	[AttributeId] [INT] NOT NULL,
	[SubAttributeId] [int] NOT NULL,	
	[Feedback] [nvarchar](250) NOT NULL,
	[Quantity] FLOAT NULL,
	CONSTRAINT [FK_TeamFeedbackDetails_TeamFeedback_TeamFeedbackId] FOREIGN KEY(TeamFeedbackId) REFERENCES [dbo].[TeamFeedback] ([Id]),	
	CONSTRAINT [FK_TeamFeedbackDetails_Team_TeamId] FOREIGN KEY(TeamId) REFERENCES [dbo].[Team] ([Id]),	
	CONSTRAINT [FK_TeamFeedbackDetails_SubAttribute_SubAttributeId] FOREIGN KEY(SubAttributeId) REFERENCES [dbo].[SubAttribute] ([Id]),
	CONSTRAINT [FK_TeamFeedbackDetails_Attribute_AttributeId] FOREIGN KEY(AttributeId) REFERENCES [dbo].[Attribute] ([Id])	
	)


	CREATE TABLE [dbo].[UserFeedback](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[LookId] [int] NOT  NULL,	
	[UserId] [int] NOT NULL,	
	[IsDraft] [BIT] NOT NULL,	
	[FeedbackDate] DATETIME NOT NULL,	
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_UserFeedback_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_UserFeedback_Look_LookId] FOREIGN KEY([LookId]) REFERENCES [dbo].[Look] ([Id]),
	CONSTRAINT [FK_UserFeedback_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	
	

	CREATE TABLE [dbo].[UserFeedbackDetails](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[UserFeedbackId] [int] NOT NULL,	
	[UserId] [int] NOT NULL,	
	[AttributeId] [INT] NOT NULL,
	[SubAttributeId] [int] NOT NULL,	
	[Feedback] [nvarchar](250) NOT NULL,
	[Quantity] FLOAT NULL,
	CONSTRAINT [FK_UserFeedbackDetails_UserFeedback_UserFeedbackId] FOREIGN KEY(UserFeedbackId) REFERENCES [dbo].[UserFeedback] ([Id]),	
	CONSTRAINT [FK_UserFeedbackDetails_User_UserId] FOREIGN KEY(UserId) REFERENCES [dbo].[UserLogin] ([Id]),	
	CONSTRAINT [FK_UserFeedbackDetails_SubAttribute_SubAttributeId] FOREIGN KEY(SubAttributeId) REFERENCES [dbo].[SubAttribute] ([Id]),
	CONSTRAINT [FK_UserFeedbackDetails_Attribute_AttributeId] FOREIGN KEY(AttributeId) REFERENCES [dbo].[Attribute] ([Id])	
	)


	CREATE TABLE [dbo].[WeightagePreset](	
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[GameId] [int] NOT NULL,
	[Name] [varchar](250) NOT NULL,
	IsDefault BIT NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_WeightagePreset_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])
	)

	
	CREATE TABLE [dbo].[WeightagePresetDetails](	
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[WeightagePresetId] [int] NOT NULL,
	[GameId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[UserId] INT NOT NULL,
	[Weightage] float NOT NULL,
	CONSTRAINT [FK_WeightagePresetDetails_WeightagePreset_WeightagePresetId] FOREIGN KEY([WeightagePresetId]) REFERENCES [dbo].[WeightagePreset] ([Id]),		
	CONSTRAINT [FK_WeightagePresetDetails_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_WeightagePresetDetails_Role_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Role] ([Id]),
	CONSTRAINT [FK_WeightagePresetDetails_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	

--------------------- 25Th Feb 2021----------------------------------------------------------

CREATE TABLE [dbo].[Session](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[GameId] [int]  NULL,		
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[TypeId] [int] NOT NULL,	
	[ModeId] [int] NOT NULL,
	Deadline INT,
	IsActive BIT NOT NULL,
	[CreatedBy] INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Session_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Session_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	
	)

	
CREATE TABLE [dbo].[SessionParticipant](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[SessionId] [int] NOT NULL,	
	[UserId] [int]  NULL,				
	[TypeId] [int] NOT NULL,	
	[ParticipantTyprId] [int] NOT NULL,		
	[Status] [int]  DEFAULT(1) NOT NULL,   -- 1. Pending,2 Confirmed,3 Rejected
	[Remarks] NVARCHAR(500) NULL,		
	CONSTRAINT [FK_SessionParticipant_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id]),	
	CONSTRAINT [FK_SessionParticipant_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)
	   
	
	CREATE TABLE [dbo].[SessionReminder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[SessionId] [int] NOT NULL,					
	[TypeId] [int] NOT NULL,	
	[Every] [int] NOT NULL,		
	[Unit] [int] NOT NULL,		
	CONSTRAINT [FK_SessionReminder_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])	
	)
	
	CREATE TABLE [dbo].[SessionScheduler](	
	[SessionId] [int] PRIMARY KEY NOT NULL,	
	[Venue] [varchar](250) NOT NULL,
	[Type] [tinyint] NOT NULL, --1= one time ,2= recurring
	[Frequency] [tinyint] NULL, --1= daily, 2=weekly,3= monthly
	[RecurseEvery] [tinyint] NULL, -- scheduler recurs every X days, weeks, months
	[DaysOfWeek] [varchar](20) NULL, -- Mo-Th-Fr
	[MonthlyOccurrence] [tinyint] NULL, -- 1=days (12th day of month), 2= Week (Second Sunday), 3=Months (Thrird month)
	[ExactDateOfMonth] [tinyint] NULL, -- 12th day
	[ExactWeekdayOfMonth] [tinyint] NULL,-- 1=sun,2=mon,3=tue ....
	[ExactWeekdayOfMonthEvery] [tinyint] NULL,--1=once per day/week/month,2= every second day/week/month, 3=every third day/week/month
	[DailyFrequency] [tinyint] NULL, -- 1=once per day,2= every x hours/day		
	[TimeStart] [time] NOT NULL,
	[OccursEveryValue] [tinyint] NULL,--
	[OccursEveryTimeUnit] [tinyint] NULL, --1=Hours,2=Min,3=Sec
	[TimeEnd] [time] NULL,
	[ValidDays] [varchar](20) NULL, -- Mo-Tu-We-Th-Fr
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	Duration INT NULL,
	ColorCode NVARCHAR(10) NULL,
	CONSTRAINT [FK_SessionScheduler_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])
	)


	CREATE TABLE [dbo].[Filter](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,		
	[Name] [nvarchar](250) NOT NULL,	
	[StartDateTime] [DateTime] NOT NULL,	
	[EndDateTime] [Datetime] NOT NULL,	
	[EmotionsFor] INT NOT NULL,
	[EmotionsFrom] INT NOT NULL,
	[EmotionsFromP] INT NOT NULL,
	[CalculationType] INT NOT NULL,
	[IsSelf] BIT DEFAULT(0) NOT NULL,
	GameId INT NOT NULL,
	CreatedBy INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Filter_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),
	CONSTRAINT [FK_Filter_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id]),
	CONSTRAINT [FK_Filter_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])
	)
	CREATE TABLE [dbo].[FilterEmotionsFor](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FilterId] [int] NOT NULL,		
	[ForId] INT NOT NULL,	
	CONSTRAINT [FK_FilterEmotionsFor_Filter_FilterId] FOREIGN KEY([FilterId]) REFERENCES [dbo].[Filter] ([Id]),		
	)

	CREATE TABLE [dbo].[FilterEmotionsFrom](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FilterId] [int] NOT NULL,		
	[FromId] INT NOT NULL,	
	CONSTRAINT [FK_FilterEmotionsFrom_Filter_FilterId] FOREIGN KEY([FilterId]) REFERENCES [dbo].[Filter] ([Id]),		
	)

	
	CREATE TABLE [dbo].[FilterEmotionsFromP](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FilterId] [int] NOT NULL,		
	[FromId] INT NOT NULL,	
	CONSTRAINT [FK_FilterEmotionsFromP_Filter_FilterId] FOREIGN KEY([FilterId]) REFERENCES [dbo].[Filter] ([Id]),		
	)


	CREATE TABLE [dbo].[FilterAttributes](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FilterId] [int] NOT NULL,		
	[AttributeId] INT NOT NULL,	
	CONSTRAINT [FK_FilterAttributes_Filter_FilterId] FOREIGN KEY([FilterId]) REFERENCES [dbo].[Filter] ([Id]),		
	)

		

	CREATE TABLE [dbo].[FilterPlayers](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FilterId] [int] NOT NULL,		
	[UserId] INT NOT NULL,
	IsView BIT NOT NULL,
	IsCalculation BIT NOT NULL,
	CONSTRAINT [FK_FilterPlayers_Filter_FilterId] FOREIGN KEY([FilterId]) REFERENCES [dbo].[Filter] ([Id])	
	)
	
   

	CREATE TABLE [dbo].[NotificationReminder](
	Id BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Type] INT NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
	[Reason] NVARCHAR(500),
	[SendBy] INT  NULL,
	[SendTo] INT NOT NULL,
	[SendDate] DATETIME NOT NULL,
	[IsRead] BIT NOT NULL,
	[ReadDate] DATETIME,
	CONSTRAINT [FK_NotificationReminder_UserLogin_SendBy] FOREIGN KEY([SendBy]) REFERENCES [dbo].[UserLogin] ([Id])	,
	CONSTRAINT [FK_NotificationReminder_UserLogin_UserId] FOREIGN KEY([SendTo]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	
	CREATE TABLE [dbo].[Play](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ParentId] [int] NULL,
	[Type] INT NOT NULL,
	[GameId] INT NOT NULL,
	[SubGameId] INT NULL,
	[Name] NVARCHAR(500) NOT NULL,
	[Description] NVARCHAR(1000) NULL,
	[AccountableId] INT NOT NULL,
	[DependancyId] INT  NULL,
	[Priority] INT NOT NULL,
	[Status] INT NOT NULL,
	[AddedOn] DATETIME NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[DeadlineDate] DATETIME NOT NULL,
	[Emotion] [nvarchar](250) NULL,
	[PHOEmotion] [nvarchar](250) NULL,
	FeedbackType INT NULL,
	ActualStartDate DATETIME NULL,
	ActualEndDate DATETIME NULL,
	[Comments] NVARCHAR(2000) NULL,
	IsActive BIT NOT NULL,
	[IsToday] BIT NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Play_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Play_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_Play_SubGame_SubGameId] FOREIGN KEY([SubGameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_Play_UserLogin_AccountableId] FOREIGN KEY([AccountableId]) REFERENCES [dbo].[UserLogin] ([Id])	,
	CONSTRAINT [FK_Play_UserLogin_DependancyId] FOREIGN KEY([DependancyId]) REFERENCES [dbo].[UserLogin] ([Id])	,
	CONSTRAINT [FK_Play_Play_ParentId] FOREIGN KEY([ParentId]) REFERENCES [dbo].[Play] ([Id])
	)
	
	
	
	CREATE TABLE [dbo].[PlayDelegate](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PlayId] INT NOT NULL,
	[AccountableId] INT NOT NULL,
	[DelegateId] INT NOT NULL,
	[Description] NVARCHAR(1000) NULL,
	DelegateDate DATETIME NOT NULL,
	CONSTRAINT [FK_PlayDelegate_Play_PlayId] FOREIGN KEY([PlayId]) REFERENCES [dbo].[Play] ([Id]),
	CONSTRAINT [FK_PlayDelegate_UserLogin_AccountableId] FOREIGN KEY([AccountableId]) REFERENCES [dbo].[UserLogin] ([Id]),
	CONSTRAINT [FK_PlayDelegate_UserLogin_DelegateId] FOREIGN KEY([DelegateId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)


	
CREATE TABLE [dbo].[PlayPersonInvolved](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PlayId] INT NOT NULL,
	[UserId] INT NOT NULL,
	CONSTRAINT [FK_PlayPersonInvolved_Play_PlayId] FOREIGN KEY([PlayId]) REFERENCES [dbo].[Play] ([Id]),
	CONSTRAINT [FK_PlayPersonInvolved_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)
--------------------- 20Th Mar 2021----------------------------------------------------------

	CREATE TABLE [dbo].[Status](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[GameId] [int]  NULL,		
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[StatusId] [int] NOT NULL,	
	[EstimatedTime] INT,
	IsActive BIT NOT NULL,
	[CreatedBy] INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_Status_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Status_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	
	)

	CREATE TABLE [dbo].[StatusReminder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,					
	[TypeId] [int] NOT NULL,	
	[Every] [int] NOT NULL,		
	[Unit] [int] NOT NULL,		
	CONSTRAINT [FK_StatusReminder_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status]([Id])	
	)
	
	CREATE TABLE [dbo].[StatusScheduler](	
	[StatusId] [int] PRIMARY KEY NOT NULL,	
	[Venue] [varchar](250) NOT NULL,
	[Type] [tinyint] NOT NULL, --1= one time ,2= recurring
	[Frequency] [tinyint] NULL, --1= daily, 2=weekly,3= monthly
	[RecurseEvery] [tinyint] NULL, -- scheduler recurs every X days, weeks, months
	[DaysOfWeek] [varchar](20) NULL, -- Mo-Th-Fr
	[MonthlyOccurrence] [tinyint] NULL, -- 1=days (12th day of month), 2= Week (Second Sunday), 3=Months (Thrird month)
	[ExactDateOfMonth] [tinyint] NULL, -- 12th day
	[ExactWeekdayOfMonth] [tinyint] NULL,-- 1=sun,2=mon,3=tue ....
	[ExactWeekdayOfMonthEvery] [tinyint] NULL,--1=once per day/week/month,2= every second day/week/month, 3=every third day/week/month
	[DailyFrequency] [tinyint] NULL, -- 1=once per day,2= every x hours/day		
	[TimeStart] [time] NOT NULL,
	[OccursEveryValue] [tinyint] NULL,--
	[OccursEveryTimeUnit] [tinyint] NULL, --1=Hours,2=Min,3=Sec
	[TimeEnd] [time] NULL,
	[ValidDays] [varchar](20) NULL, -- Mo-Tu-We-Th-Fr
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	Duration INT NULL,
	ColorCode NVARCHAR(10) NULL,
	CONSTRAINT [FK_StatusScheduler_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id])
	)

	CREATE TABLE [dbo].[StatusGameBy](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,		
	[GameId] INT NOT NULL,	
	CONSTRAINT [FK_StatusGameBy_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),	
	CONSTRAINT [FK_StatusGameBy_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	
	)

	CREATE TABLE [dbo].[StatusTeamBy](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,		
	[TeamId] INT NOT NULL,	
	CONSTRAINT [FK_StatusTeamBy_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	CONSTRAINT [FK_StatusTeamBy_Team_TeamId] FOREIGN KEY([TeamId]) REFERENCES [dbo].[Team] ([Id])	
	)

	CREATE TABLE [dbo].[StatusUserBy](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,		
	[UserId] INT NOT NULL,	
	CONSTRAINT [FK_StatusUserBy_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),	
	CONSTRAINT [FK_StatusUserBy_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE [dbo].[StatusTeamFor](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,		
	[TeamId] INT NOT NULL,	
	CONSTRAINT [FK_StatusTeamFor_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),	
	CONSTRAINT [FK_StatusTeamFor_Team_TeamId] FOREIGN KEY([TeamId]) REFERENCES [dbo].[Team] ([Id])	
	)

	CREATE TABLE [dbo].[StatusUserFor](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] [int] NOT NULL,		
	[UserId] INT NOT NULL,	
	CONSTRAINT [FK_StatusUserFor_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),	
	CONSTRAINT [FK_StatusUserFor_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	
	--------------------- 21Th Mar 2021----------------------------------------------------------



	CREATE TABLE [dbo].[FormBuilder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[CreatedBy] [int]  NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	CONSTRAINT [FK_FormBuilder_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)


	CREATE TABLE [dbo].[FormBuilderAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FormBuilderId] [int] NOT NULL,
	[AttributeName] [nvarchar](1000) NOT NULL,
	[DataType] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[OrderNo] [int] NOT NULL,
	CONSTRAINT [FK_FormBuilderAttribute_FormBuilder_FormBuilderId] FOREIGN KEY([FormBuilderId]) REFERENCES [dbo].[FormBuilder] ([Id])
	)

	CREATE TABLE [dbo].[FormBuilderAttributeLookUp](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FormBuilderAttributeId] [int] NOT NULL,
	[OptionName] [nvarchar](500) NOT NULL,
	CONSTRAINT [FK_FormBuilderAttributeLookUp_FormBuilderAttribute_FormBuilderAttributeId] FOREIGN KEY([FormBuilderAttributeId]) REFERENCES [dbo].[FormBuilderAttribute] ([Id])
	)

	CREATE TABLE [dbo].[FormBuilderAttributeValue](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ReportGiveId INT NOT NULL,
	[FormBuilderAttributeId] [int] NOT NULL,	
	[AttributeValue] [nvarchar](1000) NOT NULL,
	[LookUpId] [int] NULL,
	CONSTRAINT [FK_FormBuilderAttributeValue_ReportGive_ReportGiveId] FOREIGN KEY([ReportGiveId]) REFERENCES [dbo].[ReportGive] ([Id]),
	CONSTRAINT [FK_FormBuilderAttributeValue_FormBuilderAttribute_FormBuilderAttributeId] FOREIGN KEY([FormBuilderAttributeId]) REFERENCES [dbo].[FormBuilderAttribute] ([Id])
	)

	CREATE TABLE [dbo].[ReportTemplate](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,	
	[FormBuilderId] INT NOT NULL,
	[GameId] [int]  NULL,		
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,	
	[TypeId] [int] NOT NULL,
	[AccountAbilityId] INT NULL,
	IsActive BIT NOT NULL,
	CreatedBy INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_ReportTemplate_FormBuilder_FormBuilderId] FOREIGN KEY([FormBuilderId]) REFERENCES [dbo].[FormBuilder] ([Id]),	
	CONSTRAINT [FK_ReportTemplate_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_ReportTemplate_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_ReportTemplate_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)
	
	CREATE TABLE [dbo].[ReportTemplateScheduler](	
	[ReportTemplateId] [int] PRIMARY KEY NOT NULL,	
	[Venue] [varchar](250) NOT NULL,
	[Type] [tinyint] NOT NULL, --1= one time ,2= recurring
	[Frequency] [tinyint] NULL, --1= daily, 2=weekly,3= monthly
	[RecurseEvery] [tinyint] NULL, -- scheduler recurs every X days, weeks, months
	[DaysOfWeek] [varchar](20) NULL, -- Mo-Th-Fr
	[MonthlyOccurrence] [tinyint] NULL, -- 1=days (12th day of month), 2= Week (Second Sunday), 3=Months (Thrird month)
	[ExactDateOfMonth] [tinyint] NULL, -- 12th day
	[ExactWeekdayOfMonth] [tinyint] NULL,-- 1=sun,2=mon,3=tue ....
	[ExactWeekdayOfMonthEvery] [tinyint] NULL,--1=once per day/week/month,2= every second day/week/month, 3=every third day/week/month
	[DailyFrequency] [tinyint] NULL, -- 1=once per day,2= every x hours/day		
	[TimeStart] [time] NOT NULL,
	[OccursEveryValue] [tinyint] NULL,--
	[OccursEveryTimeUnit] [tinyint] NULL, --1=Hours,2=Min,3=Sec
	[TimeEnd] [time] NULL,
	[ValidDays] [varchar](20) NULL, -- Mo-Tu-We-Th-Fr
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	Duration INT NULL,
	ColorCode NVARCHAR(10) NULL,
	CONSTRAINT [FK_ReportTemplateScheduler_ReportTemplate_ReportTemplateId] FOREIGN KEY([ReportTemplateId]) REFERENCES [dbo].[ReportTemplate] ([Id])
	)

	CREATE TABLE [dbo].[ReportTemplateReminder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ReportTemplateId] [int] NOT NULL,					
	[TypeId] [int] NOT NULL,	
	[Every] [int] NOT NULL,		
	[Unit] [int] NOT NULL,		
	CONSTRAINT [FK_ReportTemplateReminder_ReportTemplate_ReportTemplateId] FOREIGN KEY([ReportTemplateId]) REFERENCES [dbo].[ReportTemplate]([Id])	
	)

	CREATE TABLE [dbo].[ReportTemplateGame](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ReportTemplateId] [int] NOT NULL,		
	[GameId] INT NOT NULL,	
	CONSTRAINT [FK_ReportTemplateGame_ReportTemplate_ReportTemplateId] FOREIGN KEY([ReportTemplateId]) REFERENCES [dbo].[ReportTemplate] ([Id])
	)
	
	
	CREATE TABLE [dbo].[ReportTemplateUser](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ReportTemplateId] [int] NOT NULL,		
	[UserId] INT NOT NULL,	
	[TypeId] INT NOT NULL,
	[AccountAbilityId] INT NULL,
	CONSTRAINT [FK_ReportTemplateUser_ReportTemplate_ReportTemplateId] FOREIGN KEY([ReportTemplateId]) REFERENCES [dbo].[ReportTemplate] ([Id]),
	CONSTRAINT [FK_ReportTemplateUser_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)


	CREATE TABLE [dbo].[PreSession]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,
	[SessionId] [int] NOT NULL,	
	[StartDate] [DATETIME] NOT NULL,
	[EndDate] [DATETIME] NULL,
	[CreatedBy] INT NULL,
	[AddedDate] [datetime] NOT NULL,		
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_PreSession_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])	
	)

	CREATE TABLE [dbo].[PreSessionParticipant]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PreSessionId] [int] NOT NULL,	
	[UserId] [int] NOT NULL,				
	[TypeId] [int] NOT NULL,	
	[ParticipantTyprId] [int] NOT NULL,		
	[Status] [int] NOT NULL,		
	[Remarks] NVARCHAR(500) NULL,		
	CONSTRAINT [FK_PreSessionParticipant_PreSession_PreSessionId] FOREIGN KEY([PreSessionId]) REFERENCES [dbo].[PreSession] ([Id]),	
	CONSTRAINT [FK_PreSessionParticipant_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE [dbo].[PreSessionAgenda]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PreSessionId] [int] NOT NULL,		
	[PlayId] INT NOT NULL,
	IsApproved BIT DEFAULT(0) NOT NULL,
	CONSTRAINT [FK_PreSessionAgenda_PreSession_PreSessionId] FOREIGN KEY([PreSessionId]) REFERENCES [dbo].[PreSession] ([Id]),	
	CONSTRAINT [FK_PreSessionAgenda_Play_PlayId] FOREIGN KEY([PlayId]) REFERENCES [dbo].[Play] ([Id])
	)

	CREATE TABLE [dbo].[PreSessionAgendaDoc]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PreSessionAgendaId] [int] NOT NULL,		
	[FileName] NVARCHAR(1000) NOT NULL
	CONSTRAINT [FK_PreSessionAgendaDoc_PreSessionAgenda_PreSessionId] FOREIGN KEY([PreSessionAgendaId]) REFERENCES [dbo].[PreSessionAgenda] ([Id])
	)

	
	CREATE TABLE [dbo].[StatusFeedback](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] INT NOT NULL,
	[UserId] INT NOT NULL,
	[ActualTime] INT,
	[CreatedDate] DATETIME NOT NULL,
	CONSTRAINT [FK_StatusFeedback_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	CONSTRAINT [FK_StatusFeedback_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	CREATE TABLE [dbo].[StatusFeedbackDetail](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusFeedbackId] INT NOT NULL,
	[GameId] INT NOT NULL,
	[SubGameId] INT NULL,
	[FeedbackDate] DATETIME NOT NULL,
	[Feedback] NVARCHAR(MAX) NOT NULL,
	[Status] INT NOT NULL,
	[Progress] INT NOT NULL,
	[Emotion] [nvarchar](250) NOT NULL,
	CONSTRAINT [FK_StatusFeedbackDetail_Play_PlayId] FOREIGN KEY([StatusFeedbackId]) REFERENCES [dbo].[StatusFeedback] ([Id]),
	CONSTRAINT [FK_StatusFeedbackDetail_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_StatusFeedbackDetail_Game_SubGameId] FOREIGN KEY([SubGameId]) REFERENCES [dbo].[Game] ([Id])
	)

	CREATE TABLE [dbo].[StatusSnooze](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[StatusId] INT NOT NULL,
	[UserId] INT NOT NULL,
	[SendDateTime] DATETIME NOT NULL,
	CONSTRAINT [FK_StatusSnooze_Status_StatusId] FOREIGN KEY([StatusId]) REFERENCES [dbo].[Status] ([Id]),
	CONSTRAINT [FK_StatusSnooze_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)


	CREATE TABLE [dbo].[Poll](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,		
	[GameId] [int]  NULL,
	[SubGameId] [int]  NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Desciption] [nvarchar](500) NULL,		
	[IsActive] BIT NOT NULL,
	[CreatedBy] INT NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,	
	CONSTRAINT [FK_Poll_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_Poll_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id])	,
	CONSTRAINT [FK_Poll_Game_SubGameId] FOREIGN KEY([SubGameId]) REFERENCES [dbo].[Game] ([Id])	,
	CONSTRAINT [FK_Poll_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE [dbo].[PollQuestion](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollId] [int] NOT NULL,					
	[QuestionTypeId] [int] NOT NULL,
	[Name] NVARCHAR(500) NULL,
	CONSTRAINT [FK_PollQuestion_Poll_PollId] FOREIGN KEY([PollId]) REFERENCES [dbo].[Poll]([Id])	
	)

	CREATE TABLE [dbo].[PollQuestionOption](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollQuestionId] [int] NOT NULL,					
	[Name] NVARCHAR(250) NULL,
	[FilePath] NVARCHAR(250) NULL,
	CONSTRAINT [FK_PollQuestionOption_PollQuestion_PollQuestionId] FOREIGN KEY([PollQuestionId]) REFERENCES [dbo].[PollQuestion]([Id])	
	)

	CREATE TABLE [dbo].[PollScheduler](	
	[PollId] [int] PRIMARY KEY NOT NULL,		
	[Type] [tinyint] NOT NULL, --1= one time ,2= recurring
	[Frequency] [tinyint] NULL, --1= daily, 2=weekly,3= monthly
	[RecurseEvery] [tinyint] NULL, -- scheduler recurs every X days, weeks, months
	[DaysOfWeek] [varchar](20) NULL, -- Mo-Th-Fr
	[MonthlyOccurrence] [tinyint] NULL, -- 1=days (12th day of month), 2= Week (Second Sunday), 3=Months (Thrird month)
	[ExactDateOfMonth] [tinyint] NULL, -- 12th day
	[ExactWeekdayOfMonth] [tinyint] NULL,-- 1=sun,2=mon,3=tue ....
	[ExactWeekdayOfMonthEvery] [tinyint] NULL,--1=once per day/week/month,2= every second day/week/month, 3=every third day/week/month
	[DailyFrequency] [tinyint] NULL, -- 1=once per day,2= every x hours/day		
	[TimeStart] [time] NOT NULL,
	[OccursEveryValue] [tinyint] NULL,--
	[OccursEveryTimeUnit] [tinyint] NULL, --1=Hours,2=Min,3=Sec
	[TimeEnd] [time] NULL,
	[ValidDays] [varchar](20) NULL, -- Mo-Tu-We-Th-Fr
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL	
	CONSTRAINT [FK_PollScheduler_Poll_PollId] FOREIGN KEY([PollId]) REFERENCES [dbo].[Poll] ([Id])
	)

	
	CREATE TABLE [dbo].[PollReminder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollId] [int] NOT NULL,					
	[TypeId] [int] NOT NULL,	
	[Every] [int] NOT NULL,		
	[Unit] [int] NOT NULL,		
	CONSTRAINT [FK_PollReminder_Poll_PollId] FOREIGN KEY([PollId]) REFERENCES [dbo].[Poll]([Id])	
	)

	CREATE TABLE [dbo].[PollParticipants](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollId] [int] NOT NULL,		
	[UserId] INT NOT NULL,	
	[TypeId] INT NOT NULL,
	CONSTRAINT [FK_PollParticipants_Poll_PollId] FOREIGN KEY([PollId]) REFERENCES [dbo].[Poll] ([Id]),
	CONSTRAINT [FK_PollParticipants_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	
CREATE TABLE [dbo].[PollFeedback](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollId] INT NOT NULL,
	[UserId] INT NOT NULL,	
	[Remark] NVARCHAR(500),
	[CreatedDate] DATETIME NOT NULL,
	CONSTRAINT [FK_PollFeedback_Poll_PollId] FOREIGN KEY([PollId]) REFERENCES [dbo].[Poll] ([Id]),
	CONSTRAINT [FK_PollFeedback_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	

	CREATE TABLE [dbo].[PollQuestionFeedback](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PollFeedbackId] INT NOT NULL,	
	[PollQuestionId] INT NOT NULL,
	[PollQuestionOptionId] INT NOT NULL,
	[Remark] NVARCHAR(500),
	CONSTRAINT [FK_PollQuestionFeedback_PollFeedback_PollFeedbackId] FOREIGN KEY(PollFeedbackId) REFERENCES [dbo].[PollFeedback] ([Id]),
	CONSTRAINT [FK_PollQuestionFeedback_PollQuestion_PollQuestionId] FOREIGN KEY([PollQuestionId]) REFERENCES [dbo].[PollQuestion]([Id])	,
	CONSTRAINT [FK_PollQuestionFeedback_PollQuestionOption_PollQuestionOptionId] FOREIGN KEY([PollQuestionOptionId]) REFERENCES [dbo].[PollQuestionOption]([Id])	
	)

	CREATE TABLE [dbo].[PreSessionStatus]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,
	[SessionId] [int] NOT NULL,	
	[StartDate] [DATETIME] NOT NULL,
	[EndDate] [DATETIME] NULL,
	[Status] INT NOT NULL,
	[Re-DateTime] DATETIME,
	[DecisionMakerId] INT NULL,
	[AddedDate] [datetime] NOT NULL,		
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_PreSessionStatus_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])	,
	CONSTRAINT [FK_PreSessionStatus_UserLogin_DecisionMakerId] FOREIGN KEY([DecisionMakerId]) REFERENCES [dbo].[UserLogin] ([Id]),
	)

	CREATE TABLE [dbo].[PreSessionDelegate](
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[SessionId] INT NOT NULL,
	[DecisionMakerId] INT NOT NULL,
	[DelegateId] INT NOT NULL,
	[Description] NVARCHAR(1000) NULL,
	DelegateDate DATETIME NOT NULL,
	CONSTRAINT [FK_PreSessionDelegate_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id]),
	CONSTRAINT [FK_PreSessionDelegate_UserLogin_DecisionMakerId] FOREIGN KEY([DecisionMakerId]) REFERENCES [dbo].[UserLogin] ([Id]),
	CONSTRAINT [FK_PreSessionDelegate_UserLogin_DelegateId] FOREIGN KEY([DelegateId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

CREATE TABLE ReportGive(
Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
ReportId INT,
UserId INT,
CONSTRAINT [FK_ReportGive_Report_ReportId] FOREIGN KEY([ReportId]) REFERENCES [dbo].[ReportTemplate] ([Id])	,
CONSTRAINT [FK_ReportGive_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
)

CREATE TABLE [dbo].[FormBuilderAttributeValue](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ReportGiveId INT NOT NULL,
	[FormBuilderAttributeId] [int] NOT NULL,	
	[AttributeValue] [nvarchar](1000) NOT NULL,
	[LookUpId] [int] NULL,
	CONSTRAINT [FK_FormBuilderAttributeValue_ReportGive_ReportGiveId] FOREIGN KEY([ReportGiveId]) REFERENCES [dbo].[ReportGive] ([Id]),
	CONSTRAINT [FK_FormBuilderAttributeValue_FormBuilderAttribute_FormBuilderAttributeId] FOREIGN KEY([FormBuilderAttributeId]) REFERENCES [dbo].[FormBuilderAttribute] ([Id])
	)





CREATE TABLE [dbo].[PostSession]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] [int] NOT NULL,
	[SessionId] [int] NOT NULL,	
	[StartDate] [DATETIME] NOT NULL,
	[EndDate] [DATETIME] NULL,
	[CreatedBy] INT NULL,
	[AddedDate] [datetime] NOT NULL,		
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_PostSession_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])	
	)

	
	CREATE TABLE [dbo].[PostSessionAgenda]
	(
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[PostSessionId] [int] NOT NULL,		
	[Type] INT NOT NULL,
	[GameId] INT NOT NULL,
	[SubGameId] INT NULL,
	[Name] NVARCHAR(500) NOT NULL,
	[Description] NVARCHAR(1000) NULL,
	[AccountableId] INT NOT NULL,
	[DependancyId] INT NULL,
	[Priority] INT NOT NULL,
	[Status] INT NOT NULL,
	[AddedOn] DATETIME NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[DeadlineDate] DATETIME NOT NULL,
	[FeedbackType] INT NULL,
	[ActualStartDate] DATETIME NULL,
	[ActualEndDate] DATETIME NULL,
	[Remarks] NVARCHAR(500) NULL,
	[Emotions] INT NULL,
	CONSTRAINT [FK_PostSessionAgenda_PostSession_PostSessionId] FOREIGN KEY([PostSessionId]) REFERENCES [dbo].[PostSession] ([Id]),	
	CONSTRAINT [FK_PostSessionAgenda_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),	
	CONSTRAINT [FK_PostSessionAgenda_UserLogin_AccountableId] FOREIGN KEY([AccountableId]) REFERENCES [dbo].[UserLogin] ([Id])	,
	CONSTRAINT [FK_PostSessionAgenda_UserLogin_DependancyId] FOREIGN KEY([DependancyId]) REFERENCES [dbo].[UserLogin] ([Id])	
	)

	CREATE TABLE PreSessionGroup(
Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
SessionId INT NOT NULL,
GroupName NVARCHAR(250) NOT NULL,
StartDate DATETIME NOT NULL,
EndDate DATETIME NOT NULL,
IsComplete BIT NOT NULL,
CONSTRAINT [FK_PreSessionGroup_Session_SessionId] FOREIGN KEY([SessionId]) REFERENCES [dbo].[Session] ([Id])
)

CREATE TABLE PreSessionGroupDetails(
Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
PreSessionGroupId INT NOT NULL,
UserId INT NOT NULL,
[Message] NVARCHAR(MAX) NOT NULL,
SendDate DATETIME NOT NULL,
CONSTRAINT [FK_PreSessionGroupDetails_PreSessionGroup_PreSessionGroupId] FOREIGN KEY([PreSessionGroupId]) REFERENCES [dbo].[PreSessionGroup] ([Id]),
CONSTRAINT [FK_PreSessionGroupDetails_UserLogin_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[UserLogin] ([Id])
)



CREATE TABLE Chat_Group(
Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Name NVARCHAR(250) NOT NULL,
CretedBy INT NOT NULL,
Created DATETIME,
IsDeleted BIT NOT NULL,
DeletedDate DATE NULL
)

CREATE TABLE Chat_Group_User(
Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
GroupId INT NOT NULL,
UserId INT NOT NULL,
JoinDate DATETIME,
IsDeleted BIT NOT NULL,
DeletedDate DATE NULL
)

CREATE TABLE Chat_Message(
Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
SenderId INT NOT NULL,
ReceiverId INT NOT NULL,
ReceiverType TINYINT NOT NULL,
Message NVARCHAR(MAX) NOT NULL,
SendDate DATETIME NOT NULL,
IsRead BIT NOT NULL
)

CREATE TABLE [dbo].[UserMenuPermission](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[UserId] [int] NULL,
	[MenuId] [int] NULL,
	[IsView] [bit] NOT NULL,
	[IsList] [bit] NOT NULL,
	[IsAdd] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[IsEdit] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL
)