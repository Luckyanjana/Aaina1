USE [aiinna_test]
GO
/****** Object:  StoredProcedure [dbo].[GetSessionParticipantReminderEmail]    Script Date: 4/8/2021 8:47:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetSessionParticipantReminderEmail]
	@currentDateTime DATETIME
AS
BEGIN
	
	
DECLARE @StatusUserTbl TABLE(Email varchar(max))


DECLARE @reminTbl TABLE(Id INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT s.Id,CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM Session s 
INNER JOIN SessionScheduler ss ON s.Id=ss.SessionId
INNER JOIN SessionReminder r ON s.Id=r.SessionId
WHERE s.TypeId=2 AND r.TypeId=1 AND s.IsActive=1
AND @currentDateTime BETWEEN  CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) 
AND CAST(FORMAT(ISNULL(ss.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(ss.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)


DECLARE @totalRecord INT,@indexTbl INT=1;

SELECT @totalRecord=COUNT(*) FROM @reminTbl

 WHILE @indexTbl <= @totalRecord
				BEGIN	
				DECLARE @StartDate DATETIME, @id INT
				SELECT  @id=Id,@StartDate=StartDate FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

				INSERT INTO @StatusUserTbl
				Select Email from SessionParticipant sp 
				inner join UserLogin ul on sp.UserId = ul.Id  where SessionId = @id

				SET @indexTbl= @indexTbl+1;

				END


				SELECT Distinct * FROM @StatusUserTbl
END
