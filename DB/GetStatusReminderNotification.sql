USE [aiinna_test]
GO
/****** Object:  StoredProcedure [dbo].[GetStatusReminderNotification]    Script Date: 4/8/2021 8:45:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetStatusReminderNotification]
	-- Add the parameters for the stored procedure here
	@currentDateTime DATETIME
AS
BEGIN
		
	
	DECLARE @StatusUserTbl TABLE(Email varchar(max))

	DECLARE @reminTbl TABLE(Id INT,StartDate DATETIME)

	INSERT INTO @reminTbl
	SELECT s.Id,CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM Status s 
	INNER JOIN StatusScheduler ss ON s.Id=ss.StatusId
	INNER JOIN StatusReminder r ON s.Id=r.StatusId
	WHERE r.TypeId=1 AND s.IsActive=1
	AND @currentDateTime BETWEEN  CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) 
	AND CAST(FORMAT(ISNULL(ss.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(ss.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)


	DECLARE @totalRecord INT,@indexTbl INT=1;

	SELECT @totalRecord=COUNT(*) FROM @reminTbl

	 WHILE @indexTbl <= @totalRecord
					BEGIN	
					DECLARE @StartDate DATETIME, @id INT
					SELECT  @id=Id,@StartDate=StartDate FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

					INSERT INTO @StatusUserTbl
					Select u.Email from StatusGameBy sgb
					inner join Game g on sgb.GameId = g.Id
					inner join GamePlayer gp on gp.GameId = g.Id
					inner join UserLogin u on u.Id = gp.UserId
					where sgb.StatusId = @id

					
					INSERT INTO @StatusUserTbl
					Select u.Email from StatusTeamBy stb
					inner join Team t on stb.TeamId = t.Id
					inner join TeamPlayer tp on tp.TeamId = t.Id
					inner join UserLogin u on u.Id = tp.UserId
					where stb.StatusId = @id
				
				
					INSERT INTO @StatusUserTbl
					Select u.Email from StatusUserBy sub
					inner join UserLogin u on u.Id = sub.UserId
					where sub.StatusId = @id

					SET @indexTbl= @indexTbl+1;

					END


					SELECT Distinct Email FROM @StatusUserTbl
END
