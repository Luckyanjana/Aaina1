
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
	
	DECLARE @tblSession TABLE(id INT)
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
DEALLOCATE db_cursor 

END
GO