
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*
GetCompanyReportEvent 8,58,NULL,'2021-05-01','2021-05-31'
*/
CREATE PROCEDURE [dbo].[GetCompanyReportEvent]
	@companyId INT,
	@gameId INT=null,
	@userId INT=null,
	@fromDate DATE,
	@toDate DATE
AS
BEGIN
	
	DECLARE @tblSession TABLE(id INT)
IF @userId IS NULL 
BEGIN
INSERT INTO @tblSession
SELECT  s.Id FROM ReportTemplate s INNER JOIN [ReportTemplateScheduler] ss ON s.Id=ss.ReportTemplateId WHERE s.CompanyId=@companyId AND s.IsActive=1 AND GameId= ISNULL(@gameId,s.GameId) AND
((ss.StartDate BETWEEN @fromDate AND @toDate) OR 
(ss.EndDate IS NOT NULL AND ss.EndDate BETWEEN @fromDate AND @toDate)
OR (ss.EndDate IS NULL AND (ss.StartDate<=@fromDate OR ss.StartDate<@toDate) )
)
END
ELSE
BEGIN
INSERT INTO @tblSession
SELECT s.Id FROM ReportTemplate s INNER JOIN [ReportTemplateScheduler] ss ON s.Id=ss.ReportTemplateId WHERE s.CompanyId=@companyId AND s.IsActive=1 AND GameId= ISNULL(@gameId,s.GameId)
AND s.Id IN(SELECT DISTINCT s.Id FROM ReportTemplate s INNER JOIN ReportTemplateUser sp ON s.Id=sp.ReportTemplateId where s.CompanyId=@companyId AND sp.UserId=@userId) AND
((ss.StartDate BETWEEN @fromDate AND @toDate) OR 
(ss.EndDate IS NOT NULL AND ss.EndDate BETWEEN @fromDate AND @toDate)
OR (ss.EndDate IS NULL AND (ss.StartDate<=@fromDate OR ss.StartDate<@toDate) )
)
END


DECLARE @tblResult TABLE(Id INT,Title NVARCHAR(250),[Start] DATETIME,[End] DATETIME,BackgroundColor NVARCHAR(10),BorderColor NVARCHAR(10))

DECLARE @sessionId INT

DECLARE db_cursor CURSOR FOR 
SELECT id  FROM @tblSession 

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @sessionId  

WHILE @@FETCH_STATUS = 0  
BEGIN  
 
 INSERT INTO @tblResult
      EXEC dbo.GetReportEvent @sessionId,@fromDate,@toDate

      FETCH NEXT FROM db_cursor INTO @sessionId 
END 

CLOSE db_cursor  
DEALLOCATE db_cursor 



SELECT * FROM @tblResult

END
