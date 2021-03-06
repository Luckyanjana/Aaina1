USE [aiinna_test]
GO
/****** Object:  StoredProcedure [dbo].[GetReportTemplateReminderMail]    Script Date: 4/8/2021 8:46:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetReportTemplateReminderMail]
	@currentDateTime DATETIME
AS
BEGIN
	
	
DECLARE @StatusUserTbl TABLE(Email varchar(max))


DECLARE @reminTbl TABLE(Id INT,StartDate DATETIME)

INSERT INTO @reminTbl
SELECT rt.Id,CAST(FORMAT(rts.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(rts.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME)  FROM ReportTemplate rt 
INNER JOIN ReportTemplateScheduler rts ON rt.Id=rts.ReportTemplateId
INNER JOIN ReportTemplateReminder rtr ON rt.Id=rtr.ReportTemplateId
WHERE rt.TypeId=2 AND rtr.TypeId=1 AND rt.IsActive=1
AND @currentDateTime BETWEEN  CAST(FORMAT(rts.StartDate,'yyyy-MM-dd')+' '+FORMAT(CAST(rts.TimeStart AS DATETIME),'HH:mm:ss') AS DATETIME) 
AND CAST(FORMAT(ISNULL(rts.EndDate,GETDATE()),'yyyy-MM-dd')+' '+FORMAT(ISNULL(CAST(rts.TimeEnd AS DATETIME),GETDATE()),'HH:mm:ss') AS DATETIME)


DECLARE @totalRecord INT,@indexTbl INT=1;

SELECT @totalRecord=COUNT(*) FROM @reminTbl

 WHILE @indexTbl <= @totalRecord
				BEGIN	
				DECLARE @StartDate DATETIME, @id INT
				SELECT  @id=Id,@StartDate=StartDate FROM @reminTbl ORDER BY StartDate ASC  OFFSET (@indexTbl-1) ROWS FETCH NEXT 1 ROWS ONLY

				INSERT INTO @StatusUserTbl
				Select Email from ReportTemplateUser sp 
				inner join UserLogin ul on sp.UserId = ul.Id  where ReportTemplateId = @id

				SET @indexTbl= @indexTbl+1;

				END


				SELECT Distinct * FROM @StatusUserTbl
END
