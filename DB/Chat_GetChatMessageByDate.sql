
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*

EXEC [dbo].[Chat_GetChatMessageByDate] 77,60,1,'2021-05-17','2021-05-21'

EXEC [dbo].[Chat_GetChatMessageByDate] 77,1,2,'2021-05-17','2021-05-21'

*/


CREATE PROCEDURE [dbo].[Chat_GetChatMessageByDate] 
	@senderId INT,
	@receiverId INT,
	@receiveType TINYINT,
	@fromDate DATETIME,
	@toDate DATETIME
AS
BEGIN
	
select SenderId,u.FName+' '+u.Lname  AS SenderName,ReceiverId,
(CASE WHEN AvatarUrl IS NOT NULL THEN '/DYF/'+CAST(CompanyId AS NVARCHAR(20))+'/EmployeeImages/'+AvatarUrl ELSE '/img/Default_avatar.jpg' END)AS ProfileImage,
Message,SendDate,IsRead from Chat_Message m
INNER JOIN UserLogin u ON m.SenderId=u.Id
WHERE 
(
	(
		@receiveType =1 AND 
		( 
			(SenderId=@senderId AND ReceiverId=@receiverId) OR 
			(SenderId=@receiverId AND ReceiverId=@senderId) 
		) 
	) OR 
	(
	@receiveType=2 AND
	ReceiverId=@receiverId
	)
) AND 
ReceiverType=@receiveType
AND CAST(SendDate AS DATE) BETWEEN @fromDate AND @toDate

ORDER BY SendDate DESC

END
GO


