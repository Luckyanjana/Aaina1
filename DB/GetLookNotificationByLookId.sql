
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetLookNotificationByLookId]
	@currentDateTime DATETIME,
	@id INT,
	@gameId INt

AS
BEGIN
	
	declare @date date	declare @datetime datetime	declare @time time(0)	declare @name NVARCHAR(500)	declare @t_days table	(		[the_day] tinyint not null	)	declare @t_valid_days table	(		[the_day_name] varchar(20) not null,		[diff] tinyint not null	)	declare @t_times table	(		[the_time] time	)	DECLARE @t_Dates TABLE	(		[the_Date] DATE	)	declare @Type tinyint	declare @Frequency tinyint	declare @StartDate datetime	declare @EndDate datetime	declare @TimeStart time(0)	declare @RecurseEvery tinyint	declare @DailyFrequency tinyint	declare @OccursEveryValue tinyint	declare @OccursEveryTimeUnit tinyint	declare @TimeEnd time(0)	declare @DaysOfWeek varchar(20)	declare @MonthlyOccurrence char(1)	declare @ExactDateOfMonth tinyint	declare @ExactWeekdayOfMonth tinyint	declare @ExactWeekdayOfMonthEvery tinyint	declare @ValidDays varchar(20)	declare @colorCode varchar(20)	declare @duration INT	SELECT		@Type = [Type],				@Frequency = Frequency,		@StartDate = CAST(FORMAT(StartDate,'yyyy/MM/dd')+' '+FORMAT(CAST(TimeStart AS DATETIME),'HH:mm') AS DATETIME),		@EndDate =CASE WHEN EndDate IS NOT NULL AND EndDate <=@currentDateTime THEN CAST(FORMAT(EndDate,'yyyy/MM/dd')+' '+FORMAT(CAST(ISNULL(TimeEnd,'23:59') AS DATETIME),'HH:mm') AS DATETIME) ELSE CAST(FORMAT(StartDate,'yyyy/MM/dd')+' '+FORMAT(CAST(ISNULL(TimeEnd,'23:59') AS DATETIME),'HH:mm') AS DATETIME) END,		@TimeStart = TimeStart,		@RecurseEvery = RecurseEvery,		@DailyFrequency = DailyFrequency,		@OccursEveryValue = OccursEveryValue,		@OccursEveryTimeUnit = OccursEveryTimeUnit,		@TimeEnd = ISNULL(TimeEnd,'23:59'),		@DaysOfWeek = DaysOfWeek,		@MonthlyOccurrence = MonthlyOccurrence,		@ExactDateOfMonth = ExactDateOfMonth,		@ExactWeekdayOfMonth = ExactWeekdayOfMonth,		@ExactWeekdayOfMonthEvery = ExactWeekdayOfMonthEvery,		@ValidDays = ValidDays	FROM [dbo].[LookScheduler] WHERE LookId=@id			IF (@Type=1)	BEGIN  IF(CAST(FORMAT(@StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(@TimeStart AS DATETIME),'HH:mm') AS DATETIME)=@currentDateTime) BEGIN SELECT @id As LookId,@gameId AS GameId,CAST(FORMAT(@StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(@TimeStart AS DATETIME),'HH:mm') AS DATETIME) AS StartDate  END	END	ELSE 	BEGIN		INSERT INTO @t_times(the_time) VALUES (@TimeStart)	IF @DailyFrequency = 2		BEGIN						DECLARE @ssTime DATETIME;			SET @ssTime=CAST(@TimeStart AS DATETIME);						WHILE  @ssTime >= CAST(@TimeStart AS DATETIME) AND @ssTime <= CAST('23:59:59' AS DATETIME)			BEGIN				if @OccursEveryTimeUnit = 1					set @ssTime = dateadd(hour, @OccursEveryValue, @ssTime)				else if @OccursEveryTimeUnit = 2					set @ssTime = dateadd(minute, @OccursEveryValue, @ssTime)				else if @OccursEveryTimeUnit = 3					set @ssTime = dateadd(second, @OccursEveryValue, @ssTime)									if @ssTime >= CAST(@TimeStart AS DATETIME) AND @ssTime <= CAST('23:59:59' AS DATETIME)				insert into @t_times(the_time) values (CAST(@ssTime AS TIME))															end		end			IF @Frequency=1	BEGIN		SET @date=@StartDate;	INSERT INTO @t_Dates VALUES(@date)	 WHILE DATEADD(DAY, @RecurseEvery, @date) <= CONVERT(DATE, @EndDate)			BEGIN			 			 				SET @date = DATEADD(DAY, @RecurseEvery, @date)								INSERT INTO @t_Dates VALUES(@date)				END	END		
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
		END		IF @DailyFrequency=1 AND @TimeEnd IS NOT NULLBEGINSET @duration=DATEDIFF(MINUTE,@TimeStart,@TimeEnd)ENDselect @id AS LookId,@gameId AS GameId,CAST(FORMAT(d.the_Date,'yyyy-MM-dd')+' '+FORMAT(CAST(t.the_time AS DATETIME),'HH:mm') AS DATETIME) AS StartDate from ( SELECT the_Date FROM @t_Dates where the_Date=CAST(@currentDateTime AS DATE) ) dCROSS JOIN ( SELECT the_time FROM @t_times where the_time=CAST(@currentDateTime AS time) ) t
END

END
