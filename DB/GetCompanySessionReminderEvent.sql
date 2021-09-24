
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
alter PROCEDURE [dbo].[GetCompanySessionReminderNotification]
	@currentDateTime DATETIME
AS
BEGIN
	
	
DECLARE @SessionTbl TABLE(Id INT,StartDate DATETIME)

DECLARE @reminTbl TABLE(Id INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT r.Id,CAST(FORMAT(ss.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ss.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM Session s 
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

				INSERT INTO @SessionTbl
			EXEC [dbo].[GetSessionReminderNotification] @currentDateTime,@StartDate,@id

				SET @indexTbl= @indexTbl+1;

				END


				SELECT * FROM @SessionTbl
END
GO
