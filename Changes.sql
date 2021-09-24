
	DROP TABLE [dbo].[FormBuilderAttributeValue]
	DROP TABLE [dbo].[ReportGive]
	DROP TABLE [dbo].[ReportTemplateUser]
	DROP TABLE [dbo].[ReportTemplateGame]
	DROP TABLE [dbo].[ReportTemplateReminder]
	DROP TABLE [dbo].[ReportTemplateScheduler]
	DROP TABLE [dbo].[ReportTemplate]

	DROP TABLE [dbo].[FormBuilderAttributeLookUp]
	DROP TABLE [dbo].[FormBuilderAttribute]
	DROP TABLE [dbo].[FormBuilder]
	

	CREATE TABLE [dbo].[FormBuilder](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CompanyId] INT NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Header] [nvarchar](4000)  NULL,
	[Footer] [nvarchar](4000)  NULL,
	[CreatedBy] [int]  NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	CONSTRAINT [FK_FormBuilder_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),
	CONSTRAINT [FK_FormBuilder_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)	

	CREATE TABLE [dbo].[FormBuilderAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FormBuilderId] [int] NOT NULL,
	[AttributeName] [nvarchar](1000) NOT NULL,
	[DataType] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[OrderNo] [int] NOT NULL,
	[DBColumnName] [nvarchar](100) NULL,
	CONSTRAINT [FK_FormBuilderAttribute_FormBuilder_FormBuilderId] FOREIGN KEY([FormBuilderId]) REFERENCES [dbo].[FormBuilder] ([Id])
	)	

	CREATE TABLE [dbo].[FormBuilderAttributeLookUp](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[FormBuilderAttributeId] [int] NOT NULL,
	[OptionName] [nvarchar](500) NOT NULL,
	CONSTRAINT [FK_FormBuilderAttributeLookUp_FormBuilderAttribute_FormBuilderAttributeId] FOREIGN KEY([FormBuilderAttributeId]) REFERENCES [dbo].[FormBuilderAttribute] ([Id])
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
	[EntityType] INT NULL,
	IsActive BIT NOT NULL,
	[CreatedBy] INT NULL,
	[AddedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	CONSTRAINT [FK_ReportTemplate_FormBuilder_FormBuilderId] FOREIGN KEY([FormBuilderId]) REFERENCES [dbo].[FormBuilder] ([Id]),	
	CONSTRAINT [FK_ReportTemplate_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id]),	
	CONSTRAINT [FK_ReportTemplate_Game_GameId] FOREIGN KEY([GameId]) REFERENCES [dbo].[Game] ([Id]),
	CONSTRAINT [FK_ReportTemplate_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	CREATE TABLE [dbo].[ReportTemplateEntity](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ReportId] [int] NOT NULL,		
	[EntityId] INT NOT NULL,	
	CONSTRAINT [FK_FilterEmotionsFor_ReportTemplate_ReportId] FOREIGN KEY([ReportId]) REFERENCES [dbo].[ReportTemplate] ([Id]),		
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
	
	
	alter table [dbo].[ReportGive] add AssignToType INT


	CREATE TABLE [dbo].[ReportGive] (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	ReportId INT NOT NULL,
	CreatedBy INT NOT NULL,
	CreatedDate DATETIME NOT NULL,
	Remark NVARCHAR(MAX) ,
	RemarkedBy INT ,
	AssignToType INT,
	CONSTRAINT [FK_ReportGive_Report_ReportId] FOREIGN KEY([ReportId]) REFERENCES [dbo].[ReportTemplate] ([Id])	,
	CONSTRAINT [FK_ReportGive_UserLogin_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[UserLogin] ([Id]),
	CONSTRAINT [FK_ReportGive_UserLogin_RemarkedBy] FOREIGN KEY([RemarkedBy]) REFERENCES [dbo].[UserLogin] ([Id])
	)

	CREATE TABLE [dbo].[ReportGiveDetails] (
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[ReportGiveId] INT NOT NULL,
	EntityId INT,
	Remark NVARCHAR(MAX) ,
	RemarkedBy INT ,
	CONSTRAINT [FK_ReportGiveDetails_UserLogin_RemarkedBy] FOREIGN KEY([RemarkedBy]) REFERENCES [dbo].[UserLogin] ([Id]),
	CONSTRAINT [FK_ReportGive_Report_ReportGiveId] FOREIGN KEY([ReportGiveId]) REFERENCES [dbo].[ReportGive] ([Id])	
	)
	
	

	CREATE TABLE [dbo].[ReportGiveAttributeValue](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,	
	[ReportGiveDetailId] INT NOT NULL,
	[FormBuilderAttributeId] [int] NOT NULL,	
	[AttributeValue] [nvarchar](1000)  NULL,
	[LookUpId] [int] NULL,	
	CONSTRAINT [FK_ReportGiveAttributeValue_ReportGiveDetails_ReportGiveDetailId] FOREIGN KEY([ReportGiveDetailId]) REFERENCES [dbo].[ReportGiveDetails] ([Id]),
	CONSTRAINT [FK_ReportGiveAttributeValue_FormBuilderAttribute_FormBuilderAttributeId] FOREIGN KEY([FormBuilderAttributeId]) REFERENCES [dbo].[FormBuilderAttribute] ([Id]),
	CONSTRAINT [FK_ReportGiveAttributeValue_FormBuilderAttributeLookUp_LookUpId] FOREIGN KEY([LookUpId]) REFERENCES [dbo].[FormBuilderAttributeLookUp] ([Id])
	)

	

