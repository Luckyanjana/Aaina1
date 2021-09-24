
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*
GetCompanySesstionEvent 2,1,6,'2021-02-28','2021-04-01'
*/
alter PROCEDURE dbo.GetCompanySesstionEvent
	@companyId INT,
	@gameId INT=null,
	@userId INT=null,
	@fromDate DATE,
	@toDate DATE
AS
BEGIN
	
	DECLARE @tblSession TABLE(id INT)IF @userId IS NULL BEGININSERT INTO @tblSessionSELECT s.Id FROM Session s INNER JOIN SessionScheduler ss ON s.Id=ss.SessionId WHERE s.CompanyId=@companyId AND s.IsActive=1 AND GameId= ISNULL(@gameId,s.GameId) AND((ss.StartDate BETWEEN @fromDate AND @toDate) OR (ss.EndDate IS NOT NULL AND ss.EndDate BETWEEN @fromDate AND @toDate)OR (ss.EndDate IS NULL AND (ss.StartDate<=@fromDate OR ss.StartDate<@toDate) ))ENDELSEBEGININSERT INTO @tblSessionSELECT s.Id FROM Session s INNER JOIN SessionScheduler ss ON s.Id=ss.SessionId WHERE s.CompanyId=@companyId AND s.IsActive=1 AND GameId= ISNULL(@gameId,s.GameId)AND s.Id IN(SELECT DISTINCT s.Id FROM Session s INNER JOIN SessionParticipant sp ON s.Id=sp.SessionId where s.CompanyId=@companyId AND sp.UserId=@userId) AND((ss.StartDate BETWEEN @fromDate AND @toDate) OR (ss.EndDate IS NOT NULL AND ss.EndDate BETWEEN @fromDate AND @toDate)OR (ss.EndDate IS NULL AND (ss.StartDate<=@fromDate OR ss.StartDate<@toDate) ))ENDDECLARE @tblResult TABLE(Id INT,Title NVARCHAR(250),[Start] DATETIME,[End] DATETIME,BackgroundColor NVARCHAR(10),BorderColor NVARCHAR(10))DECLARE @sessionId INTDECLARE db_cursor CURSOR FOR 
SELECT id  FROM @tblSession 

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @sessionId  

WHILE @@FETCH_STATUS = 0  
BEGIN  
 
 INSERT INTO @tblResult
      EXEC dbo.GetSessionEvent @sessionId,@fromDate,@toDate

      FETCH NEXT FROM db_cursor INTO @sessionId 
END 

CLOSE db_cursor  
DEALLOCATE db_cursor SELECT * FROM @tblResult

END
GO
