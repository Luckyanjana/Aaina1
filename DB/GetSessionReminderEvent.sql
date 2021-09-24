
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSessionReminderNotification]
	@currentDateTime DATETIME,
	@StartDate DATETIME ,
	@id INT
AS
BEGIN
	
		DECLARE @t_Dates TABLE
	(
		[the_Date] DATETIME
	)
	DECLARE @sessionId INT;
	DECLARE @Type TINYINT;
	DECLARE @RecurseEvery TINYINT
	DECLARE @OccursEveryTimeUnit TINYINT

	SELECT
		@Type = [TypeId],
		@sessionId = SessionId,
		@RecurseEvery = Every,
		@OccursEveryTimeUnit=Unit
	FROM [dbo].[SessionReminder] WHERE Id=@id


		 WHILE @StartDate <= @currentDateTime
				BEGIN			 			 
					
					IF @OccursEveryTimeUnit=1
					SET @StartDate = DATEADD(MINUTE, @RecurseEvery, @StartDate)	
								
					ELSE  IF @OccursEveryTimeUnit=2
					SET @StartDate = DATEADD(HOUR, @RecurseEvery, @StartDate)

					ELSE  IF @OccursEveryTimeUnit=3
					SET @StartDate = DATEADD(DAY, @RecurseEvery, @StartDate)

					ELSE  IF @OccursEveryTimeUnit=4
					SET @StartDate = DATEADD(WEEK, @RecurseEvery, @StartDate)

					INSERT INTO @t_Dates VALUES(@StartDate)
				END
	

	SELECT @sessionId AS SessionId,the_Date AS NotiDate FROM @t_Dates WHERE FORMAT(the_Date,'yyyy-MM-dd HH:mm')=FORMAT(@currentDateTime,'yyyy-MM-dd HH:mm')
END
GO
