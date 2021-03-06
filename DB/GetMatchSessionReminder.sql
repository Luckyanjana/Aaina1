GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetMatchSessionReminder] 
	@sessionId INT,
	@fromDate DATETIME
AS
BEGIN
	
	
	declare @date date	declare @datetime datetime	declare @time time(0)	declare @name NVARCHAR(500)	declare @t_days table	(		[the_day] tinyint not null	)	declare @t_valid_days table	(		[the_day_name] varchar(20) not null,		[diff] tinyint not null	)	declare @t_times table	(		[the_time] time	)	DECLARE @t_Dates TABLE	(		[the_Date] DATE	)	declare @Type tinyint	declare @Frequency tinyint	declare @StartDate datetime	declare @EndDate datetime	declare @TimeStart time(0)	declare @RecurseEvery tinyint	declare @DailyFrequency tinyint	declare @OccursEveryValue tinyint	declare @OccursEveryTimeUnit tinyint	declare @TimeEnd time(0)	declare @DaysOfWeek varchar(20)	declare @MonthlyOccurrence char(1)	declare @ExactDateOfMonth tinyint	declare @ExactWeekdayOfMonth tinyint	declare @ExactWeekdayOfMonthEvery tinyint	declare @ValidDays varchar(20)	declare @colorCode varchar(20)	declare @duration INT	SELECT		@Type = [Type],		@name=Venue,		@Frequency = Frequency,		@StartDate = (CASE WHEN StartDate >=@fromDate THEN  CAST(FORMAT(StartDate,'yyyy/MM/dd')+' '+FORMAT(CAST(TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) ELSE CAST(FORMAT(@fromDate,'yyyy/MM/dd')+' '+FORMAT(CAST(TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) END),		@EndDate =@fromDate,		@TimeStart = TimeStart,		@RecurseEvery = RecurseEvery,		@DailyFrequency = DailyFrequency,		@OccursEveryValue = ISNULL(OccursEveryValue,6),		@OccursEveryTimeUnit = ISNULL(OccursEveryTimeUnit,1),		@TimeEnd = ISNULL(TimeEnd,'23:59:59'),		@DaysOfWeek = DaysOfWeek,		@MonthlyOccurrence = MonthlyOccurrence,		@ExactDateOfMonth = ExactDateOfMonth,		@ExactWeekdayOfMonth = ExactWeekdayOfMonth,		@ExactWeekdayOfMonthEvery = ExactWeekdayOfMonthEvery,		@ValidDays = ValidDays,		@duration=Duration,		@colorCode=ColorCode	FROM [dbo].[SessionScheduler] WHERE sessionid=@sessionId			IF (@Type=1)	BEGIN	IF @fromdate=CAST(FORMAT(@StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(@TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) BEGIN SELECT @sessionId,CAST(FORMAT(@StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(@TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) END	END	ELSE 	BEGIN		INSERT INTO @t_times(the_time) VALUES (@TimeStart)	IF @DailyFrequency = 2		BEGIN						DECLARE @ssTime DATETIME;			SET @ssTime=CAST(@TimeStart AS DATETIME);						WHILE  @ssTime >= CAST(@TimeStart AS DATETIME) AND @ssTime <= CAST('23:59:59' AS DATETIME)			BEGIN				if @OccursEveryTimeUnit = 1					set @ssTime = dateadd(hour, @OccursEveryValue, @ssTime)				else if @OccursEveryTimeUnit = 2					set @ssTime = dateadd(minute, @OccursEveryValue, @ssTime)				else if @OccursEveryTimeUnit = 3					set @ssTime = dateadd(second, @OccursEveryValue, @ssTime)									if @ssTime >= CAST(@TimeStart AS DATETIME) AND @ssTime <= CAST('23:59:59' AS DATETIME)				insert into @t_times(the_time) values (CAST(@ssTime AS TIME))								end		end					IF @Frequency=1	BEGIN		SET @date=@StartDate;	INSERT INTO @t_Dates VALUES(@date)	 WHILE DATEADD(DAY, @RecurseEvery, @date) <= CONVERT(DATE, @EndDate)			BEGIN			 			 				SET @date = DATEADD(DAY, @RecurseEvery, @date)								INSERT INTO @t_Dates VALUES(@date)				END	END		
	IF @Frequency = 2
		begin
			-- Get days of week
			if CHARINDEX('0', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (0)
			if CHARINDEX('1', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (1)
			if CHARINDEX('2', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (2)
			if CHARINDEX('3', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (3)
			if CHARINDEX('4', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (4)
			if CHARINDEX('5', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (5)
			if CHARINDEX('6', @DaysOfWeek) > 0
				insert into @t_days(the_day) values (6)
	
			SET @date = @StartDate
		
			---- Go to the beginning of the week - Monday
			WHILE datepart(dw, @date) <> 1 -- Monday
				SET @date = dateadd(day, -1, @date)
				
			INSERT INTO @t_Dates							
			SELECT dateadd(DAY, the_day, @date) from @t_days

			WHILE DATEADD(WEEK, @RecurseEvery, @date) <= convert(date, @EndDate)
			BEGIN
				SET @date = dateadd(week, @RecurseEvery, @date)
				INSERT INTO @t_Dates							
				SELECT dateadd(DAY, the_day, @date) from @t_days
				END

			
		END	IF @Frequency = 3
		BEGIN
			SET @date = @StartDate
		
			-- Go to the beginning of the month
			SET @date = DATEADD(month, DATEDIFF(month, 0, @date), 0)
			
			-- ============================================================
			-- MONTHLY RECURRING SCHEDULER ON EXACT DAY / DATE
			-- ============================================================
			IF @MonthlyOccurrence = 1
			BEGIN
				-- Go to exact day of the month				
				SET @date = DATEADD(DAY, @ExactDateOfMonth - 1, @date)

				INSERT INTO @t_Dates VALUES(@date)
			while @date <= @EndDate
				BEGIN
				SET @date =DATEADD(DAY, @ExactDateOfMonth, @date)
				INSERT INTO @t_Dates VALUES(@date)
					
					END
			END

			-- ============================================================
			-- MONTHLY RECURRING SCHEDULER ON EXACT WEEKDAY
			-- ============================================================
			ELSE IF @MonthlyOccurrence = 2
			BEGIN
				-- Go to exact weekday of the month
				

				WHILE @ExactWeekdayOfMonth <> (DATEPART(WEEKDAY, @date)-1)
				BEGIN
						SET @date = DATEADD(DAY, 1, @date)						
				END
			END

			INSERT INTO @t_Dates VALUES(@date)
			while @date <= @EndDate
				BEGIN
				SET @date =DATEADD(WEEK, @ExactWeekdayOfMonthEvery, @date)
				INSERT INTO @t_Dates VALUES(@date)
					
					END
		END		SELECT @sessionId AS SessionId,StartDate FROM	(SELECT CAST(FORMAT(d.the_Date,'yyyy-MM-dd')+' '+FORMAT(CAST(t.the_time AS DATETIME),'HH:mm:ss') AS DATETIME) AS StartDate from @t_Dates d 	CROSS JOIN @t_times t		) a 
	where StartDate=@fromDate
	 order by startdate
	 END

	

END
