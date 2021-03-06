
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetCompanyLookNotification]
	@currentDateTime DATETIME
AS
BEGIN
	


	DECLARE @lookTbl TABLE(Id INT,GameId INT,StartDate DATETIME)

DECLARE @reminTbl TABLE(Id INT,GameId INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT l.Id,l.GameId,CAST(FORMAT(ls.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ls.TimeStart AS DATETIME),'HH:mm') AS DATETIME)  FROM Look l 
INNER JOIN LookScheduler ls ON l.Id=ls.LookId
WHERE l.IsActive=1 AND l.IsSchedule=1 AND (( l.TypeId=2 AND CAST(@currentDateTime AS DATE) BETWEEN  CAST(l.FromDate AS DATE) AND CAST(l.ToDate AS DATE) )
OR l.TypeId=1 AND CAST(FORMAT(ls.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(ls.TimeStart AS DATETIME),'HH:mm') AS DATETIME)=@currentDateTime
)

DECLARE @totalRecord INT,@indexTbl INT=1;

SELECT @totalRecord=COUNT(*) FROM @reminTbl

 WHILE @indexTbl <= @totalRecord
				BEGIN	
				DECLARE @StartDate DATETIME, @id INT, @gameId INT
				SELECT  @id=Id,@gameId=GameId FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

				INSERT INTO @lookTbl
				EXEC [dbo].[GetLookNotificationByLookId] @currentDateTime,@id,@gameId

				SET @indexTbl= @indexTbl+1;

				END


				SELECT * FROM @lookTbl

END
