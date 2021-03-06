
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetCompanySessionReminderNotification]
	@currentDateTime DATETIME,
	@reminterType INT
AS
BEGIN

SET @currentDateTime=CAST(FORMAT(@currentDateTime,'yyyy-MM-dd HH:mm') AS DATETIME)	
	
DECLARE @SessionTbl TABLE(Id INT,StartDate DATETIME)

DECLARE @reminTbl TABLE(SessionId INT,Id INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT s.id AS SessionId,r.Id,CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM Session s 
INNER JOIN SessionScheduler ss ON s.Id=ss.SessionId
INNER JOIN SessionReminder r ON s.Id=r.SessionId
WHERE s.TypeId=2 AND r.TypeId=@reminterType AND s.IsActive=1
--AND @currentDateTime BETWEEN  CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) 
--AND CAST(FORMAT(ISNULL(ss.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(ss.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)
AND  @currentDateTime<=CAST(FORMAT(ISNULL(ss.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(ss.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)


DECLARE @totalRecord INT,@indexTbl INT=1;

SELECT @totalRecord=COUNT(*) FROM @reminTbl

 WHILE @indexTbl <= @totalRecord
				BEGIN	
				DECLARE @StartDate DATETIME, @id INT,@sessionId INT
				SELECT  @sessionId=sessionId,@id=Id,@StartDate=StartDate FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

				DECLARE @d1 DATETIME=@currentDateTime
				SELECT @d1=[dbo].[FnGetSessionReminderDate] (@id,@currentDateTime)  

				INSERT INTO @SessionTbl
			EXEC [dbo].[GetMatchSessionReminder] @sessionId,@d1

				SET @indexTbl= @indexTbl+1;

				END


				SELECT * FROM @SessionTbl
END
