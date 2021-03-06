
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[GetCompanyPollReminderNotification]
	@currentDateTime DATETIME,
	@reminterType INT
AS
BEGIN

SET @currentDateTime=CAST(FORMAT(@currentDateTime,'yyyy-MM-dd HH:mm') AS DATETIME)	
	select @currentDateTime
DECLARE @PollTbl TABLE(Id INT,StartDate DATETIME)

DECLARE @reminTbl TABLE(PollId INT,Id INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT p.id AS PollId,r.Id,CAST(FORMAT(ps.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ps.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM 
Poll p 
INNER JOIN PollScheduler ps ON p.Id=ps.PollId
INNER JOIN PollReminder r ON p.Id=r.PollId
WHERE r.TypeId=@reminterType AND p.IsActive=1
AND  @currentDateTime<=CAST(FORMAT(ISNULL(ps.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(ps.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)


DECLARE @totalRecord INT,@indexTbl INT=1;

SELECT @totalRecord=COUNT(*) FROM @reminTbl

 WHILE @indexTbl <= @totalRecord
				BEGIN	
				DECLARE @StartDate DATETIME, @id INT,@pollId INT
				SELECT  @pollId=PollId,@id=Id,@StartDate=StartDate FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

				DECLARE @d1 DATETIME=@currentDateTime
				SELECT @d1=[dbo].[FnPollReminderDate] (@id,@currentDateTime)  

				INSERT INTO @PollTbl
			EXEC [dbo].[GetMatchPollReminder] @pollId,@d1

				SET @indexTbl= @indexTbl+1;

				END


				SELECT * FROM @PollTbl
END
